using System;
using System.IO;

namespace Bioware
{
    public class ContentObject
    {
        private readonly uint _offset;
        private readonly uint _size;

        public ContentObject(string path, ResRef resref, ResType restype, uint offset, uint size)
        {
            ResRef = resref;
            ResType = restype;
            FilePath = path;
            _offset = offset;
            _size = size;
        }

        public ContentObject(string path)
        {
            FileName = Path.GetFileName(path);
            FilePath = path;
            _offset = 0;
            _size = (uint) File.OpenRead(path).Length;
        }

        public MemoryStream DataStream
        {
            get
            {
                var br = new LatinBinaryReader(File.OpenRead(FilePath)) {Stream = {Position = _offset}};
                return new MemoryStream(br.ReadBytes((int) _size));
            }
        }

        public ResRef ResRef { get; private set; }
        public ResType ResType { get; private set; }
        public string FilePath { get; set; }

        public string FileName
        {
            get { return ResRef + "." + Enum.GetName(typeof (ResType), ResType); }
            set
            {
                var ext = Path.GetExtension(value);
                if (ext == null)
                {
                    throw new NullReferenceException();
                }
                ext = ext.Substring(1, 1).ToUpper() + ext.Substring(2);
                ResType = (ResType) Enum.Parse(typeof (ResType), ext.TrimStart('.').Trim());
                ResRef = new ResRef(Path.GetFileNameWithoutExtension(value));
            }
        }
    }
}