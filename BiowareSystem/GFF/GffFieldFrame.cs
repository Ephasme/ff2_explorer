namespace Bioware.GFF
{
    public class GffFieldFrame : GffBasicFrame
    {
        public GffFieldFrame(uint type, uint labelIndex, uint dataOrDataOffset)
        {
            Type = type;
            LabelIndex = labelIndex;
            DataOrDataOffset = dataOrDataOffset;
        }

        public GffFieldFrame() : this(0, 0, 0)
        {
        }

        public uint LabelIndex
        {
            get { return Datas[1]; }
            set { Datas[1] = value; }
        }

        public uint DataOrDataOffset
        {
            get { return Datas[2]; }
            set { Datas[2] = value; }
        }
    }
}