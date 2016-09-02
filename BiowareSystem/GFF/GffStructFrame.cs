namespace Bioware.GFF
{
    public class GffStructFrame : GffBasicFrame
    {
        public GffStructFrame(uint type, uint dataOrDataOffset, uint fieldCount)
        {
            Type = type;
            FieldCount = fieldCount;
            DataOrDataOffset = dataOrDataOffset;
        }

        public GffStructFrame() : this(0, 0, 0)
        {
        }

        public uint FieldCount
        {
            get { return Datas[2]; }
            set { Datas[2] = value; }
        }

        public uint DataOrDataOffset
        {
            get { return Datas[1]; }
            set { Datas[1] = value; }
        }
    }
}