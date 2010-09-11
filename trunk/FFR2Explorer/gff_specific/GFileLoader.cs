using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;

namespace FFR2Explorer {
    public class GFileLoader {
        private const string STRUCT_LABEL = "STRUCT";
        private const string LIST_LABEL = "LIST";

        private string filePath;
        private Struct _root;
        private GBinaryReader _rd;
        private Stream _s;
        private GHeader _h;

        public string Path {
            set {
                if (File.Exists(value)) {
                    if (_rd != null) {
                        _rd.Close();
                    }
                    _root = null;
                    _rd = new GBinaryReader(File.Open(value, FileMode.Open));
                    _h = new GHeader(_rd);
                    _s = _rd.Stream;
                    filePath = value;
                } else {
                    // Erreur, le fichier GFF n'existe pas.
                }
            }
            get {
                return filePath;
            }
        }
        public Struct RootStruct {
            get {
                if (_root == null) {
                    GFieldSTR f_str = new GFieldSTR();
                    f_str.Type = GConst.STRUCT;
                    _s.Position = getPositionByType(f_str);
                    _root = createStruct(null);
                }
                return _root;
            }
            private set {
            }
        }

        public GFileLoader(string path) {
            Path = path;
        }

        private GFieldSTR createFieldStructure(int index) {
            _s.Position = _h.FieldOffset + index * GConst.STRUCT_SIZE;
            Queue<DWORD> q = _rd.EnqueueDWORDs(GConst.STRUCT_VALUE_COUNT);
            GFieldSTR fldStr = new GFieldSTR();
            fldStr.Type = q.Dequeue();
            fldStr.LabelIndex = q.Dequeue();
            fldStr.DataOrOffset = q.Dequeue();
            return fldStr;
        }
        private Field createField(CompositeField owner, int index) {
            GFieldSTR f_str = createFieldStructure(index);
            Field res = null;
            long savedPosition = _s.Position;
            _s.Position = getPositionByType(f_str);
            switch (f_str.Type) {
                case GConst.INT:
                    res = createInt(owner, f_str);
                    break;
                case GConst.DWORD64:
                    res = createDword64(owner, f_str);
                    break;
                case GConst.INT64:
                    res = createInt64(owner, f_str);
                    break;
                case GConst.FLOAT:
                    res = createFloat(owner, f_str);
                    break;
                case GConst.DOUBLE:
                    res = createDouble(owner, f_str);
                    break;
                case GConst.CEXOSTRING:
                    res = createCExoString(owner, f_str);
                    break;
                case GConst.RESREF:
                    res = createResRef(owner, f_str);
                    break;
                case GConst.CEXOLOCSTRING:
                    res = createCExoLocString(owner, f_str);
                    break;
                case GConst.VOID:
                    res = createVoid(owner, f_str);
                    break;
                case GConst.STRUCT:
                    res = createStruct(owner);
                    break;
                case GConst.LIST:
                    res = createList(owner, f_str);
                    break;
                default:
                    res = createDword(owner, f_str);
                    break;
            }
            _s.Position = savedPosition;
            return res;
        }

        private long getPositionByType(GFieldSTR f_str) {
            switch (f_str.Type) {
                case GConst.DWORD64:
                case GConst.INT64:
                case GConst.DOUBLE:
                case GConst.CEXOSTRING:
                case GConst.RESREF:
                case GConst.CEXOLOCSTRING:
                case GConst.VOID:
                    return _h.FieldDataOffset + f_str.DataOrOffset;
                case GConst.STRUCT:
                    return _h.StructOffset + f_str.DataOrOffset * GConst.STRUCT_SIZE;
                case GConst.LIST:
                    return _h.ListIndicesOffset + f_str.DataOrOffset;
                default:
                    return _s.Position;
            }
        }
        private string getLabel(GFieldSTR f_str) {
            long pos = _s.Position;
            _s.Position = _h.LabelOffset + f_str.LabelIndex * GConst.LABEL_LENGTH;
            string label = new String(_rd.ReadChars(GConst.LABEL_LENGTH));
            _s.Position = pos;
            char[] toTrim = { '\0' };
            return label.TrimEnd(toTrim);
        }

        private Struct createStruct(CompositeField owner) {
            GStructSTR s_str = new GStructSTR();
            Struct g_str = new Struct(owner, STRUCT_LABEL);
            Queue<DWORD> q = _rd.EnqueueDWORDs(GConst.STRUCT_VALUE_COUNT);
            s_str.Type = q.Dequeue();
            s_str.DataOrOffset = q.Dequeue();
            s_str.FieldCount = q.Dequeue();
            if (s_str.FieldCount == 1) {
                g_str.Childs.Add(createField(g_str, (int)s_str.DataOrOffset));
            } else {
                _s.Position = _h.FieldIndicesOffset + s_str.DataOrOffset;
                Queue<DWORD> q_index = _rd.EnqueueDWORDs((int)s_str.FieldCount);
                while (q_index.Count > 0) {
                    g_str.Childs.Add(createField(g_str, (int)q_index.Dequeue()));
                }
            }
            return g_str;
        }
        private List createList(CompositeField owner, GFieldSTR f_str) {
            List g_list = new List(owner, LIST_LABEL);
            int size = (int)_rd.ReadDWORD();
            Queue<DWORD> q = _rd.EnqueueDWORDs(size);
            while (q.Count > 0) {
                GFieldSTR tp_f_str = new GFieldSTR();
                tp_f_str.DataOrOffset = q.Dequeue();
                tp_f_str.Type = GConst.STRUCT;
                long pos = _s.Position;
                _s.Position = getPositionByType(tp_f_str);
                g_list.Childs.Add(createStruct(g_list));
                _s.Position = pos;
            }
            return g_list;
        }

        private Field createDword(CompositeField owner, GFieldSTR f_str) {
            SimpleField<DWORD> g_dword = new SimpleField<DWORD>(owner, getLabel(f_str));
            g_dword.Value = f_str.DataOrOffset;
            return g_dword;
        }
        private SimpleField<int> createInt(CompositeField owner, GFieldSTR f_str) {
            SimpleField<int> g_int = new SimpleField<int>(owner, getLabel(f_str));
            byte[] bytes = BitConverter.GetBytes(f_str.DataOrOffset);
            g_int.Value = BitConverter.ToInt32(bytes, 0);
            return g_int;
        }
        private LargeField<UInt64> createDword64(CompositeField owner, GFieldSTR f_str) {
            LargeField<UInt64> g_ulong = new LargeField<UInt64>(owner, getLabel(f_str));
            g_ulong.Value = _rd.ReadUInt64();
            return g_ulong;
        }
        private LargeField<Int64> createInt64(CompositeField owner, GFieldSTR f_str) {
            LargeField<Int64> g_long = new LargeField<Int64>(owner, getLabel(f_str));
            g_long.Value = _rd.ReadInt64();
            return g_long;
        }
        private SimpleField<float> createFloat(CompositeField owner, GFieldSTR f_str) {
            SimpleField<float> g_float = new SimpleField<float>(owner, getLabel(f_str));
            byte[] bytes = BitConverter.GetBytes(f_str.DataOrOffset);
            g_float.Value = BitConverter.ToSingle(bytes, 0);
            return g_float;
        }
        private LargeField<double> createDouble(CompositeField owner, GFieldSTR f_str) {
            LargeField<double> g_double = new LargeField<double>(owner, getLabel(f_str));
            byte[] bytes = _rd.ReadBytes(sizeof(double));
            g_double.Value = BitConverter.ToDouble(bytes, 0);
            return g_double;
        }
        private CExoString createCExoString(CompositeField owner, GFieldSTR f_str) {
            CExoString g_exo_str = new CExoString(owner, getLabel(f_str));
            int size = _rd.ReadInt32();
            char[] charList = _rd.ReadChars(size);
            g_exo_str.Value = new String(charList);
            return g_exo_str;
        }
        private ResRef createResRef(CompositeField owner, GFieldSTR f_str) {
            ResRef g_resref = new ResRef(owner, getLabel(f_str));
            byte size = _rd.ReadByte();
            g_resref.Value = new String(_rd.ReadChars(size));
            return g_resref;
        }
        private CExoLocString createCExoLocString(CompositeField owner, GFieldSTR f_str) {
            CExoLocString g_exo_loc_str = new CExoLocString(owner, getLabel(f_str));
            DWORD totalSize = _rd.ReadDWORD();
            DWORD strRef = _rd.ReadDWORD();
            DWORD strCount = _rd.ReadDWORD();
            Dictionary<int, string> value = new Dictionary<int, string>((int)strCount);
            for (int i = 0; i < strCount; i++) {
                int id = _rd.ReadInt32();
                int size = _rd.ReadInt32();
                String str = new String(_rd.ReadChars(size));
                value.Add(id, str);
            }
            g_exo_loc_str.Value = value;
            return g_exo_loc_str;
        }
        private LargeField<byte[]> createVoid(CompositeField owner, GFieldSTR f_str) {
            LargeField<byte[]> gVoid = new LargeField<byte[]>(owner, getLabel(f_str));
            int size = (int)_rd.ReadDWORD();
            gVoid.Value = _rd.ReadBytes(size);
            return gVoid;
        }
    }
}
