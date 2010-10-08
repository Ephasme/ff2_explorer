using Bioware.GFF.Struct;
using Bioware.GFF.Exception;
using Bioware.GFF.Composite;
namespace Bioware.GFF.Struct {
    public abstract class GStruct : GComposite {
        public uint StructType { set; get; }
        public GStruct(string label, uint type)
            : base(label, GType.STRUCT) {
            StructType = type;
        }
    }
    public class GInFieldStruct : GStruct {
        public GInFieldStruct(string label, uint type) : base(label, type) { }
    }
    public class GInListStruct : GInFieldStruct {
        public GInListStruct(uint type) : base(null, type) { }
    }
    public class GRootStruct : GInFieldStruct {

        public const uint ROOT_INDEX = 0;
        public const uint ROOT_TYPE = uint.MaxValue;
        public string Extention { get; set; }
        public GRootStruct(string ext)
            : base(null, ROOT_TYPE) {
            Extention = ext;
        }
    }
}
namespace Bioware.GFF.List {
    public class GList : GComposite {
        public override void Add(GComponent field) {
            if (field is GInListStruct) {
                base.Add(field);
            } else {
                throw new CompositeException(Error.ADD_WRONG_STRUCTURE_CLASS_TO_LIST);
            }
        }
        public GList(string label) : base(label, GType.LIST) { }
    }
}