namespace Bioware.Erf
{
    public struct ErfResFrame
    {
        public readonly uint OffsetToResource; // 32 bit offset to file data from beginning of ERF

        public readonly uint ResourceSize; // 32 bit number of bytes

        public ErfResFrame(uint offsetToRes, uint resSize)
        {
            OffsetToResource = offsetToRes;
            ResourceSize = resSize;
        }
    }
}