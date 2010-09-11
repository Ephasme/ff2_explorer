using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFR2Explorer {
    public abstract class CompositeField : Field {
        public List<Field> Childs { get; private set; }

        public CompositeField(CompositeField owner, String label)
            : base(owner, label) {
            Childs = new List<Field>();
        }
    }
}
