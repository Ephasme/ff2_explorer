using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bioware.GFF;

namespace Bioware.Virtual {
    public abstract class VCpsitField : VField {
        private List<VField> childs;
        public void add(VField field) {
            childs.Add(field);
            field.Owner = this;
        }
        public void remove(VField field) {
            childs.Remove(field);
            field.Owner = null;
        }
        public void removeAll(VField field) {
            foreach (VField f in childs) {
                remove(f);
            }
        }
        public List<VField> get() {
            return childs;
        }
        public VCpsitField(String label, VType type)
            : base(label, type) {
            childs = new List<VField>();
        }
    }
    public sealed class VStruct : VCpsitField {
        public const string DEFAULT_LABEL = "struct";
        public VStruct(String label, VType type) : base(label,type) { }
        public void setLabel(string label) {
            Label = label;
        }
    }
    public sealed class VList : VCpsitField {
        public VList(String label, VType type) : base(label,type) { }
    }
}
