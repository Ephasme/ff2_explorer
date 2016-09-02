namespace Bioware.Erf
{
    public struct ErfKeyFrame
    {
        public readonly ResRef ResRef; // 16 bytes Filename

        public readonly ResType ResType; //16 bit File type
        //uint ResId; // 32 bit Resource ID, starts at 0 and increments
        //ushort Unused; // 16 bit NULLs

        public ErfKeyFrame(ResRef resref, ResType resType)
        {
            ResRef = resref;
            //ResId = res_id;
            ResType = resType;
            //Unused = unused;
        }
    }
}