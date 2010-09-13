using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFR2Explorer {
    public abstract class GCompositeField : GField {
        private List<GField> childs;

        public void add(GField field) {
            childs.Add(field);
            field.Owner = this;
        }

        public void remove(GField field) {
            childs.Remove(field);
            field.Owner = null;
        }

        public void removeAll(GField field) {
            foreach (GField f in childs) {
                remove(f);
            }
        }

        public List<GField> get() {
            return childs;
        }

        public GCompositeField(String label)
            : base(label) {
            childs = new List<GField>();
        }
    }
    public sealed class GStruct : GCompositeField {
        public GStruct(String label) : base(label) { }
        public void setLabel(string label) {
            Label = label;
        }
    }
    public sealed class GList : GCompositeField {
        public GList(String label) : base(label) { }
    }
}
