using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using DWORD = System.UInt32;
using System.IO;

namespace FFR2Explorer {
    public abstract class GField {

        public String Label { get; set; }
        public GCompositeField Owner { get; set; }

        public bool IsRoot {
            get {
                return (Owner == null);
            }
            private set { }
        }

        public GField(String label) {
            Owner = null;
            Label = label;
        }
    }
}

