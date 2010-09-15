using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;

namespace FFR2Explorer {
    public class GFieldFactory {
        private const string STRUCT_LABEL = "struct";

        private GBinaryReader _rd;
        private Stream _s;
        private GHeader _h;

        public GFieldFactory(GBinaryReader rd, GHeader h) {
            _rd = rd;
            _s = rd.Stream;
            _h = h;
        }

        private GFieldSTR createFieldStructure(int index) {
            _s.Position = _h.FieldOffset + index * GConst.STRUCT_SIZE;
            Queue<DWORD> q = _rd.EnqueueDWORDs(GConst.STRUCT_VALUE_COUNT);
            GFieldSTR fldStr = new GFieldSTR();
            fldStr.Type = (GType)q.Dequeue();
            fldStr.LabelIndex = q.Dequeue();
            fldStr.DataOrOffset = q.Dequeue();
            return fldStr;
        }

        public GField createField(GFieldSTR f_str) {
            GField res = null;
            long savedPosition = _s.Position;
            _s.Position = getPositionByType(f_str);
            switch (f_str.Type) {
                case GType.BYTE:
                    res = createByte(f_str);
                    break;
                case GType.CHAR:
                    res = createChar(f_str);
                    break;
                case GType.WORD:
                    res = createWord(f_str);
                    break;
                case GType.SHORT:
                    res = createShort(f_str);
                    break;
                case GType.DWORD:
                    res = createDword(f_str);
                    break;
                case GType.INT:
                    res = createInt(f_str);
                    break;
                case GType.DWORD64:
                    res = createDword64(f_str);
                    break;
                case GType.INT64:
                    res = createInt64(f_str);
                    break;
                case GType.FLOAT:
                    res = createFloat(f_str);
                    break;
                case GType.DOUBLE:
                    res = createDouble(f_str);
                    break;
                case GType.CEXOSTRING:
                    res = createCExoString(f_str);
                    break;
                case GType.RESREF:
                    res = createResRef(f_str);
                    break;
                case GType.CEXOLOCSTRING:
                    res = createCExoLocString(f_str);
                    break;
                case GType.VOID:
                    res = createVoid(f_str);
                    break;
                case GType.STRUCT:
                    res = createStruct();
                    break;
                case GType.LIST:
                    res = createList(f_str);
                    break;
            }
            _s.Position = savedPosition;
            return res;
        }
        public GField createField(int index) {
            GFieldSTR f_str = createFieldStructure(index);
            return createField(f_str);
        }
        private long getPositionByType(GFieldSTR f_str) {
            GType type = new GType();
            type = (GType)f_str.Type;
            switch (type) {
                case GType.DWORD64:
                case GType.INT64:
                case GType.DOUBLE:
                case GType.CEXOSTRING:
                case GType.RESREF:
                case GType.CEXOLOCSTRING:
                case GType.VOID:
                    return _h.FieldDataOffset + f_str.DataOrOffset;
                case GType.STRUCT:
                    return _h.StructOffset + f_str.DataOrOffset * GConst.STRUCT_SIZE;
                case GType.LIST:
                    return _h.ListIndicesOffset + f_str.DataOrOffset;
                default:
                    return _s.Position;
            }
        }
        public string getLabel(GFieldSTR f_str) {
            long pos = _s.Position;
            _s.Position = _h.LabelOffset + f_str.LabelIndex * GConst.LABEL_LENGTH;
            string label = new String(_rd.ReadChars(GConst.LABEL_LENGTH));
            _s.Position = pos;
            char[] toTrim = { '\0' };
            return label.TrimEnd(toTrim);
        }

        public GStruct createStruct() {
            return new GStruct(STRUCT_LABEL);
        }
        private GList createList(GFieldSTR f_str) {
            return new GList(getLabel(f_str));
        }

        private GByte createByte(GFieldSTR f_str) {
            GByte g_byte = new GByte(getLabel(f_str));
            g_byte.Value = (byte) f_str.DataOrOffset;
            return g_byte;
        }
        private GChar createChar(GFieldSTR f_str) {
            GChar g_char = new GChar(getLabel(f_str));
            g_char.Value = BitConverter.ToChar(BitConverter.GetBytes(f_str.DataOrOffset), 0);
            return g_char;
        }
        private GWord createWord(GFieldSTR f_str) {
            GWord g_word = new GWord(getLabel(f_str));
            g_word.Value = BitConverter.ToUInt16(BitConverter.GetBytes(f_str.DataOrOffset), 0);
            return g_word;
        }
        private GShort createShort(GFieldSTR f_str) {
            GShort g_short = new GShort(getLabel(f_str));
            g_short.Value = BitConverter.ToInt16(BitConverter.GetBytes(f_str.DataOrOffset), 0);
            return g_short;
        }
        private GDword createDword(GFieldSTR f_str) {
            GDword g_dword = new GDword(getLabel(f_str));
            g_dword.Value = f_str.DataOrOffset;
            return g_dword;
        }
        private GInt createInt(GFieldSTR f_str) {
            GInt g_int = new GInt(getLabel(f_str));
            byte[] bytes = BitConverter.GetBytes(f_str.DataOrOffset);
            g_int.Value = BitConverter.ToInt32(bytes, 0);
            return g_int;
        }
        private GDword64 createDword64(GFieldSTR f_str) {
            GDword64 g_ulong = new GDword64(getLabel(f_str));
            g_ulong.Value = _rd.ReadUInt64();
            return g_ulong;
        }
        private GInt64 createInt64(GFieldSTR f_str) {
            GInt64 g_long = new GInt64(getLabel(f_str));
            g_long.Value = _rd.ReadInt64();
            return g_long;
        }
        private GFloat createFloat(GFieldSTR f_str) {
            GFloat g_float = new GFloat(getLabel(f_str));
            byte[] bytes = BitConverter.GetBytes(f_str.DataOrOffset);
            g_float.Value = BitConverter.ToSingle(bytes, 0);
            return g_float;
        }
        private GDouble createDouble(GFieldSTR f_str) {
            GDouble g_double = new GDouble(getLabel(f_str));
            byte[] bytes = _rd.ReadBytes(sizeof(double));
            g_double.Value = BitConverter.ToDouble(bytes, 0);
            return g_double;
        }
        private GCExoString createCExoString(GFieldSTR f_str) {
            GCExoString g_exo_str = new GCExoString(getLabel(f_str));
            int size = _rd.ReadInt32();
            char[] charList = _rd.ReadChars(size);
            g_exo_str.Value = new String(charList);
            return g_exo_str;
        }
        private GResRef createResRef(GFieldSTR f_str) {
            GResRef g_resref = new GResRef(getLabel(f_str));
            byte size = _rd.ReadByte();
            g_resref.Value = new String(_rd.ReadChars(size));
            return g_resref;
        }
        private GCExoLocString createCExoLocString(GFieldSTR f_str) {
            DWORD totalSize = _rd.ReadDWORD();
            DWORD strRef = _rd.ReadDWORD();
            DWORD strCount = _rd.ReadDWORD();
            GCExoLocString g_exo_loc_str = new GCExoLocString(getLabel(f_str), strRef);
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
        private GVoid createVoid(GFieldSTR f_str) {
            GVoid gVoid = new GVoid(getLabel(f_str));
            int size = (int)_rd.ReadDWORD();
            gVoid.Value = _rd.ReadBytes(size);
            return gVoid;
        }
    }
}
