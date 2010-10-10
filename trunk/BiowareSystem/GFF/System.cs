using System;
using System.Linq;
using System.Collections.Generic;
using System.IO;
using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;
using Bioware.Tools;
using System.Collections;

namespace Bioware.GFF.Composite {
    public abstract class GComponent {
        public string Label { get; set; }
        public GComposite Owner { get; set; }
        public GType Type { get; private set; }
        public static bool IsSimple(GType type) {
            switch (type) {
                case GType.BYTE:
                case GType.CHAR:
                case GType.WORD:
                case GType.SHORT:
                case GType.DWORD:
                case GType.INT:
                case GType.FLOAT:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsComposite(GType type) {
            return (type == GType.STRUCT || type == GType.LIST);
        }
        public static bool IsLarge(GType type) {
            switch (type) {
                case GType.DWORD64:
                case GType.INT64:
                case GType.DOUBLE:
                case GType.CEXOSTRING:
                case GType.RESREF:
                case GType.CEXOLOCSTRING:
                case GType.VOID:
                    return true;
                default:
                    return false;
            }
        }
        public bool IsRoot {
            get {
                return (Owner == null);
            }
        }
        public GComponent(string label, GType type) {
            Owner = null;
            Label = label;
            Type = type;
        }
    }
    public abstract class GComposite : GComponent, IList<GComponent> {
        protected List<GComponent> childs;
        public GComposite(string label, GType type)
            : base(label, type) {
            childs = new List<GComponent>();
        }
        protected GComponent SelectComponent(string label) {
            return childs.Single((item) => { return item.Label == label; });
        }
        //public List<GComponent> SelectAll(string label) {
        //    return new List<GComponent>(childs.Where((item) => { return item.Label == label; }));
        //}
        #region IList<GComponent> Membres

        public int IndexOf(GComponent item) {
            return childs.IndexOf(item);
        }

        public void Insert(int index, GComponent item) {
            childs.Insert(index, item);
            item.Owner = this;
        }

        public void RemoveAt(int index) {
            childs[index].Owner = null;
            childs.RemoveAt(index);
        }

        public GComponent this[int index] {
            get {
                return childs[index];
            }
            set {
                childs[index] = value;
            }
        }

        #endregion
        #region ICollection<GComponent> Membres

        public void Add(GComponent item) {
            childs.Add(item);
            item.Owner = this;
        }

        public void Clear() {
            foreach (GComponent c in childs) {
                c.Owner = null;
            }
            childs.Clear();
        }

        public bool Contains(GComponent item) {
            return childs.Contains(item);
        }

        public void CopyTo(GComponent[] array, int arrayIndex) {
            childs.CopyTo(array, arrayIndex);
        }

        public int Count {
            get { return childs.Count; }
        }

        public bool IsReadOnly {
            get { return ((ICollection<GComponent>)childs).IsReadOnly; }
        }

        public bool Remove(GComponent item) {
            if (childs.Remove(item)) {
                item.Owner = null;
                return true;
            }
            return false;
        }

        #endregion
        #region IEnumerable<GComponent> Membres

        public IEnumerator<GComponent> GetEnumerator() {
            return childs.GetEnumerator();
        }

        #endregion
        #region IEnumerable Membres

        IEnumerator IEnumerable.GetEnumerator() {
            return childs.GetEnumerator();
        }

        #endregion
    }
}
namespace Bioware.GFF {
    public enum GType : uint {
        BYTE, CHAR, WORD, SHORT, DWORD, INT, DWORD64, INT64,
        FLOAT, DOUBLE, CEXOSTRING, RESREF, CEXOLOCSTRING,
        VOID, STRUCT, LIST, INVALID = 4294967295
    }
    public static class GConst {
        public const string VERSION = "V3.2";
        public const char LABEL_PADDING_CHARACTER = '\0';
        public const int LABEL_LENGTH = 16;
    }

    public class GDocument {
        GBase gb;
        GWriter wr;
        GReader rd;
        public GDocument(string path)
            : this() {
            rd.Load(path);
        }
        public GDocument(Stream stream)
            : this() {
            rd.Load(stream);
        }
        public GDocument() {
            gb = new GBase();
            wr = new GWriter(gb);
            rd = new GReader(gb);
        }
        public void Save(GRootStruct root, string path) {
            wr.Save(root, path);
        }
        public Stream Save(GRootStruct root) {
            return wr.Save(root);
        }
        public void Save(string path) {
            wr.Save(rd.RootStruct, path);
        }
        public Stream Save() {
            return wr.Save(rd.RootStruct);
        }
        public void Load(string path) {
            rd.Load(path);
        }
        public void Load(Stream stream) {
            rd.Load(stream);
        }
        public GRootStruct RootStruct {
            get {
                return rd.RootStruct;
            }
        }
    }

    class GFactory {
        GBase gbase;
        public GFactory(GBase gbase) {
            this.gbase = gbase;
        }

        private GComponent CreateComponent(uint f_index) {

            GFieldFrame f = gbase.FieldArray[(int)f_index];
            uint f_lbl_id = f.LabelIndex;
            string f_lbl = gbase.LabelArray[(int)f_lbl_id];
            GType f_type = (GType)f.Type;

            if (GComponent.IsComposite(f_type)) {
                if (f_type == GType.STRUCT) {

                    // On va chercher la frame qui lui correspond.
                    uint s_index = f.DataOrDataOffset;
                    GStructFrame s = gbase.StructArray[(int)s_index];
                    uint s_type = s.Type;
                    GInFieldStruct vs = new GInFieldStruct(f_lbl, s_type);

                    PopulateStruct(vs, GetStructFieldIndices(s));

                    return vs;

                } else if (f_type == GType.LIST) {
                    BinaryReader br = new BinaryReader(gbase.ListIndicesArray);
                    long pos = br.Stream.Position;
                    br.Stream.Position = f.DataOrDataOffset;
                    int size = (int)br.ReadUInt32();
                    GList vl = new GList(f_lbl);
                    List<uint> struct_id_list = br.GetUInt32List(size);
                    foreach (uint struct_id in struct_id_list) {
                        GStructFrame s_frame = gbase.StructArray[(int)struct_id];
                        GInListStruct vls = new GInListStruct(s_frame.Type);
                        PopulateStruct(vls, GetStructFieldIndices(s_frame));
                        vl.Add(vls);
                    }
                    return vl;
                } else {
                    throw new CompositeException(Error.UNKNOWN_COMPOSITE_TYPE);
                }
            } else {
                MemoryStream data_stream;
                if (GComponent.IsSimple(f_type)) {
                    data_stream = new MemoryStream(BitConverter.GetBytes(f.DataOrDataOffset));
                } else if (GComponent.IsLarge(f_type)) {
                    BinaryReader br = new BinaryReader(gbase.FieldDataBlock);
                    long pos = br.Stream.Position;
                    br.Stream.Position = f.DataOrDataOffset;
                    int size = 0;
                    #region Switch de détermination de la taille.
                    switch (f_type) {
                        case GType.DWORD64:
                            size = sizeof(UInt64);
                            break;
                        case GType.INT64:
                            size = sizeof(Int64);
                            break;
                        case GType.DOUBLE:
                            size = sizeof(double);
                            break;
                        case GType.RESREF:
                            size = (int)br.ReadByte();
                            break;
                        case GType.CEXOSTRING:
                        case GType.CEXOLOCSTRING:
                        case GType.VOID:
                            size = (int)br.ReadUInt32();
                            break;
                        default:
                            throw new FieldException(Error.UNKNOWN_LARGE_FIELD_TYPE);
                    }
                    #endregion
                    data_stream = new MemoryStream(br.ReadBytes(size));
                    br.Stream.Position = pos;
                } else {
                    throw new FieldException(Error.UNKNOWN_FIELD_TYPE);
                }
                return new GField(f_lbl, f_type, data_stream.ToArray());
            }
            throw new ComponentException(Error.UNKNOWN_COMPONENT_TYPE);
        }

        private void PopulateStruct(GInFieldStruct vs, List<uint> FieldIndexes) {
            foreach (uint f_id in FieldIndexes) {
                GComponent cpnt = CreateComponent(f_id);
                vs.Add(cpnt);
            }
        }

        private List<uint> GetStructFieldIndices(GStructFrame s_frame) {
            List<uint> FieldIndices = new List<uint>();
            if (s_frame.FieldCount > 1) {
                BinaryReader br = new BinaryReader(gbase.FieldIndicesArray);
                long pos = br.Stream.Position;
                br.Stream.Position = s_frame.DataOrDataOffset;
                FieldIndices = br.GetUInt32List((int)s_frame.FieldCount);
                br.Stream.Position = pos;
            } else if (s_frame.FieldCount == 1) {
                FieldIndices.Add(s_frame.DataOrDataOffset);
            }
            return FieldIndices;
        }

        public GRootStruct CreateRoot() {
            GStructFrame root_frame = gbase.StructArray[(int)GRootStruct.ROOT_INDEX];
            GRootStruct root = new GRootStruct(gbase.Header.Type);
            PopulateStruct(root, GetStructFieldIndices(root_frame));
            return root;
        }
    }
    class GHeader {
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
        public const int SIZE = DWORD_TABLE_SIZE * sizeof(uint) + (FILE_TYPE_SIZE + FILE_VERSION_SIZE);
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
            get {
                return Type;
            }
            set {
                Type = value;
            }
        }
        /// <summary>
        /// Accès à la version de la structure GFF (toujours V3.2).
        /// </summary>
        public string FileVersion {
            get {
                return Version;
            }
            set {
                Version = value;

            }
        }
        /// <summary>
        /// Accès à la position de la première structure.
        /// </summary>
        public uint StructOffset {
            get {
                return Infos[STRUCT_OFFSET];
            }
            set {
                Infos[STRUCT_OFFSET] = value;
            }
        }
        /// <summary>
        /// Accès au nombre de structures.
        /// </summary>
        public uint StructCount {
            get {
                return Infos[STRUCT_COUNT];
            }
            set {
                Infos[STRUCT_COUNT] = value;
            }
        }
        /// <summary>
        /// Accès à la position du premier champ.
        /// </summary>
        public uint FieldOffset {
            get {
                return Infos[FIELD_OFFSET];
            }
            set {
                Infos[FIELD_OFFSET] = value;
            }
        }
        /// <summary>
        /// Accès au nombre de champ.
        /// </summary>
        public uint FieldCount {
            get {
                return Infos[FIELD_COUNT];
            }
            set {
                Infos[FIELD_COUNT] = value;
            }
        }
        /// <summary>
        /// Accès à la position du premier champ.
        /// </summary>
        public uint LabelOffset {
            get {
                return Infos[LABEL_OFFSET];
            }
            set {
                Infos[LABEL_OFFSET] = value;
            }
        }
        /// <summary>
        /// Accès au nombre de champ.
        /// </summary>
        public uint LabelCount {
            get {
                return Infos[LABEL_COUNT];
            }
            set {
                Infos[LABEL_COUNT] = value;
            }
        }
        /// <summary>
        /// Accès à la position du premier champ complexe.
        /// </summary>
        public uint FieldDataOffset {
            get {
                return Infos[FIELD_DATA_OFFSET];
            }
            set {
                Infos[FIELD_DATA_OFFSET] = value;

            }
        }
        /// <summary>
        /// Accès au nombre de champs complexes.
        /// </summary>
        public uint FieldDataCount {
            get {
                return Infos[FIELD_DATA_COUNT];
            }
            set {
                Infos[FIELD_DATA_COUNT] = value;

            }
        }
        /// <summary>
        /// Accès à la position du premier indice de champ.
        /// </summary>
        public uint FieldIndicesOffset {
            get {
                return Infos[FIELD_INDICES_OFFSET];
            }
            set {
                Infos[FIELD_INDICES_OFFSET] = value;
            }
        }
        /// <summary>
        /// Accès au nombre d'indice de champ.
        /// </summary>
        public uint FieldIndicesCount {
            get {
                return Infos[FIELD_INDICES_COUNT];
            }
            set {
                Infos[FIELD_INDICES_COUNT] = value;
            }
        }
        /// <summary>
        /// Accès à la position de la première liste d'indices.
        /// </summary>
        public uint ListIndicesOffset {
            get {
                return Infos[LIST_INDICES_OFFSET];
            }
            set {
                Infos[LIST_INDICES_OFFSET] = value;
            }
        }
        /// <summary>
        /// Accès à la liste des indices de liste.
        /// </summary>
        public uint ListIndicesCount {
            get {
                return Infos[LIST_INDICES_COUNT];
            }
            set {
                Infos[LIST_INDICES_COUNT] = value;
            }
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
    class GBase {
        public GHeader Header { get; set; }
        public List<GStructFrame> StructArray { get; private set; }
        public List<GFieldFrame> FieldArray { get; private set; }
        public List<string> LabelArray { get; private set; }
        public MemoryStream FieldDataBlock { get; set; }
        public MemoryStream FieldIndicesArray { get; set; }
        public MemoryStream ListIndicesArray { get; set; }
        public GBase() {
            Header = new GHeader();
            StructArray = new List<GStructFrame>();
            FieldArray = new List<GFieldFrame>();
            LabelArray = new List<string>();
            FieldDataBlock = new MemoryStream();
            FieldIndicesArray = new MemoryStream();
            ListIndicesArray = new MemoryStream();
        }
    }
    class GReader {
        GFactory fac;
        BinaryReader br;
        GBase gb;
        GRootStruct root;

        public GReader(GBase gbase) {
            gb = gbase;
            fac = new GFactory(gb);
        }
        public GReader(string path) {
            Load(path);
        }
        public GReader(Stream stream) {
            Load(stream);
        }
        public void Load(string path) {
            if (File.Exists(path)) {
                Load(File.OpenRead(path));
            }
        }
        public void Load(Stream stream) {
            br = new BinaryReader(stream);
            gb.Header.Load(br);
            LoadStructures();
            LoadFields();
            LoadLabels();
            LoadFieldDatas();
            LoadFieldIndicesArray();
            LoadListIndicesArray();
            br.Close();
        }
        void LoadStructures() {
            long pos = br.Stream.Position;
            br.Stream.Position = gb.Header.StructOffset;
            for (int i = 0; i < gb.Header.StructCount; i++) {
                Queue<uint> q = br.GetUInt32Queue((int)GBasicFrame.VALUE_COUNT);
                GStructFrame sf = new GStructFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                gb.StructArray.Add(sf);
            }
            br.Stream.Position = pos;
        }
        void LoadFields() {
            long pos = br.Stream.Position;
            br.Stream.Position = gb.Header.FieldOffset;
            for (int i = 0; i < gb.Header.FieldCount; i++) {
                Queue<uint> q = br.GetUInt32Queue((int)GBasicFrame.VALUE_COUNT);
                GFieldFrame ff = new GFieldFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                gb.FieldArray.Add(ff);
            }
            br.Stream.Position = pos;
        }
        void LoadLabels() {
            long pos = br.Stream.Position;
            br.Stream.Position = gb.Header.LabelOffset;
            for (int i = 0; i < gb.Header.LabelCount; i++) {
                string lbl = new string(br.ReadChars((int)GConst.LABEL_LENGTH));
                lbl = lbl.TrimEnd(GConst.LABEL_PADDING_CHARACTER);
                gb.LabelArray.Add(lbl);
            }
            br.Stream.Position = pos;
        }
        void LoadFieldDatas() {
            long pos = br.Stream.Position;
            br.Stream.Position = gb.Header.FieldDataOffset;
            gb.FieldDataBlock = new MemoryStream(br.ReadBytes((int)(gb.Header.FieldDataCount)));
            br.Stream.Position = pos;
        }
        void LoadFieldIndicesArray() {
            long pos = br.Stream.Position;
            br.Stream.Position = gb.Header.FieldIndicesOffset;
            gb.FieldIndicesArray = new MemoryStream(br.ReadBytes((int)(gb.Header.FieldIndicesCount)));
            br.Stream.Position = pos;
        }
        void LoadListIndicesArray() {
            long pos = br.Stream.Position;
            br.Stream.Position = gb.Header.ListIndicesOffset;
            gb.ListIndicesArray = new MemoryStream(br.ReadBytes((int)(gb.Header.ListIndicesCount)));
            br.Stream.Position = pos;
        }
        public GRootStruct RootStruct {
            get {
                if (root == null) {
                    root = fac.CreateRoot();
                }
                return root;
            }
        }

    }
    class GWriter {
        BinaryWriter bw;
        GRootStruct rt;
        GBase gb;
        string ext;

        public DoubleDictionary<GFieldFrame, GComponent> d_fa_f;
        public DoubleDictionary<GStructFrame, GStruct> d_sa_s;

        public GWriter(GBase gbase) {
            gb = gbase;
        }

        public MemoryStream Save(GRootStruct root) {
            bw = new BinaryWriter(new MemoryStream());
            rt = root;
            this.ext = root.Extention;
            d_fa_f = new DoubleDictionary<GFieldFrame, GComponent>();
            d_sa_s = new DoubleDictionary<GStructFrame, GStruct>();

            CreateFrames(rt);
            FillFrames();
            WriteData();

            Stream res = bw.BaseStream;
            bw.Close();
            return (MemoryStream)res;
        }
        public void Save(GRootStruct root, string path) {
            File.WriteAllBytes(path, Save(root).ToArray());
        }

        private uint GetLabelIndex(string lbl) {
            if (lbl != null) {
                if (!gb.LabelArray.Contains(lbl)) {
                    gb.LabelArray.Add(lbl);
                    return (uint)gb.LabelArray.Count - 1;
                } else {
                    return (uint)gb.LabelArray.IndexOf(lbl);
                }
            } else {
                return uint.MaxValue;
            }
        }
        private void FillFrames() {
            foreach (GStructFrame s in gb.StructArray) {
                GStruct vs = d_sa_s[s];
                s.Type = vs.StructType;
                s.DataOrDataOffset = GetStructDataOrOffset(vs);
                s.FieldCount = (uint)vs.Count;
            }
            foreach (GFieldFrame f in gb.FieldArray) {
                GComponent cpnt = d_fa_f[f];
                f.LabelIndex = GetLabelIndex(cpnt.Label);
                f.Type = (uint)cpnt.Type;
                if (cpnt is GStruct) {
                    f.DataOrDataOffset = (uint)gb.StructArray.IndexOf(d_sa_s[cpnt as GStruct]);
                } else if (cpnt is GList) {
                    f.DataOrDataOffset = GetListDataOrOffset(cpnt as GList);
                } else if (cpnt is GField) {
                    f.DataOrDataOffset = GetFieldDataOrOffset(cpnt as GField);
                } else {
                    throw new ComponentException(Error.UNKNOWN_COMPONENT_TYPE);
                }
            }
        }
        private uint GetStructDataOrOffset(GStruct vs) {
            if (vs.Count == 0) {
                return uint.MaxValue;
            } else if (vs.Count == 1) {
                return (uint)gb.FieldArray.IndexOf(d_fa_f[vs[0]]);
            } else {
                BinaryWriter br = new BinaryWriter(gb.FieldIndicesArray);
                long pos = gb.FieldIndicesArray.Position;
                foreach (GComponent cpnt in vs) {
                    br.Write(gb.FieldArray.IndexOf(d_fa_f[cpnt]));
                }
                return (uint)pos;
            }
        }
        private uint GetListDataOrOffset(GList lst) {
            BinaryWriter br = new BinaryWriter(gb.ListIndicesArray);
            long pos = gb.ListIndicesArray.Position;
            br.Write((uint)lst.Count);
            foreach (GComponent cpnt in lst) {
                if (cpnt is GInListStruct) {
                    br.Write(gb.StructArray.IndexOf(d_sa_s[cpnt as GInListStruct]));
                } else {
                    throw new CompositeException(Error.ADD_WRONG_STRUCTURE_CLASS_TO_LIST);
                }
            }
            return (uint)pos;
        }
        private uint GetFieldDataOrOffset(GField f) {
            if (GComponent.IsSimple(f.Type)) {
                byte[] l_b = f.Bytes;
                if (l_b.Length < 4) {
                    Array.Resize<byte>(ref l_b, 4);
                }
                return BitConverter.ToUInt32(l_b, 0);
            } else {
                BinaryWriter br = new BinaryWriter(gb.FieldDataBlock);
                long pos = gb.FieldDataBlock.Position;
                switch (f.Type) {
                    case GType.RESREF:
                        br.Write((byte)f.Bytes.Length);
                        break;
                    case GType.CEXOSTRING:
                    case GType.CEXOLOCSTRING:
                    case GType.VOID:
                        br.Write((uint)f.Bytes.Length);
                        break;
                }
                br.Write(f.Bytes);
                return (uint)pos;
            }
        }

        private void CreateFrames(GComponent cpnt) {
            if (cpnt is GStruct) {
                GStructFrame sf = new GStructFrame();
                gb.StructArray.Add(sf);
                d_sa_s.Add(sf, (GStruct)cpnt);
            }
            if (!(cpnt is GInListStruct || cpnt is GRootStruct)) {
                GFieldFrame ff = new GFieldFrame();
                gb.FieldArray.Add(ff);
                d_fa_f.Add(ff, cpnt);
            }
            if (GComponent.IsComposite(cpnt.Type)) {
                GComposite cpsit = cpnt as GComposite;
                foreach (GComponent child in cpsit) {
                    CreateFrames(child);
                }
            }
        }
        private void WriteData() {
            bw.Seek(GHeader.SIZE, SeekOrigin.Begin);
            WriteStructures();
            WriteFields();
            WriteLabels();
            WriteFieldData();
            WriteFieldIndices();
            WriteListIndices();
            bw.Seek(0, SeekOrigin.Begin);
            WriteHeader();
        }
        private void WriteStructures() {
            gb.Header.StructOffset = (uint)bw.BaseStream.Position;
            gb.Header.StructCount = (uint)gb.StructArray.Count;
            foreach (GStructFrame sf in gb.StructArray) {
                WriteFrame(sf);
            }
        }
        private void WriteListIndices() {
            gb.Header.ListIndicesOffset = (uint)bw.BaseStream.Position;
            gb.Header.ListIndicesCount = (uint)gb.ListIndicesArray.Length;
            bw.Write(gb.ListIndicesArray.ToArray());
        }
        private void WriteFieldData() {
            gb.Header.FieldDataOffset = (uint)bw.BaseStream.Position;
            gb.Header.FieldDataCount = (uint)gb.FieldDataBlock.Length;
            bw.Write(gb.FieldDataBlock.ToArray());
        }
        private void WriteFieldIndices() {
            gb.Header.FieldIndicesOffset = (uint)bw.BaseStream.Position;
            gb.Header.FieldIndicesCount = (uint)gb.FieldIndicesArray.Length;
            bw.Write(gb.FieldIndicesArray.ToArray());
        }
        private void WriteLabels() {
            gb.Header.LabelOffset = (uint)bw.BaseStream.Position;
            gb.Header.LabelCount = (uint)gb.LabelArray.Count;
            foreach (string lbl in gb.LabelArray) {
                bw.Write(lbl.PadRight((int)GConst.LABEL_LENGTH, GConst.LABEL_PADDING_CHARACTER).ToCharArray());
            }
        }
        private void WriteFields() {
            gb.Header.FieldOffset = (uint)bw.BaseStream.Position;
            gb.Header.FieldCount = (uint)gb.FieldArray.Count;
            foreach (GFieldFrame f in gb.FieldArray) {
                WriteFrame(f);
            }
        }
        private void WriteHeader() {
            char[] cext = ext.TrimStart('.').PadRight(GHeader.FILE_TYPE_SIZE, ' ').ToUpper().ToCharArray();
            char[] vers = (GConst.VERSION).ToCharArray();
            bw.Write(cext);
            bw.Write(vers);

            foreach (uint value in gb.Header.Infos) {
                bw.Write((uint)value);
            }
        }
        private void WriteFrame(GBasicFrame frm) {
            int i = 0;
            while (i < GBasicFrame.VALUE_COUNT) {
                bw.Write((uint)frm.Datas[i++]);
            }
        }
    }
}