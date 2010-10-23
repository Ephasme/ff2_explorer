using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;
using Bioware.Tools;

namespace Bioware.GFF.Composite {
    public abstract class GComponent {
        protected GComponent(string label, GType type) {
            Owner = null;
            Label = label;
            Type = type;
        }
        public string Label { get; set; }
        public GComposite Owner { get; set; }
        public GType Type { get; private set; }
        public bool IsRoot {
            get { return (Owner == null); }
        }
        public static bool IsSimple(GType type) {
            switch (type) {
                case GType.Byte:
                case GType.Char:
                case GType.Word:
                case GType.Short:
                case GType.Dword:
                case GType.Int:
                case GType.Float:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsComposite(GType type) {
            return (type == GType.Struct || type == GType.List);
        }
        public static bool IsLarge(GType type) {
            switch (type) {
                case GType.Dword64:
                case GType.Int64:
                case GType.Double:
                case GType.CExoString:
                case GType.ResRef:
                case GType.CExoLocString:
                case GType.Void:
                    return true;
                default:
                    return false;
            }
        }
    }
    public abstract class GComposite : GComponent, IList<GComponent> {
        protected List<GComponent> Childs;
        protected GComposite(string label, GType type) : base(label, type) {
            Childs = new List<GComponent>();
        }
        protected GComponent SelectComponent(string label) {
            return Childs.Single(item => item.Label == label);
        }

        #region IList<GComponent> Membres
        public int IndexOf(GComponent item) {
            return Childs.IndexOf(item);
        }

        public void Insert(int index, GComponent item) {
            Childs.Insert(index, item);
            item.Owner = this;
        }

        public void RemoveAt(int index) {
            Childs[index].Owner = null;
            Childs.RemoveAt(index);
        }

        public GComponent this[int index] {
            get { return Childs[index]; }
            set { Childs[index] = value; }
        }
        #endregion

        #region ICollection<GComponent> Membres
        public void Add(GComponent item) {
            Childs.Add(item);
            item.Owner = this;
        }

        public void Clear() {
            foreach (var c in Childs) {
                c.Owner = null;
            }
            Childs.Clear();
        }

        public bool Contains(GComponent item) {
            return Childs.Contains(item);
        }

        public void CopyTo(GComponent[] array, int arrayIndex) {
            Childs.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return Childs.Count; }
        }

        public bool IsReadOnly {
            get { return ((ICollection<GComponent>) Childs).IsReadOnly; }
        }

        public bool Remove(GComponent item) {
            if (Childs.Remove(item)) {
                item.Owner = null;
                return true;
            }
            return false;
        }
        #endregion

        #region IEnumerable<GComponent> Membres
        public IEnumerator<GComponent> GetEnumerator() {
            return Childs.GetEnumerator();
        }
        #endregion

        #region IEnumerable Membres
        IEnumerator IEnumerable.GetEnumerator() {
            return Childs.GetEnumerator();
        }
        #endregion

        //public List<GComponent> SelectAll(string label) {
        //    return new List<GComponent>(childs.Where((item) => { return item.Label == label; }));
        //}
    }
}
namespace Bioware.GFF {
    public enum GType : uint {
        Byte,
        Char,
        Word,
        Short,
        Dword,
        Int,
        Dword64,
        Int64,
        Float,
        Double,
        CExoString,
        ResRef,
        CExoLocString,
        Void,
        Struct,
        List,
        Invalid = 4294967295
    }
    public static class GConst {
        public const string VERSION = "V3.2";
        public const char LABEL_PADDING_CHARACTER = '\0';
        public const int LABEL_LENGTH = 16;
    }

    public class GDocument {
        private readonly GBase _gb;
        private readonly GReader _rd;
        private readonly GWriter _wr;
        public GDocument(string path) : this() {
            _rd.Load(path);
        }
        public GDocument(Stream stream) : this() {
            _rd.Load(stream);
        }
        public GDocument() {
            _gb = new GBase();
            _wr = new GWriter(_gb);
            _rd = new GReader(_gb);
        }
        public GRootStruct RootStruct {
            get { return _rd.RootStruct; }
        }
        public void Save(GRootStruct root, string path) {
            _wr.Save(root, path);
        }
        public Stream Save(GRootStruct root) {
            return _wr.Save(root);
        }
        public void Save(string path) {
            _wr.Save(_rd.RootStruct, path);
        }
        public Stream Save() {
            return _wr.Save(_rd.RootStruct);
        }
        public void Load(string path) {
            _rd.Load(path);
        }
        public void Load(Stream stream) {
            _rd.Load(stream);
        }
    }

    internal class GFactory {
        private readonly GBase _gbase;
        public GFactory(GBase gbase) {
            _gbase = gbase;
        }

        private GComponent CreateComponent(uint fIndex) {
            MemoryStream dataStream;
            var f = _gbase.FieldArray[(int)fIndex];
            var fLblId = f.LabelIndex;
            var fLbl = _gbase.LabelArray[(int) fLblId];
//            if (fLbl == null) {
//                throw new NotImplementedException();
//            }
            var fType = (GType) f.Type;

            if (GComponent.IsComposite(fType)) {
                if (fType == GType.Struct) {
                    // On va chercher la frame qui lui correspond.
                    var sIndex = f.DataOrDataOffset;
                    var s = _gbase.StructArray[(int) sIndex];
                    var sType = s.Type;
                    var vs = new GInFieldStruct(fLbl, sType);

                    PopulateStruct(vs, GetStructFieldIndices(s));

                    return vs;
                }
                if (fType == GType.List) {
                    var br = new BinaryReader(_gbase.ListIndicesArray)
                                      {Stream = {Position = f.DataOrDataOffset}};
                    var size = (int) br.ReadUInt32();
                    var vl = new GList(fLbl);
                    var structIdList = br.GetUInt32List(size);
                    foreach (var structId in structIdList) {
                        var sFrame = _gbase.StructArray[(int) structId];
                        var vls = new GInListStruct(sFrame.Type);
                        PopulateStruct(vls, GetStructFieldIndices(sFrame));
                        vl.Add(vls);
                    }
                    return vl;
                }
                throw new CompositeException(Error.UnknownCompositeType);
            }
            if (GComponent.IsSimple(fType)) {
                dataStream = new MemoryStream(BitConverter.GetBytes(f.DataOrDataOffset));
            } else if (GComponent.IsLarge(fType)) {
                var br = new BinaryReader(_gbase.FieldDataBlock);
                var pos = br.Stream.Position;
                br.Stream.Position = f.DataOrDataOffset;
                int size;

                #region Switch de détermination de la taille.
                switch (fType) {
                    case GType.Dword64:
                        size = sizeof (UInt64);
                        break;
                    case GType.Int64:
                        size = sizeof (Int64);
                        break;
                    case GType.Double:
                        size = sizeof (double);
                        break;
                    case GType.ResRef:
                        size = br.ReadByte();
                        break;
                    case GType.CExoString:
                    case GType.CExoLocString:
                    case GType.Void:
                        size = (int) br.ReadUInt32();
                        break;
                    default:
                        throw new FieldException(Error.UnknownLargeFieldType);
                }
                #endregion

                dataStream = new MemoryStream(br.ReadBytes(size));
                br.Stream.Position = pos;
            } else {
                throw new FieldException(Error.UnknownFieldType);
            }
            return new GField(fLbl, fType, dataStream.ToArray());
        }

        private void PopulateStruct(ICollection<GComponent> vs, IEnumerable<uint> fieldIndexes) {
            if (vs == null) {
                throw new ArgumentNullException("vs");
            }
            foreach (var cpnt in fieldIndexes.Select(fId => CreateComponent(fId))) {
                vs.Add(cpnt);
            }
        }

        private IEnumerable<uint> GetStructFieldIndices(GStructFrame sFrame) {
            var fieldIndices = new List<uint>();
            if (sFrame.FieldCount > 1) {
                var br = new BinaryReader(_gbase.FieldIndicesArray);
                long pos = br.Stream.Position;
                br.Stream.Position = sFrame.DataOrDataOffset;
                fieldIndices = br.GetUInt32List((int) sFrame.FieldCount);
                br.Stream.Position = pos;
            } else if (sFrame.FieldCount == 1) {
                fieldIndices.Add(sFrame.DataOrDataOffset);
            }
            return fieldIndices;
        }

        public GRootStruct CreateRoot() {
            var rootFrame = _gbase.StructArray[(int) GRootStruct.ROOT_INDEX];
            var root = new GRootStruct(_gbase.Header.Type);
            PopulateStruct(root, GetStructFieldIndices(rootFrame));
            return root;
        }
    }
    internal class GHeader {
        #region Constantes du tableau des données de l'en-tête.
        public const int STRUCT_OFFSET = 0;
        public const int STRUCT_COUNT = 1;
        public const int FIELD_OFFSET = 2;
        public const int FIELD_COUNT = 3;
        public const int LABEL_OFFSET = 4;
        public const int LABEL_COUNT = 5;
        public const int FIELD_DATA_OFFSET = 6;
        public const int FIELD_DATA_COUNT = 7;
        public const int FIELD_INDICES_OFFSET = 8;
        public const int FIELD_INDICES_COUNT = 9;
        public const int LIST_INDICES_OFFSET = 10;
        public const int LIST_INDICES_COUNT = 11;
        #endregion

        #region Constantes d'information sur la structure.
        public const int FILE_TYPE_SIZE = 4;
        public const int FILE_VERSION_SIZE = 4;
        public const int DWORD_TABLE_SIZE = 12;
        public const int SIZE = DWORD_TABLE_SIZE*sizeof (uint) + (FILE_TYPE_SIZE + FILE_VERSION_SIZE);
        #endregion

        #region Variables contenant les données de l'en-tête.
        public uint[] Infos { private set; get; }
        public string Type { private set; get; }
        public string Version { private set; get; }
        #endregion

        #region Propriétés d'accès à l'en-tête
        /// <summary>
        /// Accès au type de fichier.
        /// </summary>
        public string FileType {
            get { return Type; }
            set { Type = value; }
        }
        /// <summary>
        /// Accès à la version de la structure GFF (toujours V3.2).
        /// </summary>
        public string FileVersion {
            get { return Version; }
            set { Version = value; }
        }
        /// <summary>
        /// Accès à la position de la première structure.
        /// </summary>
        public uint StructOffset {
            get { return Infos[STRUCT_OFFSET]; }
            set { Infos[STRUCT_OFFSET] = value; }
        }
        /// <summary>
        /// Accès au nombre de structures.
        /// </summary>
        public uint StructCount {
            get { return Infos[STRUCT_COUNT]; }
            set { Infos[STRUCT_COUNT] = value; }
        }
        /// <summary>
        /// Accès à la position du premier champ.
        /// </summary>
        public uint FieldOffset {
            get { return Infos[FIELD_OFFSET]; }
            set { Infos[FIELD_OFFSET] = value; }
        }
        /// <summary>
        /// Accès au nombre de champ.
        /// </summary>
        public uint FieldCount {
            get { return Infos[FIELD_COUNT]; }
            set { Infos[FIELD_COUNT] = value; }
        }
        /// <summary>
        /// Accès à la position du premier champ.
        /// </summary>
        public uint LabelOffset {
            get { return Infos[LABEL_OFFSET]; }
            set { Infos[LABEL_OFFSET] = value; }
        }
        /// <summary>
        /// Accès au nombre de champ.
        /// </summary>
        public uint LabelCount {
            get { return Infos[LABEL_COUNT]; }
            set { Infos[LABEL_COUNT] = value; }
        }
        /// <summary>
        /// Accès à la position du premier champ complexe.
        /// </summary>
        public uint FieldDataOffset {
            get { return Infos[FIELD_DATA_OFFSET]; }
            set { Infos[FIELD_DATA_OFFSET] = value; }
        }
        /// <summary>
        /// Accès au nombre de champs complexes.
        /// </summary>
        public uint FieldDataCount {
            get { return Infos[FIELD_DATA_COUNT]; }
            set { Infos[FIELD_DATA_COUNT] = value; }
        }
        /// <summary>
        /// Accès à la position du premier indice de champ.
        /// </summary>
        public uint FieldIndicesOffset {
            get { return Infos[FIELD_INDICES_OFFSET]; }
            set { Infos[FIELD_INDICES_OFFSET] = value; }
        }
        /// <summary>
        /// Accès au nombre d'indice de champ.
        /// </summary>
        public uint FieldIndicesCount {
            get { return Infos[FIELD_INDICES_COUNT]; }
            set { Infos[FIELD_INDICES_COUNT] = value; }
        }
        /// <summary>
        /// Accès à la position de la première liste d'indices.
        /// </summary>
        public uint ListIndicesOffset {
            get { return Infos[LIST_INDICES_OFFSET]; }
            set { Infos[LIST_INDICES_OFFSET] = value; }
        }
        /// <summary>
        /// Accès à la liste des indices de liste.
        /// </summary>
        public uint ListIndicesCount {
            get { return Infos[LIST_INDICES_COUNT]; }
            set { Infos[LIST_INDICES_COUNT] = value; }
        }
        #endregion

        public GHeader() {
            Infos = new uint[DWORD_TABLE_SIZE];
        }

        public void Load(BinaryReader br) {
            long pos = br.Stream.Position;
            br.Stream.Position = 0;
            Type = (new String(br.ReadChars(FILE_TYPE_SIZE))).Trim();
            Version = (new String(br.ReadChars(FILE_VERSION_SIZE))).Trim();

            Queue<uint> q = br.GetUInt32Queue(DWORD_TABLE_SIZE);
            int i = 0;
            while (q.Count > 0) {
                Infos[i++] = q.Dequeue();
            }
            br.Stream.Position = pos;
        }
    }
    internal class GBase {
        public GBase() {
            Header = new GHeader();
            StructArray = new List<GStructFrame>();
            FieldArray = new List<GFieldFrame>();
            LabelArray = new List<string>();
            FieldDataBlock = new MemoryStream();
            FieldIndicesArray = new MemoryStream();
            ListIndicesArray = new MemoryStream();
        }
        public GHeader Header { get; set; }
        public List<GStructFrame> StructArray { get; private set; }
        public List<GFieldFrame> FieldArray { get; private set; }
        public List<string> LabelArray { get; private set; }
        public MemoryStream FieldDataBlock { get; set; }
        public MemoryStream FieldIndicesArray { get; set; }
        public MemoryStream ListIndicesArray { get; set; }
    }
    internal class GReader {
         readonly GFactory _fac;
         readonly GBase _gb;
         BinaryReader _br;
         GRootStruct _root;

        public GReader(GBase gbase) {
            _gb = gbase;
            _fac = new GFactory(_gb);
        }
        public GReader(string path) {
            Load(path);
        }
        public GReader(Stream stream) {
            Load(stream);
        }
        public GRootStruct RootStruct {
            get { return _root ?? (_root = _fac.CreateRoot()); }
        }
        public void Load(string path) {
            if (File.Exists(path)) {
                Load(File.OpenRead(path));
            }
        }
        public void Load(Stream stream) {
            _br = new BinaryReader(stream);
            _gb.Header.Load(_br);
            LoadStructures();
            LoadFields();
            LoadLabels();
            LoadFieldDatas();
            LoadFieldIndicesArray();
            LoadListIndicesArray();
            _br.Close();
        }
        private void LoadStructures() {
            long pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.StructOffset;
            for (int i = 0; i < _gb.Header.StructCount; i++) {
                Queue<uint> q = _br.GetUInt32Queue(GBasicFrame.VALUE_COUNT);
                var sf = new GStructFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                _gb.StructArray.Add(sf);
            }
            _br.Stream.Position = pos;
        }
        private void LoadFields() {
            long pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.FieldOffset;
            for (int i = 0; i < _gb.Header.FieldCount; i++) {
                Queue<uint> q = _br.GetUInt32Queue(GBasicFrame.VALUE_COUNT);
                var ff = new GFieldFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                _gb.FieldArray.Add(ff);
            }
            _br.Stream.Position = pos;
        }
        private void LoadLabels() {
            long pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.LabelOffset;
            for (int i = 0; i < _gb.Header.LabelCount; i++) {
                var lbl = new string(_br.ReadChars(GConst.LABEL_LENGTH));
                lbl = lbl.TrimEnd(GConst.LABEL_PADDING_CHARACTER);
                _gb.LabelArray.Add(lbl);
            }
            _br.Stream.Position = pos;
        }
        private void LoadFieldDatas() {
            long pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.FieldDataOffset;
            _gb.FieldDataBlock = new MemoryStream(_br.ReadBytes((int) (_gb.Header.FieldDataCount)));
            _br.Stream.Position = pos;
        }
        private void LoadFieldIndicesArray() {
            long pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.FieldIndicesOffset;
            _gb.FieldIndicesArray = new MemoryStream(_br.ReadBytes((int) (_gb.Header.FieldIndicesCount)));
            _br.Stream.Position = pos;
        }
        private void LoadListIndicesArray() {
            long pos = _br.Stream.Position;
            _br.Stream.Position = _gb.Header.ListIndicesOffset;
            _gb.ListIndicesArray = new MemoryStream(_br.ReadBytes((int) (_gb.Header.ListIndicesCount)));
            _br.Stream.Position = pos;
        }
    }
    internal class GWriter {
        private readonly GBase _gb;
        private BinaryWriter _bw;

        public DoubleDictionary<GFieldFrame, GComponent> DfaF;
        public DoubleDictionary<GStructFrame, GStruct> DsaS;
        private string _ext;
        private GRootStruct _rt;

        public GWriter(GBase gbase) {
            _gb = gbase;
        }

        public MemoryStream Save(GRootStruct root) {
            _bw = new BinaryWriter(new MemoryStream());
            _rt = root;
            _ext = root.Extention;
            DfaF = new DoubleDictionary<GFieldFrame, GComponent>();
            DsaS = new DoubleDictionary<GStructFrame, GStruct>();

            CreateFrames(_rt);
            FillFrames();
            WriteData();

            Stream res = _bw.BaseStream;
            _bw.Close();
            return (MemoryStream) res;
        }
        public void Save(GRootStruct root, string path) {
            File.WriteAllBytes(path, Save(root).ToArray());
        }

        private uint GetLabelIndex(string lbl) {
            if (lbl != null) {
                if (!_gb.LabelArray.Contains(lbl)) {
                    _gb.LabelArray.Add(lbl);
                    return (uint) _gb.LabelArray.Count - 1;
                }
                return (uint) _gb.LabelArray.IndexOf(lbl);
            }
            return uint.MaxValue;
        }
        private void FillFrames() {
            foreach (var s in _gb.StructArray) {
                var vs = DsaS[s];
                s.Type = vs.StructType;
                s.DataOrDataOffset = GetStructDataOrOffset(vs);
                s.FieldCount = (uint) vs.Count;
            }
            foreach (var f in _gb.FieldArray) {
                var cpnt = DfaF[f];
                f.LabelIndex = GetLabelIndex(cpnt.Label);
                f.Type = (uint) cpnt.Type;
                if (cpnt is GStruct) {
                    f.DataOrDataOffset = (uint) _gb.StructArray.IndexOf(DsaS[cpnt as GStruct]);
                } else if (cpnt is GList) {
                    f.DataOrDataOffset = GetListDataOrOffset(cpnt as GList);
                } else if (cpnt is GField) {
                    f.DataOrDataOffset = GetFieldDataOrOffset(cpnt as GField);
                } else {
                    throw new ComponentException(Error.UnknownComponentType);
                }
            }
        }
        private uint GetStructDataOrOffset(IList<GComponent> vs) {
            switch (vs.Count) {
                case 0:
                    return uint.MaxValue;
                case 1:
                    return (uint) _gb.FieldArray.IndexOf(DfaF[vs[0]]);
                default: {
                    var br = new BinaryWriter(_gb.FieldIndicesArray);
                    var pos = _gb.FieldIndicesArray.Position;
                    foreach (var cpnt in vs) {
                        br.Write(_gb.FieldArray.IndexOf(DfaF[cpnt]));
                    }
                    return (uint) pos;
                }
            }
        }
        private uint GetListDataOrOffset(ICollection<GComponent> lst) {
            var br = new BinaryWriter(_gb.ListIndicesArray);
            var pos = _gb.ListIndicesArray.Position;
            br.Write((uint) lst.Count);
            foreach (var cpnt in lst) {
                if (cpnt is GInListStruct) {
                    br.Write(_gb.StructArray.IndexOf(DsaS[cpnt as GInListStruct]));
                } else {
                    throw new CompositeException(Error.AddWrongStructureClassToList);
                }
            }
            return (uint) pos;
        }
        private uint GetFieldDataOrOffset(GField f) {
            if (GComponent.IsSimple(f.Type)) {
                byte[] lB = f.Bytes;
                if (lB.Length < 4) {
                    Array.Resize(ref lB, 4);
                }
                return BitConverter.ToUInt32(lB, 0);
            }
            var br = new BinaryWriter(_gb.FieldDataBlock);
            var pos = _gb.FieldDataBlock.Position;
            switch (f.Type) {
                case GType.ResRef:
                    br.Write((byte) f.Bytes.Length);
                    break;
                case GType.CExoString:
                case GType.CExoLocString:
                case GType.Void:
                    br.Write((uint) f.Bytes.Length);
                    break;
            }
            br.Write(f.Bytes);
            return (uint) pos;
        }

        private void CreateFrames(GComponent cpnt) {
            if (cpnt is GStruct) {
                var sf = new GStructFrame();
                _gb.StructArray.Add(sf);
                DsaS.Add(sf, (GStruct) cpnt);
            }
            if (!(cpnt is GInListStruct || cpnt is GRootStruct)) {
                var ff = new GFieldFrame();
                _gb.FieldArray.Add(ff);
                DfaF.Add(ff, cpnt);
            }
            if (!GComponent.IsComposite(cpnt.Type)) {
                return;
            }
            var cpsit = cpnt as GComposite;
            if (cpsit == null) {
                return;
            }
            foreach (var child in cpsit) {
                CreateFrames(child);
            }
        }
        private void WriteData() {
            _bw.Seek(GHeader.SIZE, SeekOrigin.Begin);
            WriteStructures();
            WriteFields();
            WriteLabels();
            WriteFieldData();
            WriteFieldIndices();
            WriteListIndices();
            _bw.Seek(0, SeekOrigin.Begin);
            WriteHeader();
        }
        private void WriteStructures() {
            _gb.Header.StructOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.StructCount = (uint) _gb.StructArray.Count;
            foreach (GStructFrame sf in _gb.StructArray) {
                WriteFrame(sf);
            }
        }
        private void WriteListIndices() {
            _gb.Header.ListIndicesOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.ListIndicesCount = (uint) _gb.ListIndicesArray.Length;
            _bw.Write(_gb.ListIndicesArray.ToArray());
        }
        private void WriteFieldData() {
            _gb.Header.FieldDataOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.FieldDataCount = (uint) _gb.FieldDataBlock.Length;
            _bw.Write(_gb.FieldDataBlock.ToArray());
        }
        private void WriteFieldIndices() {
            _gb.Header.FieldIndicesOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.FieldIndicesCount = (uint) _gb.FieldIndicesArray.Length;
            _bw.Write(_gb.FieldIndicesArray.ToArray());
        }
        private void WriteLabels() {
            _gb.Header.LabelOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.LabelCount = (uint) _gb.LabelArray.Count;
            foreach (string lbl in _gb.LabelArray) {
                _bw.Write(lbl.PadRight(GConst.LABEL_LENGTH, GConst.LABEL_PADDING_CHARACTER).ToCharArray());
            }
        }
        private void WriteFields() {
            _gb.Header.FieldOffset = (uint) _bw.BaseStream.Position;
            _gb.Header.FieldCount = (uint) _gb.FieldArray.Count;
            foreach (GFieldFrame f in _gb.FieldArray) {
                WriteFrame(f);
            }
        }
        private void WriteHeader() {
            char[] cext = _ext.TrimStart('.').PadRight(GHeader.FILE_TYPE_SIZE, ' ').ToUpper().ToCharArray();
            char[] vers = (GConst.VERSION).ToCharArray();
            _bw.Write(cext);
            _bw.Write(vers);

            foreach (uint value in _gb.Header.Infos) {
                _bw.Write(value);
            }
        }
        private void WriteFrame(GBasicFrame frm) {
            int i = 0;
            while (i < GBasicFrame.VALUE_COUNT) {
                _bw.Write(frm.Datas[i++]);
            }
        }
    }
}