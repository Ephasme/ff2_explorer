using System;
using System.Collections.Generic;
using System.Text;


using DWORD = System.UInt32;

namespace FFR2Explorer {
    public class GSimpleField<T> : GField {
        public T Value { get; set; }
        public GSimpleField(string label) : base(label) { }
    }
    public class GByte : GSimpleField<byte> {
        public GByte(string label)
            : base(label) {
        }
    }
    public class GChar : GSimpleField<char> {
        public GChar(string label)
            : base(label) {
        }
    }
    public class GWord : GSimpleField<UInt16> {
        public GWord(string label)
            : base(label) {
        }
    }
    public class GShort : GSimpleField<short> {
        public GShort(string label)
            : base(label) {
        }
    }
    public class GDword : GSimpleField<uint> {
        public GDword(string label)
            : base(label) {
        }
    }
    public class GInt : GSimpleField<int> {
        public GInt(string label)
            : base(label) {
        }
    }
    public class GFloat : GSimpleField<float> {
        public GFloat(string label)
            : base(label) {
        }
    }

}
