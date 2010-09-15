using System;
using System.Collections.Generic;
using System.Text;


using DWORD = System.UInt32;
using Bioware.GFF;
using Bioware.Virtual;

namespace Bioware.Virtual {
    abstract public class VSimpleField<T> : VField {
        public T Value { get; set; }
        public VSimpleField(string label, VType type) : base(label, type) { }
    }
    public class VByte : VSimpleField<byte> {
        public VByte(string label, VType type)
            : base(label,type) {
        }
    }
    public class VChar : VSimpleField<char> {
        public VChar(string label, VType type)
            : base(label, type) {
        }
    }
    public class VWord : VSimpleField<UInt16> {
        public VWord(string label, VType type)
            : base(label, type) {
        }
    }
    public class VShort : VSimpleField<short> {
        public VShort(string label, VType type)
            : base(label, type) {
        }
    }
    public class VDword : VSimpleField<uint> {
        public VDword(string label, VType type)
            : base(label, type) {
        }
    }
    public class VInt : VSimpleField<int> {
        public VInt(string label, VType type)
            : base(label, type) {
        }
    }
    public class VFloat : VSimpleField<float> {
        public VFloat(string label, VType type)
            : base(label, type) {
        }
    }

}
