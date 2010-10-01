using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Tools;
using GFFSystem.Virtual;
using GFFSystem.Exception;

namespace GFFSystem.GFF {
    public static class GConst {
        public const string ENCODING_NAME = "ISO-8859-1";
        public const string VERSION = "V3.2";
        public const char LABEL_PADDING_CHARACTER = '\0';
        public const int LABEL_LENGTH = 16;
    }

    public class GBinaryReader : BinaryReader {

        /// <summary>
        /// Accès au stream associé à ce lecteur.
        /// </summary>
        public Stream Stream { set; get; }

        /// <summary>
        /// Lecteur de données binaires spécifique aux fichiers GFF.
        /// </summary>
        /// <param name="stream">Stream d'un fichier GFF.</param>
        public GBinaryReader(Stream stream)
            : base(stream, Encoding.GetEncoding(GConst.ENCODING_NAME)) {
            Stream = stream;
        }

        /// <summary>
        /// Lit et renvoie une valeur DWORD.
        /// </summary>
        public uint ReadDWORD() {
            return ReadUInt32();
        }

        /// <summary>
        /// Renvoie une queue de DWORD contenant [count] valeurs.
        /// </summary>
        /// <param name="count">Nombre de DWORD que la fonction doit lire et stocker.</param>
        public Queue<uint> EnqueueDWORDs(int count) {
            Queue<uint> q = new Queue<uint>(count);
            for (int i = 0; i < count; i++) {
                q.Enqueue(ReadDWORD());
            }
            return q;
        }

        /// <summary>
        /// Renvoie une liste de DWORD contenant [count] valeurs.
        /// </summary>
        /// <param name="count">Nombre de DWORD que la fonction doit lire et stocker.</param>
        public List<uint> ListDWORDS(int count) {
            List<uint> l = new List<uint>(count);
            for (int i = 0; i < count; i++) {
                l.Add(ReadDWORD());
            }
            return l;
        }
    }
    public class GBinaryWriter : BinaryWriter {
        public GBinaryWriter(Stream stream)
            : base(stream, Encoding.GetEncoding(GConst.ENCODING_NAME)) {
        }
    }

    public class GHeader {
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

        public void Load(string path) {
            if (File.Exists(path)) {
                GBinaryReader br = new GBinaryReader(File.Open(path, FileMode.Open));

                Type = (new String(br.ReadChars(FILE_TYPE_SIZE))).Trim();
                Version = (new String(br.ReadChars(FILE_VERSION_SIZE))).Trim();

                Queue<uint> q = br.EnqueueDWORDs(DWORD_TABLE_SIZE);
                int i = 0;
                while (q.Count > 0) {
                    Infos[i++] = q.Dequeue();
                }
                br.Close();
            }
        }
    }

    public abstract class GBasicFrame {
        public const int TYPE = 0;
        public const int VALUE_COUNT = 3;
        public const int SIZE = VALUE_COUNT * sizeof(uint);
        public uint Type {
            get {
                return Datas[TYPE];
            }
            set {
                Datas[TYPE] = value;
            }
        }
        public uint[] Datas { get; set; }
        public GBasicFrame() {
            Datas = new uint[VALUE_COUNT];
        }
    }
    public class GFieldFrame : GBasicFrame {
        public const int LABEL_INDEX = 1;
        public const int DATA_OR_DATA_OFFSET = 2;
        public uint LabelIndex {
            get {
                return Datas[LABEL_INDEX];
            }
            set {
                Datas[LABEL_INDEX] = value;
            }
        }
        public uint DataOrDataOffset {
            get {
                return Datas[DATA_OR_DATA_OFFSET];
            }
            set {
                Datas[DATA_OR_DATA_OFFSET] = value;
            }
        }
        public GFieldFrame(uint Type, uint LabelIndex, uint DataOrDataOffset) {
            this.Type = Type;
            this.LabelIndex = LabelIndex;
            this.DataOrDataOffset = DataOrDataOffset;
        }
        public GFieldFrame() : this(0, 0, 0) { }
    }
    public class GStructFrame : GBasicFrame {
        public const int DATA_OR_DATA_OFFSET = 1;
        public const int FIELD_COUNT = 2;
        public uint FieldCount {
            get {
                return Datas[FIELD_COUNT];
            }
            set {
                Datas[FIELD_COUNT] = value;
            }
        }
        public uint DataOrDataOffset {
            get {
                return Datas[DATA_OR_DATA_OFFSET];
            }
            set {
                Datas[DATA_OR_DATA_OFFSET] = value;
            }
        }
        public GStructFrame(uint Type, uint DataOrDataOffset, uint FieldCount) {
            this.Type = Type;
            this.FieldCount = FieldCount;
            this.DataOrDataOffset = DataOrDataOffset;
        }
        public GStructFrame() : this(0, 0, 0) { }
    }

    public abstract class GFile {
        protected GHeader h;

        protected List<GFieldFrame> l_fa;
        protected List<GStructFrame> l_sa;
        protected List<string> l_lbl;

        protected MemoryStream ms_f_db, ms_f_ia, ms_l_ia;
        protected GBinaryWriter bw_f_db, bw_f_ia, bw_l_ia;

        public List<GStructFrame> StructArray {
            get {
                return l_sa;
            }
        }
        public List<GFieldFrame> FieldArray {
            get {
                return l_fa;
            }
        }
        public List<string> LabelArray {
            get {
                return l_lbl;
            }
        }
        public MemoryStream FieldDataBlock {
            get {
                return ms_f_db;
            }
        }
        public MemoryStream FieldIndicesArray {
            get {
                return ms_f_ia;
            }
        }
        public MemoryStream ListIndicesArray {
            get {
                return ms_l_ia;
            }
        }

        public GFile() {
        }

        protected void Initialize() {
            h = new GHeader();
            l_sa = new List<GStructFrame>();
            l_fa = new List<GFieldFrame>();
            l_lbl = new List<string>();
            ms_f_db = new MemoryStream();
            bw_f_db = new GBinaryWriter(ms_f_db);
            ms_f_ia = new MemoryStream();
            bw_f_ia = new GBinaryWriter(ms_f_ia);
            ms_l_ia = new MemoryStream();
            bw_l_ia = new GBinaryWriter(ms_l_ia);
        }
    }

    public class GFactory {
        GFileReader gfRdr;
        public GFactory(GFileReader gfRdr) {
            this.gfRdr = gfRdr;
        }

        private VComponent CreateComponent(uint f_index) {

            GFieldFrame f = gfRdr.FieldArray[(int)f_index];
            uint f_lbl_id = f.LabelIndex;
            string f_lbl = gfRdr.LabelArray[(int)f_lbl_id];
            VType f_type = (VType)f.Type;

            if (VComponent.IsComposite(f_type)) {
                if (f_type == VType.STRUCT) {

                    // On va chercher la frame qui lui correspond.
                    uint s_index = f.DataOrDataOffset;
                    GStructFrame s = gfRdr.StructArray[(int)s_index];
                    uint s_type = s.Type;
                    VInFieldStruct vs = new VInFieldStruct(f_lbl, s_type);

                    PopulateStruct(vs, GetStructFieldIndices(s));

                    return vs;

                } else if (f_type == VType.LIST) {
                    GBinaryReader br = new GBinaryReader(gfRdr.ListIndicesArray);
                    long pos = br.Stream.Position;
                    br.Stream.Position = f.DataOrDataOffset;
                    int size = (int)br.ReadDWORD();
                    VList vl = new VList(f_lbl);
                    List<uint> struct_id_list = br.ListDWORDS(size);
                    foreach (uint struct_id in struct_id_list) {
                        GStructFrame s_frame = gfRdr.StructArray[(int)struct_id];
                        VInListStruct vls = new VInListStruct(s_frame.Type);
                        PopulateStruct(vls, GetStructFieldIndices(s_frame));
                        vl.Add(vls);
                    }
                    return vl;
                } else {
                    throw new CompositeException(GError.UNKNOWN_COMPOSITE_TYPE);
                }
            } else {
                VFieldData f_dat;
                if (VComponent.IsSimple(f_type)) {
                    f_dat = new VFieldData(BitConverter.GetBytes(f.DataOrDataOffset));
                } else if (VComponent.IsLarge(f_type)) {
                    GBinaryReader br = new GBinaryReader(gfRdr.FieldDataBlock);
                    long pos = br.Stream.Position;
                    br.Stream.Position = f.DataOrDataOffset;
                    int size = 0;
                    #region Switch de détermination de la taille.
                    switch (f_type) {
                        case VType.DWORD64:
                            size = sizeof(UInt64);
                            break;
                        case VType.INT64:
                            size = sizeof(Int64);
                            break;
                        case VType.DOUBLE:
                            size = sizeof(double);
                            break;
                        case VType.RESREF:
                            size = (int)br.ReadByte();
                            break;
                        case VType.CEXOSTRING:
                        case VType.CEXOLOCSTRING:
                        case VType.VOID:
                            size = (int)br.ReadUInt32();
                            break;
                        default:
                            throw new FieldException(GError.UNKNOWN_LARGE_FIELD_TYPE);
                    }
                    #endregion
                    f_dat = new VFieldData(br.ReadBytes(size));
                    br.Stream.Position = pos;
                } else {
                    throw new FieldException(GError.UNKNOWN_FIELD_TYPE);
                }
                return new VField(f_lbl, f_type, f_dat);
            }
            throw new ComponentException(GError.UNKNOWN_COMPONENT_TYPE);
        }

        private void PopulateStruct(VInFieldStruct vs, List<uint> FieldIndexes) {
            foreach (uint f_id in FieldIndexes) {
                VComponent cpnt = CreateComponent(f_id);
                vs.Add(cpnt);
            }
        }

        private List<uint> GetStructFieldIndices(GStructFrame s_frame) {
            List<uint> FieldIndices = new List<uint>();
            if (s_frame.FieldCount > 1) {
                GBinaryReader br = new GBinaryReader(gfRdr.FieldIndicesArray);
                long pos = br.Stream.Position;
                br.Stream.Position = s_frame.DataOrDataOffset;
                FieldIndices = br.ListDWORDS((int)s_frame.FieldCount);
                br.Stream.Position = pos;
            } else if (s_frame.FieldCount == 1) {
                FieldIndices.Add(s_frame.DataOrDataOffset);
            }
            return FieldIndices;
        }

        public VRootStruct CreateRoot() {
            GStructFrame root_frame = gfRdr.StructArray[(int)VRootStruct.ROOT_INDEX];
            VRootStruct root = new VRootStruct();
            PopulateStruct(root, GetStructFieldIndices(root_frame));
            return root;
        }
    }
    public class GFileReader : GFile {

        private GFactory fac;
        private GBinaryReader br;

        public GFileReader()
            : base() {
            fac = new GFactory(this);
        }

        public void Load(string path) {
            Initialize();
            if (File.Exists(path)) {
                h.Load(path);
                br = new GBinaryReader(File.Open(path, FileMode.Open));
                LoadStructures();
                LoadFields();
                LoadLabels();
                LoadFieldDatas();
                LoadFieldIndicesArray();
                LoadListIndicesArray();
                br.Close();
            }
        }
        private void LoadStructures() {
            long pos = br.Stream.Position;
            br.Stream.Position = h.StructOffset;
            for (int i = 0; i < h.StructCount; i++) {
                Queue<uint> q = br.EnqueueDWORDs((int)GBasicFrame.VALUE_COUNT);
                GStructFrame sf = new GStructFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                l_sa.Add(sf);
            }
            br.Stream.Position = pos;
        }
        private void LoadFields() {
            long pos = br.Stream.Position;
            br.Stream.Position = h.FieldOffset;
            for (int i = 0; i < h.FieldCount; i++) {
                Queue<uint> q = br.EnqueueDWORDs((int)GBasicFrame.VALUE_COUNT);
                GFieldFrame ff = new GFieldFrame(q.Dequeue(), q.Dequeue(), q.Dequeue());
                l_fa.Add(ff);
            }
            br.Stream.Position = pos;
        }
        private void LoadLabels() {
            long pos = br.Stream.Position;
            br.Stream.Position = h.LabelOffset;
            for (int i = 0; i < h.LabelCount; i++) {
                string lbl = new string(br.ReadChars((int)GConst.LABEL_LENGTH));
                lbl = lbl.TrimEnd(GConst.LABEL_PADDING_CHARACTER);
                l_lbl.Add(lbl);
            }
            br.Stream.Position = pos;
        }
        private void LoadFieldDatas() {
            long pos = br.Stream.Position;
            br.Stream.Position = h.FieldDataOffset;
            ms_f_db = new MemoryStream(br.ReadBytes((int)(h.FieldDataCount)));
            br.Stream.Position = pos;
        }
        private void LoadFieldIndicesArray() {
            long pos = br.Stream.Position;
            br.Stream.Position = h.FieldIndicesOffset;
            ms_f_ia = new MemoryStream(br.ReadBytes((int)(h.FieldIndicesCount)));
            br.Stream.Position = pos;
        }
        private void LoadListIndicesArray() {
            long pos = br.Stream.Position;
            br.Stream.Position = h.ListIndicesOffset;
            ms_l_ia = new MemoryStream(br.ReadBytes((int)(h.ListIndicesCount)));
            br.Stream.Position = pos;
        }
        public VRootStruct RootStruct {
            get {
                return fac.CreateRoot();
            }
        }
    }
    public class GFileWriter : GFile {

        GBinaryWriter bw;
        VRootStruct Root;
        string Ext, SavePath;

        public DoubleDictionary<GFieldFrame, VComponent> d_fa_f;
        public DoubleDictionary<GStructFrame, VStruct> d_sa_s;

        public GFileWriter()
            : base() {
        }

        public void Save(VRootStruct root, string ext, string path) {
            Initialize();
            Root = root;
            Ext = ext.TrimStart('.');
            d_fa_f = new DoubleDictionary<GFieldFrame, VComponent>();
            d_sa_s = new DoubleDictionary<GStructFrame, VStruct>();
            SavePath = path;
            if (File.Exists(path)) {
                bw = new GBinaryWriter(File.Open(path, FileMode.Truncate));
            } else {
                bw = new GBinaryWriter(File.Open(path, FileMode.Create));
            }

            CreateFrames(Root);
            FillFrames();
            WriteData();

            bw.Close();
        }
        private uint GetLabelIndex(string lbl) {
            if (lbl != null) {
                if (!l_lbl.Contains(lbl)) {
                    l_lbl.Add(lbl);
                    return (uint)l_lbl.Count - 1;
                } else {
                    return (uint)l_lbl.IndexOf(lbl);
                }
            } else {
                return uint.MaxValue;
            }
        }
        private void FillFrames() {
            foreach (GStructFrame s in l_sa) {
                VStruct vs = d_sa_s[s];
                s.Type = vs.StructType;
                s.DataOrDataOffset = GetStructDataOrOffset(vs);
                s.FieldCount = (uint)vs.Get().Count;
            }
            foreach (GFieldFrame f in l_fa) {
                VComponent cpnt = d_fa_f[f];
                f.LabelIndex = GetLabelIndex(cpnt.Label);
                f.Type = (uint)cpnt.Type;
                if (cpnt is VStruct) {
                    f.DataOrDataOffset = (uint)l_sa.IndexOf(d_sa_s[(VStruct)cpnt]);
                } else if (cpnt is VList) {
                    f.DataOrDataOffset = GetListDataOrOffset((VList)cpnt);
                } else if (cpnt is VField) {
                    f.DataOrDataOffset = GetFieldDataOrOffset((VField)cpnt);
                } else {
                    throw new ComponentException(GError.UNKNOWN_COMPONENT_TYPE);
                }
            }
        }
        private uint GetStructDataOrOffset(VStruct vs) {
            if (vs.Get().Count == 0) {
                return uint.MaxValue;
            } else if (vs.Get().Count == 1) {
                return (uint)l_fa.IndexOf(d_fa_f[vs.Get()[0]]);
            } else {
                long pos = ms_f_ia.Position;
                foreach (VComponent cpnt in vs.Get()) {
                    bw_f_ia.Write(l_fa.IndexOf(d_fa_f[cpnt]));
                }
                return (uint)pos;
            }
        }
        private uint GetListDataOrOffset(VList lst) {
            long pos = ms_l_ia.Position;
            bw_l_ia.Write((uint)lst.Get().Count);
            foreach (VComponent cpnt in lst.Get()) {
                if (cpnt is VInListStruct) {
                    bw_l_ia.Write(l_sa.IndexOf(d_sa_s[(VInListStruct)cpnt]));
                } else {
                    throw new CompositeException(GError.ADD_WRONG_STRUCTURE_CLASS_TO_LIST);
                }
            }
            return (uint)pos;
        }
        private uint GetFieldDataOrOffset(VField f) {
            if (VComponent.IsSimple(f.Type)) {
                return BitConverter.ToUInt32(f.FieldData.DataBuffer, 0);
            } else {
                long pos = ms_f_db.Position;
                switch (f.Type) {
                    case VType.RESREF:
                        bw_f_db.Write((byte)f.FieldData.DataBuffer.Length);
                        break;
                    case VType.CEXOSTRING:
                    case VType.CEXOLOCSTRING:
                    case VType.VOID:
                        bw_f_db.Write((uint)f.FieldData.DataBuffer.Length);
                        break;
                }
                bw_f_db.Write(f.FieldData.DataBuffer);
                return (uint)pos;
            }
        }

        private void CreateFrames(VComponent cpnt) {
            if (cpnt is VStruct) {
                GStructFrame sf = new GStructFrame();
                l_sa.Add(sf);
                d_sa_s.Add(sf, (VStruct)cpnt);
            }
            if (!(cpnt is VInListStruct || cpnt is VRootStruct)) {
                GFieldFrame ff = new GFieldFrame();
                l_fa.Add(ff);
                d_fa_f.Add(ff, cpnt);
            }
            if (VComponent.IsComposite(cpnt.Type)) {
                VComposite cpsit = (VComposite)cpnt;
                foreach (VComponent child in cpsit.Get()) {
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
            h.StructOffset = (uint)bw.BaseStream.Position;
            h.StructCount = (uint)l_sa.Count;
            foreach (GStructFrame sf in l_sa) {
                WriteFrame(sf);
            }
        }
        private void WriteListIndices() {
            h.ListIndicesOffset = (uint)bw.BaseStream.Position;
            h.ListIndicesCount = (uint)ms_l_ia.Length;
            bw.Write(ms_l_ia.ToArray());
        }
        private void WriteFieldData() {
            h.FieldDataOffset = (uint)bw.BaseStream.Position;
            h.FieldDataCount = (uint)ms_f_db.Length;
            bw.Write(ms_f_db.ToArray());
        }
        private void WriteFieldIndices() {
            h.FieldIndicesOffset = (uint)bw.BaseStream.Position;
            h.FieldIndicesCount = (uint)ms_f_ia.Length;
            bw.Write(ms_f_ia.ToArray());
        }
        private void WriteLabels() {
            h.LabelOffset = (uint)bw.BaseStream.Position;
            h.LabelCount = (uint)l_lbl.Count;
            foreach (string lbl in l_lbl) {
                bw.Write(lbl.PadRight((int)GConst.LABEL_LENGTH, GConst.LABEL_PADDING_CHARACTER).ToCharArray());
            }
        }
        private void WriteFields() {
            h.FieldOffset = (uint)bw.BaseStream.Position;
            h.FieldCount = (uint)l_fa.Count;
            foreach (GFieldFrame f in l_fa) {
                WriteFrame(f);
            }
        }
        private void WriteHeader() {
            char[] ext = Ext.PadRight(GHeader.FILE_TYPE_SIZE, ' ').ToUpper().ToCharArray();
            char[] vers = (GConst.VERSION).ToCharArray();
            bw.Write(ext);
            bw.Write(vers);

            foreach (uint value in h.Infos) {
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