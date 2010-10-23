using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;

namespace Bioware.GFF.Struct {
    public abstract class GStruct : GComposite {
        protected GStruct(string label, uint type) : base(label, GType.Struct) {
            StructType = type;
        }
        public uint StructType { set; get; }
        public GField SelectField(string label) {
            return SelectComponent(label) as GField;
        }
        public GStruct SelectStruct(string label) {
            return SelectComponent(label) as GStruct;
        }
        public GList SelectList(string label) {
            return SelectComponent(label) as GList;
        }
    }
    public class GInFieldStruct : GStruct {
        public GInFieldStruct(string label, uint type) : base(label, type) {}
    }
    public class GInListStruct : GInFieldStruct {
        public GInListStruct(uint type) : base(null, type) {}
    }
    public class GRootStruct : GInFieldStruct {
        public const uint ROOT_INDEX = 0;
        public const uint ROOT_TYPE = uint.MaxValue;
        public GRootStruct(string ext) : base(null, ROOT_TYPE) {
            Extention = ext;
        }
        public string Extention { get; set; }
    }
}
namespace Bioware.GFF.List {
    public class GList : GComposite {
        public GList(string label) : base(label, GType.List) {}
        public new void Add(GComponent field) {
            if (field is GInListStruct) {
                base.Add(field);
            } else {
                throw new CompositeException(Error.AddWrongStructureClassToList);
            }
        }
    }
}