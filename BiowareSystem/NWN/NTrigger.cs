using Bioware.GFF.Struct;

namespace Bioware.NWN
{
    public abstract class NTrigger : NObject
    {
        public const string Ext = ".utw";
        protected readonly GffStruct GffStr;

        protected NTrigger(GffStruct gffStr)
        {
            GffStr = gffStr;
        }
    }
}