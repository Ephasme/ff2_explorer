using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace Bioware {
    public class ResRef {
        public const int LENGTH = 16;
        private string _value;
        public ResRef(char[] resRef) {
            CharTable = resRef;
        }
        public ResRef(string resRef) {
            TestLength(resRef.ToCharArray());
            _value = resRef;
        }
        public char[] CharTable {
            get { return _value.PadRight(LENGTH, '\0').ToCharArray(); }
            set {
                TestLength(value);
                _value = new string(value).TrimEnd('\0').Trim().ToLower();
            }
        }
        public static implicit operator string(ResRef resref) {
            return resref.ToString();
        }
        public static explicit operator ResRef(string value) {
            return new ResRef(value);
        }
        public static bool operator ==(ResRef r1, ResRef r2) {
            return r1.Equals(r2);
        }
        public static bool operator !=(ResRef r1, ResRef r2) {
            return !(r1.Equals(r2));
        }
        public override bool Equals(object obj) {
            var rr = obj as ResRef;
            if (rr != null) {
                return (rr._value == _value);
            }
            return false;
        }
        public override int GetHashCode() {
            return BitConverter.ToInt32(Encoding.ASCII.GetBytes(_value), 0);
        }
        public override string ToString() {
            return _value;
        }

        private static void TestLength(char[] resref) {
            if (resref.Length > LENGTH) {
                throw new ApplicationException("ResRef trop long.");
            }
        }
    }
    public enum ResType : ushort {
        Bmp = 1,
        Tga = 3,
        Wav = 4,
        Plt = 6,
        Ini = 7,
        Txt = 10,
        Mdl = 2002,
        Nss = 2009,
        Ncs = 2010,
        Are = 2012,
        Set = 2013,
        Ifo = 2014,
        Bic = 2015,
        Wok = 2016,
        Dda = 2017,
        Txi = 2022,
        Git = 2023,
        Uti = 2025,
        Utc = 2027,
        Dlg = 2029,
        Itp = 2030,
        Utt = 2032,
        Dds = 2033,
        Uts = 2035,
        Ltr = 2036,
        Gff = 2037,
        Fac = 2038,
        Ute = 2040,
        Utd = 2042,
        Utp = 2044,
        Dft = 2045,
        Gic = 2046,
        Gui = 2047,
        Utm = 2051,
        Dwk = 2052,
        Pwk = 2053,
        Jrl = 2056,
        Utw = 2058,
        Ssf = 2060,
        Ndb = 2064,
        Ptm = 2065,
        Ptt = 2066
    }
    public class BinaryReader : System.IO.BinaryReader {
        public BinaryReader(Stream stream) : base(stream, LatinEncoding.Latin1) {
            Stream = stream;
        }
        public Stream Stream { set; get; }
        public Queue<uint> GetUInt32Queue(int count) {
            var q = new Queue<uint>(count);
            for (int i = 0; i < count; i++) {
                q.Enqueue(ReadUInt32());
            }
            return q;
        }
        public List<uint> GetUInt32List(int count) {
            var l = new List<uint>(count);
            for (var i = 0; i < count; i++) {
                l.Add(ReadUInt32());
            }
            return l;
        }
        public Queue<int> GetInt32Queue(int count) {
            var q = new Queue<int>(count);
            for (var i = 0; i < count; i++) {
                q.Enqueue(ReadInt32());
            }
            return q;
        }
        public List<int> GetInt32List(int count) {
            var l = new List<int>(count);
            for (var i = 0; i < count; i++) {
                l.Add(ReadInt32());
            }
            return l;
        }
    }
    public class BinaryWriter : System.IO.BinaryWriter {
        public BinaryWriter(Stream stream) : base(stream, LatinEncoding.Latin1) {}
    }
    public static class LatinEncoding {
        public const string Name = "ISO-8859-1";
        public static Encoding Latin1 = Encoding.GetEncoding(Name);
    }
    public enum Gender : uint {
        Male = 0,
        None = 0,
        Female = 1
    }
    public enum Lang : uint {
        English,
        French,
        German,
        Italian,
        Spanish,
        Polish,
        Korean = 128,
        ChineseTraditional = 129,
        ChineseSimplified = 130,
        Japanese = 131
    }
}