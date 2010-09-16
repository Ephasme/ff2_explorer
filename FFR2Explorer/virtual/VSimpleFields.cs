using System;
using System.Collections.Generic;
using System.Text;


using DWORD = System.UInt32;
using Bioware.GFF;
using Bioware.Virtual;

namespace Bioware.Virtual {
    abstract public class VSimpleField<T> : VField {
        public T Value { get; set; }
        public VSimpleField(string label, VType type, int index) : base(label, type, index) { }
    }
    public class VByte : VSimpleField<byte> {
        public VByte(string label, byte value, int index)
            : this(label, index) {
            Value = value;
        }
        public VByte(string label, int index)
            : base(label, VType.BYTE, index) {
        }
    }
    public class VChar : VSimpleField<char> {
        public VChar(string label, int index)
            : base(label, VType.CHAR, index) {
        }
    }
    public class VWord : VSimpleField<UInt16> {
        public VWord(string label, int index)
            : base(label, VType.WORD, index) {
        }
    }
    public class VShort : VSimpleField<short> {
        public VShort(string label, int index)
            : base(label, VType.SHORT, index) {
        }
    }
    public class VDword : VSimpleField<DWORD> {
        public VDword(string label, int index)
            : base(label, VType.DWORD, index) {
        }
    }
    public class VInt : VSimpleField<int> {
        public VInt(string label, int index)
            : base(label, VType.INT, index) {
        }
    }
    public class VFloat : VSimpleField<float> {
        public VFloat(string label, int index)
            : base(label, VType.FLOAT, index) {
        }
    }

}
