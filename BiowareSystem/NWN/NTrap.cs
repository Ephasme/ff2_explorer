using Bioware.GFF.Struct;

namespace Bioware.NWN
{
    public abstract class NTrap : NTrigger
    {
        protected NTrap(GffStruct gffStr) : base(gffStr)
        {
        }
    }
}