using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;
using Bioware.GFF.Composite;
using Bioware.Tools;
using System.Text;

namespace Bioware.GFF.Field {
    class GFieldData : MemoryStream {
        public GFieldData() : base() { }
        public GFieldData(byte[] buffer) : base(buffer) { }
        public GFieldData(string value) : base(HexaManip.StringToByteArray(value)) { }
        public override string ToString() {
            return HexaManip.ByteArrayToString(this.ToArray());
        }
    }

    public interface IValueReader {
        void Parse(string value);
        void Parse(GField field);
        string TextValue { get; }
        byte[] ByteArray { get; }
    }

    public class GByteReader : IValueReader {
        byte value;
        public void Parse(string value) {
            this.value = byte.Parse(value);
        }

        public void Parse(GField field) {
            this.value = field.Bytes[0];
        }

        public string TextValue {
            get { return value.ToString(); }
        }

        public byte[] ByteArray {

            get {
                byte[] res = new byte[] { value };
                return res;
            }
        }
    }
    public class GExoLocStringReader : IValueReader {
        Dictionary<int, string> dic;
        int strref;
        public void Parse(string value) {
            dic = new Dictionary<int, string>();
            GExoLocStringReader result = new GExoLocStringReader();
            string[] locstr_list = Regex.Split(value, "\\|\\|");
            strref = int.Parse(locstr_list[0]);
            for (int i = 1; i < locstr_list.Length; i++) {
                Regex rgx = new Regex("(?<id>[0-9]+)=(?<value>.*)");
                Match m = rgx.Match(locstr_list[i]);
                dic.Add(int.Parse(m.Groups["id"].Value), _Decode(m.Groups["value"].Value));
            }
        }
        public void Parse(GField fld) {
            if (fld.Type == GType.CEXOLOCSTRING) {
                dic = new Dictionary<int, string>();
                BinaryReader br = new BinaryReader(new MemoryStream(fld.Bytes));
                strref = (int)br.ReadUInt32();
                uint strcount = br.ReadUInt32();
                string[] str_list = new string[strcount];
                foreach (string str in str_list) {
                    int id = br.ReadInt32();
                    int size = br.ReadInt32();
                    dic.Add(id, new string(br.ReadChars(size)));
                }
            } else {
                throw new ApplicationException("Impossible de parser ce type " + Enum.GetName(typeof(GType), fld.Type) + " en ExoLocString.");
            }
        }
        public string TextValue {
            get {
                string res = string.Empty;
                res += strref;
                foreach (KeyValuePair<int, string> kvp in dic) {
                    res += "||" + kvp.Key + "=" + _Encode(kvp.Value);
                }
                return res;
            }
        }
        public byte[] ByteArray {
            get {
                BinaryWriter br = new BinaryWriter(new MemoryStream());
                br.Write((uint)strref);
                br.Write((uint)dic.Count);
                foreach (KeyValuePair<int, string> kvp in dic) {
                    br.Write(kvp.Key);
                    br.Write(kvp.Value.Length);
                    br.Write(kvp.Value.ToCharArray());
                }
                return ((br.BaseStream) as MemoryStream).ToArray();
            }
        }

        private string _Encode(string p) {
            return p.Replace("||", "&DBLBAR&").Replace("=", "&EGAL&");
        }
        private string _Decode(string p) {
            return p.Replace("&DBLBAR&", "||").Replace("&EGAL&", "=");
        }
    }
    public class GExoStringReader : IValueReader {
        public GExoStringReader() { }
        string value;
        public void Parse(string value) {
            this.value = value;
        }

        public void Parse(GField field) {
            value = LatinEncoding.LATIN1.GetString(field.Bytes);
        }

        public string TextValue {
            get { return value; }
        }

        public byte[] ByteArray {
            get { return LatinEncoding.LATIN1.GetBytes(value); }
        }
    }
    public class GCharReader : IValueReader {
        #region IValueReader Membres
        char c;
        public void Parse(string value) {
            c = value.ToCharArray()[0];
        }

        public void Parse(GField field) {
            c = LatinEncoding.LATIN1.GetChars(field.Bytes)[0];
        }

        public string TextValue {
            get {
                char[] l = { c };
                return new string(l);
            }
        }

        public byte[] ByteArray {
            get { return LatinEncoding.LATIN1.GetBytes(TextValue); }
        }

        #endregion
    }
    public class GDoubleReader : IValueReader {
        #region IValueReader Membres
        double d;
        public void Parse(string value) {
            d = double.Parse(value);
        }

        public void Parse(GField field) {
            d = BitConverter.ToDouble(field.Bytes, 0);
        }

        public string TextValue {
            get { return d.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(d); }
        }

        #endregion
    }
    public class GDwordReader : IValueReader {
        #region IValueReader Membres
        uint dword;
        public void Parse(string value) {
            dword = uint.Parse(value);
        }

        public void Parse(GField field) {
            dword = BitConverter.ToUInt32(field.Bytes, 0);
        }

        public string TextValue {
            get { return dword.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(dword); }
        }

        #endregion
    }
    public class GDword64Reader : IValueReader {
        #region IValueReader Membres
        ulong d64;
        public void Parse(string value) {
            d64 = ulong.Parse(value);
        }

        public void Parse(GField field) {
            d64 = BitConverter.ToUInt64(field.Bytes, 0);
        }

        public string TextValue {
            get { return d64.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(d64); }
        }

        #endregion
    }
    public class GFloatReader : IValueReader {
        #region IValueReader Membres
        float f;
        public void Parse(string value) {
            f = float.Parse(value);
        }

        public void Parse(GField field) {
            f = BitConverter.ToSingle(field.Bytes, 0);
        }

        public string TextValue {
            get { return f.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(f); }
        }

        #endregion
    }
    public class GIntReader : IValueReader {
        #region IValueReader Membres
        int i;
        public void Parse(string value) {
            i = int.Parse(value);
        }

        public void Parse(GField field) {
            i = BitConverter.ToInt32(field.Bytes, 0);
        }

        public string TextValue {
            get { return i.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(i); }
        }

        #endregion
    }
    public class GInt64Reader : IValueReader {
        #region IValueReader Membres
        long i64;
        public void Parse(string value) {
            i64 = long.Parse(value);
        }

        public void Parse(GField field) {
            i64 = BitConverter.ToInt64(field.Bytes, 0);
        }

        public string TextValue {
            get { return i64.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(i64); }
        }

        #endregion
    }
    public class GResRefReader : IValueReader {
        ResRef resref;
        public void Parse(string value) {
            resref = new ResRef(value);
        }

        public void Parse(GField field) {
            resref = new ResRef(LatinEncoding.LATIN1.GetString(field.Bytes));
        }

        public string TextValue {
            get { return resref; }
        }

        public byte[] ByteArray {
            get { return LatinEncoding.LATIN1.GetBytes(resref); }
        }
    }
    public class GShortReader : IValueReader {
        #region IValueReader Membres
        short sh;
        public void Parse(string value) {
            sh = short.Parse(value);
        }

        public void Parse(GField field) {
            sh = BitConverter.ToInt16(field.Bytes, 0);
        }

        public string TextValue {
            get { return sh.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(sh); }
        }

        #endregion
    }
    public class GWordReader : IValueReader {
        #region IValueReader Membres
        ushort ush;
        public void Parse(string value) {
            ush = ushort.Parse(value);
        }

        public void Parse(GField field) {
            ush = BitConverter.ToUInt16(field.Bytes, 0);
        }

        public string TextValue {
            get { return ush.ToString(); }
        }

        public byte[] ByteArray {
            get {
                return BitConverter.GetBytes(ush);
            }
        }

        #endregion
    }

    public class GField : GComponent {
        IValueReader ValueReader;
        GFieldData FieldData;
        public byte[] Bytes {
            get {
                return FieldData.ToArray();
            }
        }
        private GField(string label, GType type, GFieldData gfd)
            : base(label, type) {
            FieldData = gfd;
            switch (type) {
                case GType.BYTE:
                    ValueReader = new GByteReader();
                    break;
                case GType.CEXOLOCSTRING:
                    ValueReader = new GExoLocStringReader();
                    break;
                case GType.CEXOSTRING:
                    ValueReader = new GExoStringReader();
                    break;
                case GType.CHAR:
                    ValueReader = new GCharReader();
                    break;
                case GType.DOUBLE:
                    ValueReader = new GDoubleReader();
                    break;
                case GType.DWORD:
                    ValueReader = new GDwordReader();
                    break;
                case GType.DWORD64:
                    ValueReader = new GDword64Reader();
                    break;
                case GType.FLOAT:
                    ValueReader = new GFloatReader();
                    break;
                case GType.INT:
                    ValueReader = new GIntReader();
                    break;
                case GType.INT64:
                    ValueReader = new GInt64Reader();
                    break;
                case GType.RESREF:
                    ValueReader = new GResRefReader();
                    break;
                case GType.SHORT:
                    ValueReader = new GShortReader();
                    break;
                case GType.WORD:
                    ValueReader = new GWordReader();
                    break;
                default:
                    ValueReader = null;
                    break;
            }
        }
        public GField(string label, GType type, string value)
            : this(label, type, new GFieldData()) {
            Value = value;
        }
        public GField(string label, GType type, byte[] data)
            : this(label, type, new GFieldData(data)) {
        }
        public string Value {
            get {
                if (ValueReader != null) {
                    ValueReader.Parse(this);
                    return ValueReader.TextValue;
                } else {
                    return FieldData.ToString();
                }
            }
            set {
                if (ValueReader != null) {
                    ValueReader.Parse(value);
                    FieldData = new GFieldData(ValueReader.ByteArray);
                } else {
                    FieldData = new GFieldData(value);
                }
            }
        }
    }
}