using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;
using Bioware.Virtual;

namespace Bioware.GFF {

    public class GffFieldFactory {

        private GffBinaryReader _rd;
        private Stream _s;
        private GffHeader _h;

        public GffFieldFactory(GffBinaryReader rd, GffHeader h) {
            _rd = rd;
            _s = rd.Stream;
            _h = h;
        }

        private GffFieldStr createFieldStructure(int index) {
            _s.Position = _h.FieldOffset + index * GffConst.STRUCT_SIZE;
            Queue<DWORD> q = _rd.EnqueueDWORDs(GffConst.STRUCT_VALUE_COUNT);
            GffFieldStr fldStr = new GffFieldStr();
            fldStr.Type = (VType)q.Dequeue();
            fldStr.LabelIndex = q.Dequeue();
            fldStr.DataOrOffset = q.Dequeue();
            return fldStr;
        }

        public VField createField(GffFieldStr f_str, int index) {
            VField res = null;
            long savedPosition = _s.Position;
            _s.Position = getPositionByType(f_str);
            switch (f_str.Type) {
                case VType.BYTE:
                    res = createByte(f_str, index);
                    break;
                case VType.CHAR:
                    res = createChar(f_str, index);
                    break;
                case VType.WORD:
                    res = createWord(f_str, index);
                    break;
                case VType.SHORT:
                    res = createShort(f_str, index);
                    break;
                case VType.DWORD:
                    res = createDword(f_str, index);
                    break;
                case VType.INT:
                    res = createInt(f_str, index);
                    break;
                case VType.DWORD64:
                    res = createDword64(f_str, index);
                    break;
                case VType.INT64:
                    res = createInt64(f_str, index);
                    break;
                case VType.FLOAT:
                    res = createFloat(f_str, index);
                    break;
                case VType.DOUBLE:
                    res = createDouble(f_str, index);
                    break;
                case VType.CEXOSTRING:
                    res = createCExoString(f_str, index);
                    break;
                case VType.RESREF:
                    res = createResRef(f_str, index);
                    break;
                case VType.CEXOLOCSTRING:
                    res = createCExoLocString(f_str, index);
                    break;
                case VType.VOID:
                    res = createVoid(f_str, index);
                    break;
                case VType.STRUCT:
                    res = createStruct(index);
                    break;
                case VType.LIST:
                    res = createList(f_str, index);
                    break;
            }
            _s.Position = savedPosition;
            return res;
        }
        public VField createField(int index) {
            GffFieldStr f_str = createFieldStructure(index);
            return createField(f_str, index);
        }
        private long getPositionByType(GffFieldStr f_str) {
            VType type = new VType();
            type = (VType)f_str.Type;
            switch (type) {
                case VType.DWORD64:
                case VType.INT64:
                case VType.DOUBLE:
                case VType.CEXOSTRING:
                case VType.RESREF:
                case VType.CEXOLOCSTRING:
                case VType.VOID:
                    return _h.FieldDataOffset + f_str.DataOrOffset;
                case VType.STRUCT:
                    return _h.StructOffset + f_str.DataOrOffset * GffConst.STRUCT_SIZE;
                case VType.LIST:
                    return _h.ListIndicesOffset + f_str.DataOrOffset;
                default:
                    return _s.Position;
            }
        }
        public string getLabel(GffFieldStr f_str) {
            long pos = _s.Position;
            _s.Position = _h.LabelOffset + f_str.LabelIndex * GffConst.LABEL_LENGTH;
            string label = new String(_rd.ReadChars(GffConst.LABEL_LENGTH));
            _s.Position = pos;
            char[] toTrim = { '\0' };
            return label.TrimEnd(toTrim);
        }

        public VStruct createStruct(int index) {
            return new VStruct(VStruct.DEFAULT_LABEL, index);
        }
        private VList createList(GffFieldStr f_str, int index) {
            return new VList(getLabel(f_str), index);
        }

        private VByte createByte(GffFieldStr f_str, int index) {
            VByte g_byte = new VByte(getLabel(f_str), index);
            g_byte.Value = (byte) f_str.DataOrOffset;
            return g_byte;
        }
        private VChar createChar(GffFieldStr f_str, int index) {
            VChar g_char = new VChar(getLabel(f_str), index);
            g_char.Value = BitConverter.ToChar(BitConverter.GetBytes(f_str.DataOrOffset), 0);
            return g_char;
        }
        private VWord createWord(GffFieldStr f_str, int index) {
            VWord g_word = new VWord(getLabel(f_str), index);
            g_word.Value = BitConverter.ToUInt16(BitConverter.GetBytes(f_str.DataOrOffset), 0);
            return g_word;
        }
        private VShort createShort(GffFieldStr f_str, int index) {
            VShort g_short = new VShort(getLabel(f_str), index);
            g_short.Value = BitConverter.ToInt16(BitConverter.GetBytes(f_str.DataOrOffset), 0);
            return g_short;
        }
        private VDword createDword(GffFieldStr f_str, int index) {
            VDword g_dword = new VDword(getLabel(f_str), index);
            g_dword.Value = f_str.DataOrOffset;
            return g_dword;
        }
        private VInt createInt(GffFieldStr f_str, int index) {
            VInt g_int = new VInt(getLabel(f_str), index);
            byte[] bytes = BitConverter.GetBytes(f_str.DataOrOffset);
            g_int.Value = BitConverter.ToInt32(bytes, 0);
            return g_int;
        }
        private VDword64 createDword64(GffFieldStr f_str, int index) {
            VDword64 g_ulong = new VDword64(getLabel(f_str), index);
            g_ulong.Value = _rd.ReadUInt64();
            return g_ulong;
        }
        private VInt64 createInt64(GffFieldStr f_str, int index) {
            VInt64 g_long = new VInt64(getLabel(f_str), index);
            g_long.Value = _rd.ReadInt64();
            return g_long;
        }
        private VFloat createFloat(GffFieldStr f_str, int index) {
            VFloat g_float = new VFloat(getLabel(f_str), index);
            byte[] bytes = BitConverter.GetBytes(f_str.DataOrOffset);
            g_float.Value = BitConverter.ToSingle(bytes, 0);
            return g_float;
        }
        private VDouble createDouble(GffFieldStr f_str, int index) {
            VDouble g_double = new VDouble(getLabel(f_str), index);
            byte[] bytes = _rd.ReadBytes(sizeof(double));
            g_double.Value = BitConverter.ToDouble(bytes, 0);
            return g_double;
        }
        private VExoString createCExoString(GffFieldStr f_str, int index) {
            VExoString g_exo_str = new VExoString(getLabel(f_str), index);
            int size = _rd.ReadInt32();
            char[] charList = _rd.ReadChars(size);
            g_exo_str.Value = new String(charList);
            return g_exo_str;
        }
        private VResRef createResRef(GffFieldStr f_str, int index) {
            VResRef g_resref = new VResRef(getLabel(f_str), index);
            byte size = _rd.ReadByte();
            g_resref.Value = new String(_rd.ReadChars(size));
            return g_resref;
        }
        private VExoLocString createCExoLocString(GffFieldStr f_str, int index) {
            DWORD totalSize = _rd.ReadDWORD();
            DWORD strRef = _rd.ReadDWORD();
            DWORD strCount = _rd.ReadDWORD();
            VExoLocString g_exo_loc_str = new VExoLocString(getLabel(f_str), strRef, index);
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
        private VVoid createVoid(GffFieldStr f_str, int index) {
            VVoid gVoid = new VVoid(getLabel(f_str), index);
            int size = (int)_rd.ReadDWORD();
            gVoid.Value = _rd.ReadBytes(size);
            return gVoid;
        }
    }
}
