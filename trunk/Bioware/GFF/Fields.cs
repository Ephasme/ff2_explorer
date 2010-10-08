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
    interface IValueReader {
        void Parse(string value);
        void Parse(GField field);
        string TextValue { get; }
        byte[] ByteArray { get; }
    }
    
    class GExoLocStringReader : IValueReader {
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
    class GExoStringReader : IValueReader {
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
    class GResRefReader : IValueReader {
        ResRef resref;
        public void Parse(string value) {
            resref = new ResRef(value);
        }

        public void Parse(GField field) {
            resref = new ResRef(LatinEncoding.LATIN1.GetString(field.Bytes));
        }

        public string TextValue {
            get { return resref.String; }
        }

        public byte[] ByteArray {
            get { return LatinEncoding.LATIN1.GetBytes(resref.String); }
        }
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
                case GType.CEXOLOCSTRING:
                    ValueReader = new GExoLocStringReader();
                    break;
                case GType.CEXOSTRING:
                    ValueReader = new GExoStringReader();
                    break;
                case GType.RESREF:
                    ValueReader = new GResRefReader();
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