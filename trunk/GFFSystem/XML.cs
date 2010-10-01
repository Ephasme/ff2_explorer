using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using GFFSystem.Virtual;
using GFFSystem.Exception;
using GFFSystem.GFF;
using Tools;

namespace GFFSystem.XML {
    public abstract class XFile {
        public const string EXT = ".xml";
        public const string E_STRUCT = "struct";
        public const string E_LIST = "list";
        public const string E_FIELD = "field";
        public const string A_LABEL = "label";
        public const string A_TYPE = "type";
        protected VRootStruct vroot;
        protected XmlDocument xdoc;
        protected XmlNode xroot;
        protected string path;
        protected void Initialize() {
            xdoc = new XmlDocument();
        }
    }
    public class XFileReader : XFile {

        public const int SCLASS_IN_FIELD = 0;
        public const int SCLASS_IN_LIST = 1;
        public const int SCLASS_ROOT = 2;

        public XFileReader() { }

        public void Load(string path) {
            Initialize();
            this.path = path;
            if (File.Exists(path) && Path.GetExtension(path) == EXT) {
                xdoc.Load(path);
            }
            xroot = xdoc.SelectSingleNode("/"+E_STRUCT);
            vroot = (VRootStruct)CreateComponent(xroot);
        }
        public VRootStruct RootStruct {
            get {
                return vroot;
            }
        }
        private string GetLabel(XmlNode node) {
            if (node.Attributes[A_LABEL] != null) {
                return node.Attributes[A_LABEL].Value;
            } else {
                throw new XFileException(XError.CAN_NOT_GET_LABEL);
            }
        }
        private uint GetStructType(XmlNode node) {
            if (node.Attributes[A_TYPE] != null) {
                return uint.Parse(node.Attributes[A_TYPE].Value);
            } else {
                throw new XFileException(XError.CAN_NOT_GET_STRUCT_TYPE);
            }
        }
        private int GetStructClass(XmlNode node) {
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
        private VType GetFieldType(XmlNode node) {
            if (node.Attributes[A_TYPE] != null) {
                return (VType)Enum.Parse(typeof(VType), node.Attributes[A_TYPE].Value);
            } else {
                throw new XFileException(XError.CAN_NOT_GET_FIELD_TYPE);
            }
        }
        private VFieldData GetFieldData(XmlNode node) {
            return new VFieldData(HexaManip.StringToByteArray(node.InnerText));
        }
        private VComponent CreateComponent(XmlNode node) {
            VComponent cpnt = null;
            switch (node.Name) {
                case E_STRUCT:
                    switch (GetStructClass(node)) {
                        case SCLASS_IN_FIELD:
                            cpnt = new VInFieldStruct(GetLabel(node), GetStructType(node));
                            break;
                        case SCLASS_IN_LIST:
                            cpnt = new VInListStruct(GetStructType(node));
                            break;
                        case SCLASS_ROOT:
                            cpnt = new VRootStruct();
                            break;
                    }
                    break;
                case E_LIST:
                    cpnt = new VList(GetLabel(node));
                    break;
                case E_FIELD:
                    cpnt = new VField(GetLabel(node), GetFieldType(node), GetFieldData(node));
                    break;
            }
            if (cpnt is VComposite) {
                VComposite cpsit = (VComposite)cpnt;
                foreach (XmlNode child in node.ChildNodes) {
                    cpsit.Add(CreateComponent(child));
                }
            }
            return cpnt;
        }
    }
    public class XFileWriter : XFile {

        public XFileWriter() { }

        public void Save(VRootStruct root, string path) {
            Initialize();
            this.path = path;
            this.vroot = root;
            FileStream fs;
            if (File.Exists(path)) {
                fs = new FileStream(path, FileMode.Truncate);
            } else {
                fs = new FileStream(path, FileMode.Create);
            }
            xdoc.AppendChild(xdoc.CreateXmlDeclaration("1.0", GConst.ENCODING_NAME, "yes"));
            Write(xdoc, vroot);
            xdoc.Save(fs);
            fs.Close();
        }

        private void Write(XmlNode x_root, VComponent v_child) {
            XmlElement x_child = null;
            if (v_child is VStruct) {
                x_child = xdoc.CreateElement(E_STRUCT);
            } else if (v_child is VList) {
                x_child = xdoc.CreateElement(E_LIST);
            } else if (v_child is VField) {
                x_child = xdoc.CreateElement(E_FIELD);
            } else {
                throw new ComponentException(GError.UNKNOWN_COMPONENT_TYPE);
            }
            WriteLabel(x_child, v_child);
            if (v_child is VStruct) {
                WriteStruct(x_child, (VStruct)v_child);
            } else if (v_child is VField) {
                WriteField(x_child, v_child);
            }

            x_root.AppendChild(x_child);

            if (v_child is VComposite) {
                VComposite cv_child = (VComposite)v_child;
                foreach (VComponent v_child2 in cv_child.Get()) {
                    Write(x_child, v_child2);
                }
            }
        }
        private void WriteStruct(XmlElement n, VStruct s) {
            n.SetAttribute(A_TYPE, s.StructType.ToString());
        }
        private void WriteField(XmlElement n, VComponent c) {
            VField fld = (VField)c;
            n.SetAttribute(A_TYPE, Enum.GetName(typeof(VType), fld.Type));
            string data = fld.FieldData.ToHexaString();
            if (data != String.Empty) {
                n.AppendChild(xdoc.CreateTextNode(data));
            }
        }
        private void WriteLabel(XmlElement n, VComponent c) {
            if (c.Label != null && c.Label != String.Empty) {
                n.SetAttribute(A_LABEL, c.Label);
            }
        }
    }
}