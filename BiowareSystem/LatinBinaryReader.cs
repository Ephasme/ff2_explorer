using System.Collections.Generic;
using System.IO;

namespace Bioware
{
    public class LatinBinaryReader : BinaryReader
    {
        public LatinBinaryReader(Stream stream) : base(stream, LatinEncoding.Latin1)
        {
            Stream = stream;
        }

        public Stream Stream { set; get; }

        public Queue<uint> GetUInt32Queue(int count)
        {
            var q = new Queue<uint>(count);
            for (var i = 0; i < count; i++)
            {
                q.Enqueue(ReadUInt32());
            }
            return q;
        }

        public List<uint> GetUInt32List(int count)
        {
            var l = new List<uint>(count);
            for (var i = 0; i < count; i++)
            {
                l.Add(ReadUInt32());
            }
            return l;
        }

        public Queue<int> GetInt32Queue(int count)
        {
            var q = new Queue<int>(count);
            for (var i = 0; i < count; i++)
            {
                q.Enqueue(ReadInt32());
            }
            return q;
        }

        public List<int> GetInt32List(int count)
        {
            var l = new List<int>(count);
            for (var i = 0; i < count; i++)
            {
                l.Add(ReadInt32());
            }
            return l;
        }
    }
}