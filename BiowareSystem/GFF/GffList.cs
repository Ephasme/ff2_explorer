using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Struct;

namespace Bioware.GFF.List
{
    public class GffList : GffComposite
    {
        public GffList(string label) : base(label, GffType.List)
        {
        }

        public new void Add(GffComponent field)
        {
            if (field is GffInListStruct)
            {
                base.Add(field);
            }
            else
            {
                throw new CompositeException(ErrorLabels.AddWrongStructureClassToList);
            }
        }
    }
}