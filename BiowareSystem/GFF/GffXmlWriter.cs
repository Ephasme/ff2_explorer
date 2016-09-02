using System;
using System.IO;
using System.Xml;
using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;

namespace Bioware.GFF
{
    internal class GffXmlWriter : GffXmlBase
    {
        private GffRootStruct _rootStruct;

        public void Save(GffRootStruct root, string path)
        {
            Initialize();
            _rootStruct = root;
            FileStream fs;
            if (File.Exists(path))
            {
                fs = new FileStream(path, FileMode.Truncate);
            }
            else
            {
                fs = new FileStream(path, FileMode.Create);
            }
            Xdoc.AppendChild(Xdoc.CreateXmlDeclaration("1.0", LatinEncoding.Name, "yes"));
            Write(Xdoc, _rootStruct);
            Xdoc.Save(fs);
            fs.Close();
        }

        private void Write(XmlNode xRoot, GffComponent vChild)
        {
            XmlElement xChild;
            if (vChild is GffStruct)
            {
                xChild = Xdoc.CreateElement(EStruct);
            }
            else if (vChild is GffList)
            {
                xChild = Xdoc.CreateElement(EList);
            }
            else if (vChild is GffField)
            {
                xChild = Xdoc.CreateElement(EField);
            }
            else
            {
                throw new ComponentException("Type de composant inconnu.");
            }
            WriteLabel(xChild, vChild);
            var child = vChild as GffStruct;
            if (child != null)
            {
                WriteStruct(xChild, child);
            }
            else if (vChild is GffField)
            {
                WriteField(xChild, vChild);
            }

            xRoot.AppendChild(xChild);

            var composite = vChild as GffComposite;
            if (composite == null) return;
            var cvChild = composite;
            foreach (var vChild2 in cvChild)
            {
                Write(xChild, vChild2);
            }
        }

        private static void WriteStruct(XmlElement n, GffStruct s)
        {
            var @struct = s as GffRootStruct;
            if (@struct != null)
            {
                n.SetAttribute(AExtention, @struct.Extention);
            }
            n.SetAttribute(AType, s.StructType.ToString());
        }

        private void WriteField(XmlElement n, GffComponent c)
        {
            var fld = (GffField) c;
            n.SetAttribute(AType, Enum.GetName(typeof (GffType), fld.Type));
            var data = fld.Value;
            if (data != string.Empty)
            {
                n.AppendChild(Xdoc.CreateTextNode(data));
            }
        }

        private static void WriteLabel(XmlElement n, GffComponent c)
        {
            if (!string.IsNullOrEmpty(c.Label))
            {
                n.SetAttribute(ALabel, c.Label);
            }
        }
    }
}