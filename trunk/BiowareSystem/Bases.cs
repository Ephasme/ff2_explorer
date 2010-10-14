using System.IO;
using System.Collections.Generic;
using System.Text;
using System;
namespace Bioware {
    public class ResRef {
        public const int LENGTH = 16;
        string value;
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
            ResRef rr = obj as ResRef;
            if (rr != null) {
                return (rr.value == value);
            }
            return false;
        }
        public override int GetHashCode() {
            return BitConverter.ToInt32(Encoding.ASCII.GetBytes(this.value), 0);
        }
        public override string ToString() {
            return value;
        }
        public char[] CharTable {
            get {
                return value.PadRight(LENGTH, '\0').ToCharArray();
            }
            set {
                test_length(value);
                this.value = new string(value).TrimEnd('\0').Trim().ToLower();
            }
        }
        public ResRef(char[] res_ref) {
            CharTable = res_ref;
        }
        public ResRef(string res_ref) {
            test_length(res_ref.ToCharArray());
            this.value = res_ref;
        }
        private void test_length(char[] resref) {
            if (resref.Length > LENGTH) {
                throw new ApplicationException("ResRef trop long.");
            }
        }
    }
    public enum ResType : ushort {
        bmp = 1,
        tga = 3,
        wav = 4,
        plt = 6,
        ini = 7,
        txt = 10,
        mdl = 2002,
        nss = 2009,
        ncs = 2010,
        are = 2012,
        set = 2013,
        ifo = 2014,
        bic = 2015,
        wok = 2016,
        dda = 2017,
        txi = 2022,
        git = 2023,
        uti = 2025,
        utc = 2027,
        dlg = 2029,
        itp = 2030,
        utt = 2032,
        dds = 2033,
        uts = 2035,
        ltr = 2036,
        gff = 2037,
        fac = 2038,
        ute = 2040,
        utd = 2042,
        utp = 2044,
        dft = 2045,
        gic = 2046,
        gui = 2047,
        utm = 2051,
        dwk = 2052,
        pwk = 2053,
        jrl = 2056,
        utw = 2058,
        ssf = 2060,
        ndb = 2064,
        ptm = 2065,
        ptt = 2066
    }
    public class BinaryReader : System.IO.BinaryReader {
        public Stream Stream { set; get; }
        public BinaryReader(Stream stream)
            : base(stream, LatinEncoding.LATIN1) {
            Stream = stream;
        }
        public Queue<uint> GetUInt32Queue(int count) {
            Queue<uint> q = new Queue<uint>(count);
            for (int i = 0; i < count; i++) {
                q.Enqueue(ReadUInt32());
            }
            return q;
        }
        public List<uint> GetUInt32List(int count) {
            List<uint> l = new List<uint>(count);
            for (int i = 0; i < count; i++) {
                l.Add(ReadUInt32());
            }
            return l;
        }
        public Queue<int> GetInt32Queue(int count) {
            Queue<int> q = new Queue<int>(count);
            for (int i = 0; i < count; i++) {
                q.Enqueue(ReadInt32());
            }
            return q;
        }
        public List<int> GetInt32List(int count) {
            List<int> l = new List<int>(count);
            for (int i = 0; i < count; i++) {
                l.Add(ReadInt32());
            }
            return l;
        }
    }
    public class BinaryWriter : System.IO.BinaryWriter {
        public BinaryWriter(Stream stream)
            : base(stream, LatinEncoding.LATIN1) {
        }
    }
    public static class LatinEncoding {
        public const string NAME = "ISO-8859-1";
        public static Encoding LATIN1 = Encoding.GetEncoding(NAME);
    }
    public enum Gender : uint {
        Male = 0, None = 0, Female = 1
    }
    public enum Lang : uint {
        English, French, German, Italian, Spanish, Polish,
        Korean = 128, ChineseTraditional = 129, ChineseSimplified = 130, Japanese = 131
    }
}
