using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;

namespace Bioware.GFF
{
    internal class GffFactory
    {
        private readonly GffBase _gbase;

        public GffFactory(GffBase gbase)
        {
            _gbase = gbase;
        }

        private GffComponent CreateComponent(uint fIndex)
        {
            MemoryStream dataStream;
            var f = _gbase.FieldArray[(int) fIndex];
            var fLblId = f.LabelIndex;
            var fLbl = _gbase.LabelArray[(int) fLblId];
//            if (fLbl == null) {
//                throw new NotImplementedException();
//            }
            var fType = (GffType) f.Type;

            if (GffComponent.IsComposite(fType))
            {
                if (fType == GffType.Struct)
                {
                    // On va chercher la frame qui lui correspond.
                    var sIndex = f.DataOrDataOffset;
                    var s = _gbase.StructArray[(int) sIndex];
                    var sType = s.Type;
                    var vs = new GffInFieldStruct(fLbl, sType);

                    PopulateStruct(vs, GetStructFieldIndices(s));

                    return vs;
                }
                if (fType == GffType.List)
                {
                    var br = new LatinBinaryReader(_gbase.ListIndicesArray)
                    {Stream = {Position = f.DataOrDataOffset}};
                    var size = (int) br.ReadUInt32();
                    var vl = new GffList(fLbl);
                    var structIdList = br.GetUInt32List(size);
                    foreach (var structId in structIdList)
                    {
                        var sFrame = _gbase.StructArray[(int) structId];
                        var vls = new GffInListStruct(sFrame.Type);
                        PopulateStruct(vls, GetStructFieldIndices(sFrame));
                        vl.Add(vls);
                    }
                    return vl;
                }
                throw new CompositeException(ErrorLabels.UnknownCompositeType);
            }
            if (GffComponent.IsSimple(fType))
            {
                dataStream = new MemoryStream(BitConverter.GetBytes(f.DataOrDataOffset));
            }
            else if (GffComponent.IsLarge(fType))
            {
                var br = new LatinBinaryReader(_gbase.FieldDataBlock);
                var pos = br.Stream.Position;
                br.Stream.Position = f.DataOrDataOffset;
                var size = 0;

                #region Switch de détermination de la taille.

                switch (fType)
                {
                    case GffType.Dword64:
                        size = 8;
                        break;
                    case GffType.Int64:
                        size = sizeof (long);
                        break;
                    case GffType.Double:
                        size = sizeof (double);
                        break;
                    case GffType.ResRef:
                        size = br.ReadByte();
                        break;
                    case GffType.CExoString:
                    case GffType.CExoLocString:
                    case GffType.Void:
                        size = (int) br.ReadUInt32();
                        break;
                    case GffType.Byte:
                        break;
                    case GffType.Char:
                        break;
                    case GffType.Word:
                        break;
                    case GffType.Short:
                        break;
                    case GffType.Dword:
                        break;
                    case GffType.Int:
                        break;
                    case GffType.Float:
                        break;
                    case GffType.Struct:
                        break;
                    case GffType.List:
                        break;
                    case GffType.Invalid:
                        break;
                    default:
                        throw new FieldException(ErrorLabels.UnknownLargeFieldType);
                }

                #endregion

                dataStream = new MemoryStream(br.ReadBytes(size));
                br.Stream.Position = pos;
            }
            else
            {
                throw new FieldException(ErrorLabels.UnknownFieldType);
            }
            return new GffField(fLbl, fType, dataStream.ToArray());
        }

        private void PopulateStruct(ICollection<GffComponent> vs, IEnumerable<uint> fieldIndexes)
        {
            if (vs == null)
            {
                throw new ArgumentNullException(nameof(vs));
            }
            foreach (var cpnt in fieldIndexes.Select(CreateComponent))
            {
                vs.Add(cpnt);
            }
        }

        private IEnumerable<uint> GetStructFieldIndices(GffStructFrame sFrame)
        {
            var fieldIndices = new List<uint>();
            if (sFrame.FieldCount > 1)
            {
                var br = new LatinBinaryReader(_gbase.FieldIndicesArray);
                var pos = br.Stream.Position;
                br.Stream.Position = sFrame.DataOrDataOffset;
                fieldIndices = br.GetUInt32List((int) sFrame.FieldCount);
                br.Stream.Position = pos;
            }
            else if (sFrame.FieldCount == 1)
            {
                fieldIndices.Add(sFrame.DataOrDataOffset);
            }
            return fieldIndices;
        }

        public GffRootStruct CreateRoot()
        {
            var rootFrame = _gbase.StructArray[(int) GffRootStruct.RootIndex];
            var root = new GffRootStruct(_gbase.Header.Type);
            PopulateStruct(root, GetStructFieldIndices(rootFrame));
            return root;
        }
    }
}