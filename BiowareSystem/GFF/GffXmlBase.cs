using System.Xml;

namespace Bioware.GFF
{
    internal class GffXmlBase
    {
        public const string Ext = ".xml";
        public const string EStruct = "struct";
        public const string EList = "list";
        public const string EField = "field";
        public const string ALabel = "label";
        public const string AType = "type";
        public const string AExtention = "ext";
        protected XmlDocument Xdoc;
        protected XmlNode Xroot;

        protected void Initialize()
        {
            Xdoc = new XmlDocument();
        }
    }
}