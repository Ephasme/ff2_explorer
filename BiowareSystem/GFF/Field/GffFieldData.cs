using System.IO;

namespace Bioware.GFF.Field
{
    internal class GffFieldData : MemoryStream
    {
        public GffFieldData()
        {
        }

        public GffFieldData(byte[] buffer) : base(buffer)
        {
        }

        public GffFieldData(string value) : base(HexaManip.StringToByteArray(value))
        {
        }

        public override string ToString()
        {
            return HexaManip.ByteArrayToString(ToArray());
        }
    }
}