using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using DWORD = System.UInt32;
using System.IO;

namespace FFR2Explorer {
    public abstract class Field {

        public String Label { get; set; }
        public CompositeField Owner { get; set; }

        public Field(CompositeField owner, String label) {
            Label = label;
            Owner = owner;
        }
    }
}

