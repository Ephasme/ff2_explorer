using Bioware.GFF.Composite;
using Bioware.GFF.Field;
using Bioware.GFF.List;

namespace Bioware.GFF.Struct
{
    public abstract class GffStruct : GffComposite
    {
        protected GffStruct(string label, uint type) : base(label, GffType.Struct)
        {
            StructType = type;
        }

        public uint StructType { get; }

        public GffField SelectField(string label)
        {
            return SelectComponent(label) as GffField;
        }

        public GffStruct SelectStruct(string label)
        {
            return SelectComponent(label) as GffStruct;
        }

        public GffList SelectList(string label)
        {
            return SelectComponent(label) as GffList;
        }
    }
}