using System;
using System.Collections.Generic;
using System.Text;

using DWORD = System.UInt32;
using System.IO;


namespace FFR2Explorer {
    public class LargeField<T> : Field {
        public T Value { get; set; }
        public LargeField(CompositeField owner, String label) : base(owner, label) { }
    }
}
