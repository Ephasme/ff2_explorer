using System;
using System.Collections.Generic;
using System.Text;

using DWORD = System.UInt32;
using System.IO;
using Bioware;


namespace Bioware.Virtual {
    public abstract class VLargeField<T> : VField {
        public static bool isLargeField(VField fld) {
            VType type = fld.Type;
            switch (type) {
                case VType.DWORD64:
                case VType.INT64:
                case VType.DOUBLE:
                case VType.CEXOSTRING:
                case VType.RESREF:
                case VType.CEXOLOCSTRING:
                case VType.VOID:
                    return true;
                default:
                    return false;
            }
        }
        private T value;
        public T Value {
            get { return value; }
            set {
                if (isValueValid(value)) {
                    this.value = value;
                } else {
                    throw new FieldException(FieldError.VALUE_IS_INVALID);
                }
            }
        }
        virtual public bool isValueValid(T value) {
            return true;
        }
        public VLargeField(string label, VType type, int index) : base(label, type, index) { }
    }
    public class VDword64 : VLargeField<UInt64> {
        public VDword64(string label, int index) : base(label, VType.DWORD64, index) { }
    }
    public class VInt64 : VLargeField<Int64> {
        public VInt64(string label, int index) : base(label, VType.INT64, index) { }
    }
    public class VDouble : VLargeField<double> {
        public VDouble(string label, int index) : base(label, VType.DOUBLE, index) { }
    }
    public class VExoString : VLargeField<string> {
        public const int MAX_LENGTH = 1024;
        public VExoString(string label, string value, int index)
            : this(label, index) {
            Value = value;
        }
        public VExoString(string label, int index)
            : base(label, VType.CEXOSTRING, index) { }
        public override bool isValueValid(string value) {
            return (value.Length <= MAX_LENGTH);
        }
    }
    public class VResRef : VLargeField<string> {
        public const int MAX_LENGTH = 16;
        public VResRef(string label, string value, int index)
            : this(label, index) {
            Value = value;
        }
        public VResRef(string label, int index)
            : base(label, VType.RESREF, index) { }
        public override bool isValueValid(string value) {
            return (value.Length <= MAX_LENGTH);
        }
    }
    public class VExoLocString : VLargeField<Dictionary<int, string>> {
        public const int MAX_LENGTH = 1024;
        public DWORD StringRef { private set; get; }
        public VExoLocString(string label, DWORD strRef, Dictionary<int, string> value, int index)
            : this(label, strRef, index) {
            Value = value;
        }
        public VExoLocString(string label, DWORD strRef, int index)
            : base(label, VType.CEXOLOCSTRING, index) {
            StringRef = strRef;
        }
        public override bool isValueValid(Dictionary<int, string> value) {
            foreach (KeyValuePair<int, string> kvp in value) {
                if (kvp.Value.Length > MAX_LENGTH) {
                    return false;
                }
            }
            return true;
        }
    }
    public class VVoid : VLargeField<byte[]> {
        public VVoid(string label, int index) : base(label, VType.VOID, index) { }
    }
}
