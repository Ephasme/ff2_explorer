using System;
using System.IO;
using System.Xml;
using Bioware.GFF.Composite;
using Bioware.GFF.Exception;
using Bioware.GFF.Field;
using Bioware.GFF.List;
using Bioware.GFF.Struct;
using Bioware.GFF.XML.Exception;

namespace Bioware.GFF.XML {
    public class GXmlDocument {
        public GRootStruct RootStruct { get { return xread.RootStruct; } }
        GXmlReader xread;
        GXmlWriter xwrite;
        public GXmlDocument() {
            xread = new GXmlReader();
            xwrite = new GXmlWriter();
        }
        public GXmlDocument(string path)
            : this() {
            Load(path);
        }
        public void Load(string path) {
            xread.Load(path);
        }
        public void Save(GRootStruct root, string path) {
            xwrite.Save(root, path);
        }
    }
    class GXmlReader : GXmlBase {

        public GRootStruct RootStruct { get; private set; }

        public const int SCLASS_IN_FIELD = 0;
        public const int SCLASS_IN_LIST = 1;
        public const int SCLASS_ROOT = 2;

        public GXmlReader() { }

        public void Load(string path) {
            Initialize();
            this.path = path;
            if (File.Exists(path) && Path.GetExtension(path) == EXT) {
                xdoc.Load(path);
            }
            xroot = xdoc.SelectSingleNode("/" + E_STRUCT);
            RootStruct = (GRootStruct)CreateComponent(xroot);
        }
        string GetLabel(XmlNode node) {
            if (node.Attributes[A_LABEL] != null) {
                return node.Attributes[A_LABEL].Value;
            } else {
                throw new FileException(Bioware.GFF.XML.Exception.Error.CAN_NOT_GET_LABEL);
            }
        }
        uint GetStructType(XmlNode node) {
            if (node.Attributes[A_TYPE] != null) {
                return uint.Parse(node.Attributes[A_TYPE].Value);
            } else {
                throw new FileException(Bioware.GFF.XML.Exception.Error.CAN_NOT_GET_STRUCT_TYPE);
            }
        }
        int GetStructClass(XmlNode node) {
            XmlNode parent = node.ParentNode;
            if (parent != xdoc) {
                if (parent.Name == E_LIST) {
                    return SCLASS_IN_LIST;
                } else {
                    return SCLASS_IN_FIELD;
                }
            } else {
                return SCLASS_ROOT;
            }
        }
        GType GetFieldType(XmlNode node) {
            if (node.Attributes[A_TYPE] != null) {
                return (GType)Enum.Parse(typeof(GType), node.Attributes[A_TYPE].Value);
            } else {
                throw new FileException(Bioware.GFF.XML.Exception.Error.CAN_NOT_GET_FIELD_TYPE);
            }
        }
        GComponent CreateComponent(XmlNode node) {
            GComponent cpnt = null;
            switch (node.Name) {
                case E_STRUCT:
                    switch (GetStructClass(node)) {
                        case SCLASS_IN_FIELD:
                            cpnt = new GInFieldStruct(GetLabel(node), GetStructType(node));
                            break;
                        case SCLASS_IN_LIST:
                            cpnt = new GInListStruct(GetStructType(node));
                            break;
                        case SCLASS_ROOT:
                            cpnt = new GRootStruct(GetExtention(node));
                            break;
                    }
                    break;
                case E_LIST:
                    cpnt = new GList(GetLabel(node));
                    break;
                case E_FIELD:
                    cpnt = new GField(GetLabel(node), GetFieldType(node), GetFieldValue(node));
                    break;
            }
            if (cpnt is GComposite) {
                GComposite cpsit = (GComposite)cpnt;
                foreach (XmlNode child in node.ChildNodes) {
                    cpsit.Add(CreateComponent(child));
                }
            }
            return cpnt;
        }

        private string GetFieldValue(XmlNode node) {
            return node.InnerText;
        }

        private string GetExtention(XmlNode node) {
            if (node.Attributes[A_EXTENTION] != null) {
                return node.Attributes[A_EXTENTION].Value;
            } else {
                throw new FileException(Bioware.GFF.XML.Exception.Error.CAN_NOT_GET_EXTENTION);
            }

        }
    }
    class GXmlWriter : GXmlBase {

        GRootStruct RootStruct;

        public GXmlWriter() { }

        public void Save(GRootStruct root, string path) {
            Initialize();
            this.path = path;
            RootStruct = root;
            FileStream fs;
            if (File.Exists(path)) {
                fs = new FileStream(path, FileMode.Truncate);
            } else {
                fs = new FileStream(path, FileMode.Create);
            }
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", LatinEncoding.NAME, "yes"));
            Write(xdoc, RootStruct);
            xdoc.Save(fs);
            fs.Close();
        }

        void Write(XmlNode x_root, GComponent v_child) {
            XmlElement x_child = null;
            if (v_child is GStruct) {
                x_child = xdoc.CreateElement(E_STRUCT);
            } else if (v_child is GList) {
                x_child = xdoc.CreateElement(E_LIST);
            } else if (v_child is GField) {
                x_child = xdoc.CreateElement(E_FIELD);
            } else {
                throw new ComponentException(Bioware.GFF.Exception.Error.UNKNOWN_COMPONENT_TYPE);
            }
            WriteLabel(x_child, v_child);
            if (v_child is GStruct) {
                WriteStruct(x_child, (GStruct)v_child);
            } else if (v_child is GField) {
                WriteField(x_child, v_child);
            }

            x_root.AppendChild(x_child);

            if (v_child is GComposite) {
                GComposite cv_child = (GComposite)v_child;
                foreach (GComponent v_child2 in cv_child) {
                    Write(x_child, v_child2);
                }
            }
        }
        void WriteStruct(XmlElement n, GStruct s) {
            if (s is GRootStruct) {
                n.SetAttribute(A_EXTENTION, ((GRootStruct)s).Extention);
            }
            n.SetAttribute(A_TYPE, s.StructType.ToString());
        }
        void WriteField(XmlElement n, GComponent c) {
            GField fld = (GField)c;
            n.SetAttribute(A_TYPE, Enum.GetName(typeof(GType), fld.Type));
            string data = fld.Value;
            if (data != String.Empty) {
                n.AppendChild(xdoc.CreateTextNode(data));
            }
        }
        void WriteLabel(XmlElement n, GComponent c) {
            if (c.Label != null && c.Label != String.Empty) {
                n.SetAttribute(A_LABEL, c.Label);
            }
        }
    }
    class GXmlBase {
        public const string EXT = ".xml";
        public const string E_STRUCT = "struct";
        public const string E_LIST = "list";
        public const string E_FIELD = "field";
        public const string A_LABEL = "label";
        public const string A_TYPE = "type";
        public const string A_EXTENTION = "ext";
        protected XmlDocument xdoc;
        protected XmlNode xroot;
        protected string path;
        protected void Initialize() {
            xdoc = new XmlDocument();
        }
        public GXmlBase() {
        }
    }
}
