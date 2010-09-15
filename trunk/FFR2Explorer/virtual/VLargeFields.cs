using System;
using System.Collections.Generic;
using System.Text;

using DWORD = System.UInt32;
using System.IO;
using Bioware;


namespace Bioware.Virtual {
    public class VLargeField<T> : VField {
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
        public T Value { get; set; }
        public VLargeField(string label, VType type) : base(label,type) { }
    }
    public class VDword64 : VLargeField<UInt64> {
        public VDword64(string label, VType type) : base(label,type) { }
    }
    public class VInt64 : VLargeField<Int64> {
        public VInt64(string label, VType type) : base(label,type) { }
    }
    public class VDouble : VLargeField<double> {
        public VDouble(string label, VType type) : base(label,type) { }
    }
    public class VExoString : VLargeField<string> {
        public VExoString(string label, VType type) : base(label,type) { }
    }
    public class VResRef : VLargeField<string> {
        public VResRef(string label, VType type) : base(label,type) { }
    }
    public class VExoLocString : VLargeField<Dictionary<int, string>> {
        public uint StringRef { private set; get; }
        public VExoLocString(string label, uint strRef, VType type)
            : base(label,type) {
            StringRef = strRef;
        }
    }
    public class VVoid : VLargeField<byte[]> {
        public VVoid(string label, VType type) : base(label,type) { }
    }
}
