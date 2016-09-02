namespace Bioware.GFF
{
    public abstract class GffBasicFrame
    {
        public const int ValueCount = 3;

        public const int Size = ValueCount*sizeof (uint);

        protected GffBasicFrame()
        {
            Datas = new uint[ValueCount];
        }

        public uint Type
        {
            get { return Datas[0]; }
            set { Datas[0] = value; }
        }

        public uint[] Datas { get; }
    }
}