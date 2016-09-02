namespace Bioware.Key
{
    public class BiffResource
    {
        public BiffResource(uint offset, uint size, ResType resType)
        {
            Offset = offset;
            Size = size;
            ResType = resType;
        }

        public uint Offset { get; private set; }
        public uint Size { get; private set; }
        public ResType ResType { get; private set; } //DWORD Resource type of this resource
    }
}