using Bioware.GFF.Struct;

namespace Bioware.GFF
{
    public class GffXmlDocument
    {
        private readonly GffXmlReader _xread;
        private readonly GffXmlWriter _xwrite;

        public GffXmlDocument()
        {
            _xread = new GffXmlReader();
            _xwrite = new GffXmlWriter();
        }

        public GffXmlDocument(string path) : this()
        {
            Load(path);
        }

        public GffRootStruct RootStruct => _xread.RootStruct;

        public void Load(string path)
        {
            _xread.Load(path);
        }

        public void Save(GffRootStruct root, string path)
        {
            _xwrite.Save(root, path);
        }
    }
}