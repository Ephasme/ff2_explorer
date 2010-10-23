using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

using Bioware.GFF.Composite;
using Bioware.Tools;

namespace Bioware.GFF.Field {
    internal class GFieldData : MemoryStream {
        public GFieldData() {}
        public GFieldData(byte[] buffer) : base(buffer) {}
        public GFieldData(string value) : base(HexaManip.StringToByteArray(value)) {}
        public override string ToString() {
            return HexaManip.ByteArrayToString(ToArray());
        }
    }

    #region Readers.
    public interface IValueReader {
        string TextValue { get; }
        byte[] ByteArray { get; }
        void Parse(string value);
        void Parse(GField field);
    }
    internal class GByteReader : IValueReader {
        private byte _value;

        #region IValueReader Members
        public void Parse(string value) {
            _value = byte.Parse(value);
        }

        public void Parse(GField field) {
            _value = field.Bytes[0];
        }

        public string TextValue {
            get { return _value.ToString(); }
        }

        public byte[] ByteArray {
            get {
                var res = new[] {_value};
                return res;
            }
        }
        #endregion
    }
    internal class GExoLocStringReader : IValueReader {
        private Dictionary<int, string> _dic;
        private int _strref;

        #region IValueReader Members
        public void Parse(string value) {
            _dic = new Dictionary<int, string>();
            var locstrList = Regex.Split(value, "\\|\\|");
            _strref = int.Parse(locstrList[0]);
            for (var i = 1; i < locstrList.Length; i++) {
                var rgx = new Regex("(?<id>[0-9]+)=(?<value>.*)");
                var m = rgx.Match(locstrList[i]);
                _dic.Add(int.Parse(m.Groups["id"].Value), Decode(m.Groups["value"].Value));
            }
        }

        public void Parse(GField fld) {
            if (fld.Type != GType.CExoLocString) {
                throw new ApplicationException("Impossible de parser ce type " + Enum.GetName(typeof (GType), fld.Type) +
                                               " en ExoLocString.");
            }
            var br = new BinaryReader(new MemoryStream(fld.Bytes));
            _strref = (int) br.ReadUInt32();
            var strcount = br.ReadUInt32();
            _dic = new Dictionary<int, string>((int)strcount);
            for (var i=0; i < strcount;i++ ) {
                    var id = br.ReadInt32();
                    var size = br.ReadInt32();
                    _dic.Add(id, new string(br.ReadChars(size)));
                }
        }
        public string TextValue {
            get {
                var res = string.Empty;
                res += _strref;
                return _dic.Aggregate(res, (current, kvp) => current + ("||" + kvp.Key + "=" + Encode(kvp.Value)));
            }
        }
        public byte[] ByteArray {
            get {
                var br = new BinaryWriter(new MemoryStream());
                br.Write((uint) _strref);
                br.Write((uint) _dic.Count);
                foreach (var kvp in _dic) {
                    br.Write(kvp.Key);
                    br.Write(kvp.Value.Length);
                    br.Write(kvp.Value.ToCharArray());
                }
                return ((MemoryStream) (br.BaseStream)).ToArray();
            }
        }
        #endregion

        public string GetString(Lang lang) {
            return _dic.ContainsKey((int) lang) ? _dic[(int) lang] : string.Empty;
        }
        public void SetString(Lang lang, string name) {
            if (_dic.ContainsKey((int) lang)) {
                _dic[(int) lang] = name;
            }
        }

        private static string Encode(string p) {
            return p.Replace("||", "&DBLBAR&").Replace("=", "&EGAL&");
        }
        private static string Decode(string p) {
            return p.Replace("&DBLBAR&", "||").Replace("&EGAL&", "=");
        }
    }
    internal class GExoStringReader : IValueReader {
        #region IValueReader Members
        public void Parse(string value) {
            TextValue = value;
        }

        public void Parse(GField field) {
            TextValue = LatinEncoding.Latin1.GetString(field.Bytes);
        }

        public string TextValue { get; private set; }

        public byte[] ByteArray {
            get { return LatinEncoding.Latin1.GetBytes(TextValue); }
        }
        #endregion
    }
    internal class GCharReader : IValueReader {
        #region IValueReader Membres
        private char _c;
        public void Parse(string value) {
            _c = value.ToCharArray()[0];
        }

        public void Parse(GField field) {
            _c = LatinEncoding.Latin1.GetChars(field.Bytes)[0];
        }

        public string TextValue {
            get {
                char[] l = {_c};
                return new string(l);
            }
        }

        public byte[] ByteArray {
            get { return LatinEncoding.Latin1.GetBytes(TextValue); }
        }
        #endregion
    }
    internal class GDoubleReader : IValueReader {
        #region IValueReader Membres
        private double _d;
        public void Parse(string value) {
            _d = double.Parse(value);
        }

        public void Parse(GField field) {
            _d = BitConverter.ToDouble(field.Bytes, 0);
        }

        public string TextValue {
            get { return _d.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_d); }
        }
        #endregion
    }
    internal class GDwordReader : IValueReader {
        #region IValueReader Membres
        private uint _dword;
        public void Parse(string value) {
            _dword = uint.Parse(value);
        }

        public void Parse(GField field) {
            _dword = BitConverter.ToUInt32(field.Bytes, 0);
        }

        public string TextValue {
            get { return _dword.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_dword); }
        }
        #endregion
    }
    internal class GDword64Reader : IValueReader {
        #region IValueReader Membres
        private ulong _d64;
        public void Parse(string value) {
            _d64 = ulong.Parse(value);
        }

        public void Parse(GField field) {
            _d64 = BitConverter.ToUInt64(field.Bytes, 0);
        }

        public string TextValue {
            get { return _d64.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_d64); }
        }
        #endregion
    }
    internal class GFloatReader : IValueReader {
        #region IValueReader Membres
        private float _f;
        public void Parse(string value) {
            _f = float.Parse(value);
        }

        public void Parse(GField field) {
            _f = BitConverter.ToSingle(field.Bytes, 0);
        }

        public string TextValue {
            get { return _f.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_f); }
        }
        #endregion
    }
    internal class GIntReader : IValueReader {
        #region IValueReader Membres
        private int _i;
        public void Parse(string value) {
            _i = int.Parse(value);
        }

        public void Parse(GField field) {
            _i = BitConverter.ToInt32(field.Bytes, 0);
        }

        public string TextValue {
            get { return _i.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_i); }
        }
        #endregion
    }
    internal class GInt64Reader : IValueReader {
        #region IValueReader Membres
        private long _i64;
        public void Parse(string value) {
            _i64 = long.Parse(value);
        }

        public void Parse(GField field) {
            _i64 = BitConverter.ToInt64(field.Bytes, 0);
        }

        public string TextValue {
            get { return _i64.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_i64); }
        }
        #endregion
    }
    internal class GResRefReader : IValueReader {
        private ResRef _resref;

        #region IValueReader Members
        public void Parse(string value) {
            _resref = new ResRef(value);
        }

        public void Parse(GField field) {
            _resref = new ResRef(LatinEncoding.Latin1.GetString(field.Bytes));
        }

        public string TextValue {
            get { return _resref; }
        }

        public byte[] ByteArray {
            get { return LatinEncoding.Latin1.GetBytes(_resref); }
        }
        #endregion
    }
    internal class GShortReader : IValueReader {
        #region IValueReader Membres
        private short _sh;
        public void Parse(string value) {
            _sh = short.Parse(value);
        }

        public void Parse(GField field) {
            _sh = BitConverter.ToInt16(field.Bytes, 0);
        }

        public string TextValue {
            get { return _sh.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_sh); }
        }
        #endregion
    }
    internal class GWordReader : IValueReader {
        #region IValueReader Membres
        private ushort _ush;
        public void Parse(string value) {
            _ush = ushort.Parse(value);
        }

        public void Parse(GField field) {
            _ush = BitConverter.ToUInt16(field.Bytes, 0);
        }

        public string TextValue {
            get { return _ush.ToString(); }
        }

        public byte[] ByteArray {
            get { return BitConverter.GetBytes(_ush); }
        }
        #endregion
    }
    #endregion

    public class GExoLocField : GField {
        public GExoLocField(GField field) : base(field.Label, field.Type, field.Value) {
            ValueReader = field.ValueReader as GExoLocStringReader;
        }
        private new GExoLocStringReader ValueReader { get; set; }
        public string GetString(Lang lang) {
            return ValueReader.GetString(lang);
        }
        public void SetString(Lang lang, string value) {
            ValueReader.SetString(lang, value);
        }
    }

    public class GField : GComponent {
        GFieldData _fieldData;
        public IValueReader ValueReader;
        private GField(string label, GType type, GFieldData gfd) : base(label, type) {
            _fieldData = gfd;
            switch (type) {
                case GType.Byte:
                    ValueReader = new GByteReader();
                    break;
                case GType.CExoLocString:
                    ValueReader = new GExoLocStringReader();
                    break;
                case GType.CExoString:
                    ValueReader = new GExoStringReader();
                    break;
                case GType.Char:
                    ValueReader = new GCharReader();
                    break;
                case GType.Double:
                    ValueReader = new GDoubleReader();
                    break;
                case GType.Dword:
                    ValueReader = new GDwordReader();
                    break;
                case GType.Dword64:
                    ValueReader = new GDword64Reader();
                    break;
                case GType.Float:
                    ValueReader = new GFloatReader();
                    break;
                case GType.Int:
                    ValueReader = new GIntReader();
                    break;
                case GType.Int64:
                    ValueReader = new GInt64Reader();
                    break;
                case GType.ResRef:
                    ValueReader = new GResRefReader();
                    break;
                case GType.Short:
                    ValueReader = new GShortReader();
                    break;
                case GType.Word:
                    ValueReader = new GWordReader();
                    break;
                default:
                    ValueReader = null;
                    break;
            }
        }
        public GField(string label, GType type, string value) : this(label, type, new GFieldData()) {
            Value = value;
        }
        public GField(string label, GType type, byte[] data) : this(label, type, new GFieldData(data)) {}
        public byte[] Bytes {
            get { return _fieldData.ToArray(); }
        }
        public string Value {
            get {
                if (ValueReader != null) {
                    ValueReader.Parse(this);
                    return ValueReader.TextValue;
                }
                return _fieldData.ToString();
            }
            set {
                if (ValueReader != null) {
                    ValueReader.Parse(value);
                    _fieldData = new GFieldData(ValueReader.ByteArray);
                } else {
                    _fieldData = new GFieldData(value);
                }
            }
        }
    }
}