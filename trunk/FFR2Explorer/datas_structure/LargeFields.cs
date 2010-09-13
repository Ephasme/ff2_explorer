using System;
using System.Collections.Generic;
using System.Text;

using DWORD = System.UInt32;
using System.IO;


namespace FFR2Explorer {
    public class GLargeField<T> : GField {
        public T Value { get; set; }
        public GLargeField(string label) : base(label) { }
    }
    public class GDword64 : GLargeField<UInt64> {
        public GDword64(string label) : base(label) { }
    }
    public class GInt64 : GLargeField<Int64> {
        public GInt64(string label) : base(label) { }
    }
    public class GDouble : GLargeField<double> {
        public GDouble(string label) : base(label) { }
    }
    public class GCExoString : GLargeField<string> {
        public GCExoString(string label) : base(label) { }
    }
    public class GResRef : GLargeField<string> {
        public GResRef(string label) : base(label) { }
    }
    public class GCExoLocString : GLargeField<Dictionary<int, string>> {
        public uint StringRef { private set; get; }
        public GCExoLocString(string label, uint strRef) : base(label) {
            StringRef = strRef;
        }
    }
    public class GVoid : GLargeField<byte[]> {
        public GVoid(string label) : base(label) { }
    }
}
