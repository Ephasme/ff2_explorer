//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Text.RegularExpressions;
//
//using Bioware.GFF.Composite;
//using Bioware.Tools;
//
//namespace Bioware.GFF.Field {
//    internal class GFieldData : MemoryStream {
//        public GFieldData() {}
//        public GFieldData(byte[] buffer) : base(buffer) {}
//        public GFieldData(string value) : base(HexaManip.StringToByteArray(value)) {}
//        public override string ToString() {
//            return HexaManip.ByteArrayToString(ToArray());
//        }
//    }
//
//    #region Readers.
//    public interface IValueReader {
//        string TextValue { get; }
//        byte[] ByteArray { get; }
//        void Parse(string value);
//        void Parse(GField field);
//    }
//    internal class GByteReader : IValueReader {
//        private byte value;
//
//        #region IValueReader Members
//        public void Parse(string value) {
//            this.value = byte.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            value = field.Bytes[0];
//        }
//
//        public string TextValue {
//            get { return value.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get {
//                var res = new[] {value};
//                return res;
//            }
//        }
//        #endregion
//    }
//    internal class GExoLocStringReader : IValueReader {
//        private Dictionary<int, string> dic;
//        private int strref;
//
//        #region IValueReader Members
//        public void Parse(string value) {
//            dic = new Dictionary<int, string>();
//            var result = new GExoLocStringReader();
//            string[] locstr_list = Regex.Split(value, "\\|\\|");
//            strref = int.Parse(locstr_list[0]);
//            for (int i = 1; i < locstr_list.Length; i++) {
//                var rgx = new Regex("(?<id>[0-9]+)=(?<value>.*)");
//                Match m = rgx.Match(locstr_list[i]);
//                dic.Add(int.Parse(m.Groups["id"].Value), Decode(m.Groups["value"].Value));
//            }
//        }
//
//        public void Parse(GField fld) {
//            if (fld.Type == GType.CExoLocString) {
//                dic = new Dictionary<int, string>();
//                var br = new BinaryReader(new MemoryStream(fld.Bytes));
//                strref = (int) br.ReadUInt32();
//                uint strcount = br.ReadUInt32();
//                var str_list = new string[strcount];
//                foreach (string str in str_list) {
//                    int id = br.ReadInt32();
//                    int size = br.ReadInt32();
//                    dic.Add(id, new string(br.ReadChars(size)));
//                }
//            } else {
//                throw new ApplicationException("Impossible de parser ce type " + Enum.GetName(typeof (GType), fld.Type) +
//                                               " en ExoLocString.");
//            }
//        }
//        public string TextValue {
//            get {
//                string res = string.Empty;
//                res += strref;
//                foreach (var kvp in dic) {
//                    res += "||" + kvp.Key + "=" + Encode(kvp.Value);
//                }
//                return res;
//            }
//        }
//        public byte[] ByteArray {
//            get {
//                var br = new BinaryWriter(new MemoryStream());
//                br.Write((uint) strref);
//                br.Write((uint) dic.Count);
//                foreach (var kvp in dic) {
//                    br.Write(kvp.Key);
//                    br.Write(kvp.Value.Length);
//                    br.Write(kvp.Value.ToCharArray());
//                }
//                return ((br.BaseStream) as MemoryStream).ToArray();
//            }
//        }
//        #endregion
//
//        public string GetString(Lang lang) {
//            if (dic.ContainsKey((int) lang)) {
//                return dic[(int) lang];
//            }
//            return string.Empty;
//        }
//        public void SetString(Lang lang, string name) {
//            if (dic.ContainsKey((int) lang)) {
//                dic[(int) lang] = name;
//            }
//        }
//
//        private static string Encode(string p) {
//            return p.Replace("||", "&DBLBAR&").Replace("=", "&EGAL&");
//        }
//        private static string Decode(string p) {
//            return p.Replace("&DBLBAR&", "||").Replace("&EGAL&", "=");
//        }
//    }
//    internal class GExoStringReader : IValueReader {
//        private string value;
//
//        #region IValueReader Members
//        public void Parse(string value) {
//            this.value = value;
//        }
//
//        public void Parse(GField field) {
//            value = LatinEncoding.Latin1.GetString(field.Bytes);
//        }
//
//        public string TextValue {
//            get { return value; }
//        }
//
//        public byte[] ByteArray {
//            get { return LatinEncoding.Latin1.GetBytes(value); }
//        }
//        #endregion
//    }
//    internal class GCharReader : IValueReader {
//        #region IValueReader Membres
//        private char c;
//        public void Parse(string value) {
//            c = value.ToCharArray()[0];
//        }
//
//        public void Parse(GField field) {
//            c = LatinEncoding.Latin1.GetChars(field.Bytes)[0];
//        }
//
//        public string TextValue {
//            get {
//                char[] l = {c};
//                return new string(l);
//            }
//        }
//
//        public byte[] ByteArray {
//            get { return LatinEncoding.Latin1.GetBytes(TextValue); }
//        }
//        #endregion
//    }
//    internal class GDoubleReader : IValueReader {
//        #region IValueReader Membres
//        private double d;
//        public void Parse(string value) {
//            d = double.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            d = BitConverter.ToDouble(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return d.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(d); }
//        }
//        #endregion
//    }
//    internal class GDwordReader : IValueReader {
//        #region IValueReader Membres
//        private uint dword;
//        public void Parse(string value) {
//            dword = uint.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            dword = BitConverter.ToUInt32(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return dword.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(dword); }
//        }
//        #endregion
//    }
//    internal class GDword64Reader : IValueReader {
//        #region IValueReader Membres
//        private ulong d64;
//        public void Parse(string value) {
//            d64 = ulong.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            d64 = BitConverter.ToUInt64(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return d64.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(d64); }
//        }
//        #endregion
//    }
//    internal class GFloatReader : IValueReader {
//        #region IValueReader Membres
//        private float f;
//        public void Parse(string value) {
//            f = float.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            f = BitConverter.ToSingle(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return f.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(f); }
//        }
//        #endregion
//    }
//    internal class GIntReader : IValueReader {
//        #region IValueReader Membres
//        private int i;
//        public void Parse(string value) {
//            i = int.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            i = BitConverter.ToInt32(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return i.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(i); }
//        }
//        #endregion
//    }
//    internal class GInt64Reader : IValueReader {
//        #region IValueReader Membres
//        private long i64;
//        public void Parse(string value) {
//            i64 = long.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            i64 = BitConverter.ToInt64(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return i64.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(i64); }
//        }
//        #endregion
//    }
//    internal class GResRefReader : IValueReader {
//        private ResRef resref;
//
//        #region IValueReader Members
//        public void Parse(string value) {
//            resref = new ResRef(value);
//        }
//
//        public void Parse(GField field) {
//            resref = new ResRef(LatinEncoding.Latin1.GetString(field.Bytes));
//        }
//
//        public string TextValue {
//            get { return resref; }
//        }
//
//        public byte[] ByteArray {
//            get { return LatinEncoding.Latin1.GetBytes(resref); }
//        }
//        #endregion
//    }
//    internal class GShortReader : IValueReader {
//        #region IValueReader Membres
//        private short sh;
//        public void Parse(string value) {
//            sh = short.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            sh = BitConverter.ToInt16(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return sh.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(sh); }
//        }
//        #endregion
//    }
//    internal class GWordReader : IValueReader {
//        #region IValueReader Membres
//        private ushort ush;
//        public void Parse(string value) {
//            ush = ushort.Parse(value);
//        }
//
//        public void Parse(GField field) {
//            ush = BitConverter.ToUInt16(field.Bytes, 0);
//        }
//
//        public string TextValue {
//            get { return ush.ToString(); }
//        }
//
//        public byte[] ByteArray {
//            get { return BitConverter.GetBytes(ush); }
//        }
//        #endregion
//    }
//    #endregion
//
//    public class GExoLocField : GField {
//        public GExoLocField(GField field) : base(field.Label, field.Type, field.Value) {
//            ValueReader = field.ValueReader as GExoLocStringReader;
//        }
//        private new GExoLocStringReader ValueReader { get; set; }
//        public string GetString(Lang lang) {
//            return ValueReader.GetString(lang);
//        }
//        public void SetString(Lang lang, string value) {
//            ValueReader.SetString(lang, value);
//        }
//    }
//
//    public class GField : GComponent {
//        private GFieldData FieldData;
//        public IValueReader ValueReader;
//        private GField(string label, GType type, GFieldData gfd) : base(label, type) {
//            FieldData = gfd;
//            switch (type) {
//                case GType.Byte:
//                    ValueReader = new GByteReader();
//                    break;
//                case GType.CExoLocString:
//                    ValueReader = new GExoLocStringReader();
//                    break;
//                case GType.CExoString:
//                    ValueReader = new GExoStringReader();
//                    break;
//                case GType.Char:
//                    ValueReader = new GCharReader();
//                    break;
//                case GType.Double:
//                    ValueReader = new GDoubleReader();
//                    break;
//                case GType.Dword:
//                    ValueReader = new GDwordReader();
//                    break;
//                case GType.Dword64:
//                    ValueReader = new GDword64Reader();
//                    break;
//                case GType.Float:
//                    ValueReader = new GFloatReader();
//                    break;
//                case GType.Int:
//                    ValueReader = new GIntReader();
//                    break;
//                case GType.Int64:
//                    ValueReader = new GInt64Reader();
//                    break;
//                case GType.ResRef:
//                    ValueReader = new GResRefReader();
//                    break;
//                case GType.Short:
//                    ValueReader = new GShortReader();
//                    break;
//                case GType.Word:
//                    ValueReader = new GWordReader();
//                    break;
//                default:
//                    ValueReader = null;
//                    break;
//            }
//        }
//        public GField(string label, GType type, string value) : this(label, type, new GFieldData()) {
//            Value = value;
//        }
//        public GField(string label, GType type, byte[] data) : this(label, type, new GFieldData(data)) {}
//        public byte[] Bytes {
//            get { return FieldData.ToArray(); }
//        }
//        public string Value {
//            get {
//                if (ValueReader != null) {
//                    ValueReader.Parse(this);
//                    return ValueReader.TextValue;
//                } else {
//                    return FieldData.ToString();
//                }
//            }
//            set {
//                if (ValueReader != null) {
//                    ValueReader.Parse(value);
//                    FieldData = new GFieldData(ValueReader.ByteArray);
//                } else {
//                    FieldData = new GFieldData(value);
//                }
//            }
//        }
//    }
//}