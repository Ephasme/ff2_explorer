using System.IO;

namespace Bioware
{
    public class LatinBinaryWriter : BinaryWriter
    {
        public LatinBinaryWriter(Stream stream) : base(stream, LatinEncoding.Latin1)
        {
        }
    }
}