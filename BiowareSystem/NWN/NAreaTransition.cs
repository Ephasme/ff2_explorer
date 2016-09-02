using Bioware.GFF.Struct;

namespace Bioware.NWN
{
    public abstract class NAreaTransition : NTrigger
    {
        protected NAreaTransition(GffStruct gffStr) : base(gffStr)
        {
        }
    }
}