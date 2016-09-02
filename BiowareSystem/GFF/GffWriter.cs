using System;
using System.Collections.Generic;
using System.IO;
using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;

namespace Bioware.GFF
{
    internal class GffWriter
    {
        private readonly GffBase _gb;
        private LatinBinaryWriter _bw;
        private string _ext;
        private GffRootStruct _rt;

        public DoubleDictionary<GffFieldFrame, GffComponent> DfaF;
        public DoubleDictionary<GffStructFrame, GffStruct> DsaS;

        public GffWriter(GffBase gbase)
        {
            _gb = gbase;
        }

        public MemoryStream Save(GffRootStruct root)
        {
            _bw = new LatinBinaryWriter(new MemoryStream());
            _rt = root;
            _ext = root.Extention;
            DfaF = new DoubleDictionary<GffFieldFrame, GffComponent>();
            DsaS = new DoubleDictionary<GffStructFrame, GffStruct>();

            CreateFrames(_rt);
            FillFrames();
            WriteData();

            var res = _bw.BaseStream;
            _bw.Close();
            return (MemoryStream) res;
        }

        public void Save(GffRootStruct root, string path)
        {
            File.WriteAllBytes(path, Save(root).ToArray());
        }

        private uint GetLabelIndex(string lbl)
        {
            if (lbl != null)
            {
                if (!_gb.LabelArray.Contains(lbl))
                {
                    _gb.LabelArray.Add(lbl);
                    return (uint) _gb.LabelArray.Count - 1;
                }
                return (uint) _gb.LabelArray.IndexOf(lbl);
            }
            return uint.MaxValue;
        }

        private void FillFrames()
        {
            foreach (var s in _gb.StructArray)
            {
                var vs = DsaS[s];
                s.Type = vs.StructType;
                s.DataOrDataOffset = GetStructDataOrOffset(vs);
                s.FieldCount = (uint) vs.Count;
            }
            foreach (var f in _gb.FieldArray)
            {
                var cpnt = DfaF[f];
                f.LabelIndex = GetLabelIndex(cpnt.Label);
                f.Type = (uint) cpnt.Type;
                if (cpnt is GffStruct)
                {
                    f.DataOrDataOffset = (uint) _gb.StructArray.IndexOf(DsaS[cpnt as GffStruct]);
                }
                else if (cpnt is GffList)
                {
                    f.DataOrDataOffset = GetListDataOrOffset(cpnt as GffList);
                }
                else if (cpnt is GffField)
                {
                    f.DataOrDataOffset = GetFieldDataOrOffset(cpnt as GffField);
                }
                else
                {
                    throw new ComponentException(ErrorLabels.UnknownComponentType);
                }
            }
        }

        private uint GetStructDataOrOffset(IList<GffComponent> vs)
        {
            switch (vs.Count)
            {
                case 0:
                    return uint.MaxValue;
                case 1:
                    return (uint) _gb.FieldArray.IndexOf(DfaF[vs[0]]);
                default:
                {
                    var br = new LatinBinaryWriter(_gb.FieldIndicesArray);
                    var pos = _gb.FieldIndicesArray.Position;
                    foreach (var cpnt in vs)
                    {
                        br.Write(_gb.FieldArray.IndexOf(DfaF[cpnt]));
                    }
                    return (uint) pos;
                }
            }
        }

        private uint GetListDataOrOffset(ICollection<GffComponent> lst)
        {
            var br = new LatinBinaryWriter(_gb.ListIndicesArray);
            var pos = _gb.ListIndicesArray.Position;
            br.Write((uint) lst.Count);
            foreach (var cpnt in lst)
            {
                if (cpnt is GffInListStruct)
                {
                    br.Write(_gb.StructArray.IndexOf(DsaS[cpnt as GffInListStruct]));
                }
                else
                {
                    throw new CompositeException(ErrorLabels.AddWrongStructureClassToList);
                }
            }
            return (uint) pos;
        }

        private uint GetFieldDataOrOffset(GffField f)
        {
            if (GffComponent.IsSimple(f.Type))
            {
                var lB = f.Bytes;
                if (lB.Length < 4)
                {
                    Array.Resize(ref lB, 4);
                }
                return BitConverter.ToUInt32(lB, 0);
            }
            var br = new LatinBinaryWriter(_gb.FieldDataBlock);
            var pos = _gb.FieldDataBlock.Position;
            switch (f.Type)
            {
                case GffType.ResRef:
                    br.Write((byte) f.Bytes.Length);
                    break;
                case GffType.CExoString:
                case GffType.CExoLocString:
                case GffType.Void:
                    br.Write((uint) f.Bytes.Length);
                    break;
            }
            br.Write(f.Bytes);
            return (uint) pos;
        }

        private void CreateFrames(GffComponent cpnt)
        {
            var value = cpnt as GffStruct;
            if (value != null)
            {
                var sf = new GffStructFrame();
                _gb.StructArray.Add(sf);
                DsaS.Add(sf, value);
            }
            if (!(cpnt is GffInListStruct || cpnt is GffRootStruct))
            {
                var ff = new GffFieldFrame();
                _gb.FieldArray.Add(ff);
                DfaF.Add(ff, cpnt);
            }
            if (!GffComponent.IsComposite(cpnt.Type))
            {
                return;
            }
            var cpsit = cpnt as GffComposite;
            if (cpsit == null)
            {
                return;
            }
            foreach (var child in cpsit)
            {
                CreateFrames(child);
            }
        }

        private void WriteData()
        {
            _bw.Seek(GffHeader.Size, SeekOrigin.Begin);
            WriteStructures();
            WriteFields();
            WriteLabels();
            WriteFieldData();
            WriteFieldIndices();
            WriteListIndices();
            _bw.Seek(0, SeekOrigin.Begin);
            WriteHeader();
        }

        private void WriteStructures()
        {
            _gb.Header.StructOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.StructCount = (uint) _gb.StructArray.Count;
            foreach (var sf in _gb.StructArray)
            {
                WriteFrame(sf);
            }
        }

        private void WriteListIndices()
        {
            _gb.Header.ListIndicesOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.ListIndicesCount = (uint) _gb.ListIndicesArray.Length;
            _bw.Write(_gb.ListIndicesArray.ToArray());
        }

        private void WriteFieldData()
        {
            _gb.Header.FieldDataOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.FieldDataCount = (uint) _gb.FieldDataBlock.Length;
            _bw.Write(_gb.FieldDataBlock.ToArray());
        }

        private void WriteFieldIndices()
        {
            _gb.Header.FieldIndicesOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.FieldIndicesCount = (uint) _gb.FieldIndicesArray.Length;
            _bw.Write(_gb.FieldIndicesArray.ToArray());
        }

        private void WriteLabels()
        {
            _gb.Header.LabelOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.LabelCount = (uint) _gb.LabelArray.Count;
            foreach (var lbl in _gb.LabelArray)
            {
                _bw.Write(lbl.PadRight(GffConst.LabelLength, GffConst.LabelPaddingCharacter).ToCharArray());
            }
        }

        private void WriteFields()
        {
            _gb.Header.FieldOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.FieldCount = (uint) _gb.FieldArray.Count;
            foreach (var f in _gb.FieldArray)
            {
                WriteFrame(f);
            }
        }

        private void WriteHeader()
        {
            var cext = _ext.TrimStart('.').PadRight(GffHeader.FileTypeSize, ' ').ToUpper().ToCharArray();
            var vers = GffConst.Version.ToCharArray();
            _bw.Write(cext);
            _bw.Write(vers);

            foreach (var value in _gb.Header.Infos)
            {
                _bw.Write(value);
            }
        }

        private void WriteFrame(GffBasicFrame frm)
        {
            var i = 0;
            while (i < GffBasicFrame.ValueCount)
            {
                _bw.Write(frm.Datas[i++]);
            }
        }
    }
}