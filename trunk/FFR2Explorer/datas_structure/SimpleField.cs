using System;
using System.Collections.Generic;
using System.Text;


using DWORD = System.UInt32;

namespace FFR2Explorer {
    public class SimpleField<T> : Field {
        public T Value { get; set; }
        public SimpleField(CompositeField owner, String label) : base(owner, label) { }
    }
}
