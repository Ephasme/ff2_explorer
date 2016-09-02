using System.IO;
using Bioware.GFF.Struct;

namespace Bioware.GFF
{
    public class GffDocument
    {
        private readonly GffReader _rd;
        private readonly GffWriter _wr;

        public GffDocument(string path) : this()
        {
            _rd.Load(path);
        }

        public GffDocument(Stream stream) : this()
        {
            _rd.Load(stream);
        }

        public GffDocument()
        {
            var gb = new GffBase();
            _wr = new GffWriter(gb);
            _rd = new GffReader(gb);
        }

        public GffRootStruct RootStruct => _rd.RootStruct;

        public void Save(GffRootStruct root, string path)
        {
            _wr.Save(root, path);
        }

        public Stream Save(GffRootStruct root)
        {
            return _wr.Save(root);
        }

        public void Save(string path)
        {
            _wr.Save(_rd.RootStruct, path);
        }

        public Stream Save()
        {
            return _wr.Save(_rd.RootStruct);
        }

        public void Load(string path)
        {
            _rd.Load(path);
        }

        public void Load(Stream stream)
        {
            _rd.Load(stream);
        }
    }
}