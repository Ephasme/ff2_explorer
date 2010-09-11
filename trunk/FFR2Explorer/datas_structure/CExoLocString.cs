using System.Collections.Generic;
using System;
namespace FFR2Explorer {
    public class CExoLocString : LargeField<Dictionary<int, string>> {
        public CExoLocString(CompositeField owner, String label) : base(owner, label) { }
    }
}
