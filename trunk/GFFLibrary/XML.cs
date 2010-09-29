using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using System.IO;
using GFFLibrary.GFF;
using GFFLibrary.Virtual;

namespace GFFLibrary.XML {
    public class XmlFile {

        public const string EXT = ".xml";

        public const string E_STRUCT = "struct";
        public const string E_LIST = "list";
        public const string E_FIELD = "field";
        public const string E_CHILDS = "childs";
        public const string E_LABEL = "label";
        public const string E_VALUE = "value";

        public const string A_INDEX = "Index";
        public const string A_TYPE = "Type";
        public const string A_CLASS = "Class";
        public const string A_FIELD_FRAME_INDEX = "FieldFrameIndex";
        public const string A_STRUCT_TYPE = "StructType";

        public const string S_CLASS_NORMAL = "normal";
        public const string S_CLASS_LISTED = "listed";
        public const string S_CLASS_ROOT = "root";

        protected VRoot vroot;
        protected XmlNode xroot;
        protected string path;

        protected XmlDocument xdoc;

        public XmlFile() {
        }
        protected void Initialize() {
            xdoc = new XmlDocument();
        }
    }

    public class XmlFileReader : XmlFile {

        public XmlFileReader()
            : base() {
        }

        public void Load(string path) {
            Initialize();
            this.path = path;
            if (File.Exists(path) && Path.GetExtension(path) == EXT) {
                xdoc.Load(path);
            }

            xroot = xdoc.SelectSingleNode("/" + E_STRUCT + "[@" + A_CLASS + "='" + S_CLASS_ROOT + "']");

            VComponent cpnt = CreateComponent(xroot);
            if (cpnt is VRoot) {
                vroot = (VRoot)cpnt;
            } else {
                throw new ApplicationException("Elément racine manquant.");
            }
        }

        public VRoot RootStruct {
            get {
                return vroot;
            }
        }

        public static byte[] StringToByteArray(String hex) {
            int NumberChars = hex.Length;
            if (hex.Length % 2 != 0) {
                throw new ApplicationException("Invalid hexadecimal value.");
            }
            byte[] bytes = new byte[NumberChars / 2];
            for (int i = 0; i < NumberChars; i += 2)
                bytes[i / 2] = Convert.ToByte(hex.Substring(i, 2), 16);
            return bytes;
        }

        private VComponent CreateComponent(XmlNode node) {
            VComponent cpnt = null;
            VLabel cpnt_lbl = GetLabel(node);
            switch (node.Name) {
                case E_STRUCT:
                    switch (GetStructClass(node)) {
                        case S_CLASS_NORMAL:
                            cpnt = new VNormalStruct(cpnt_lbl, GetFieldFrameIndex(node), GetIndex(node), GetStructType(node));
                            break;
                        case S_CLASS_LISTED:
                            cpnt = new VListedStruct(GetIndex(node), GetStructType(node));
                            break;
                        case S_CLASS_ROOT:
                            cpnt = new VRoot();
                            break;
                    }
                    break;
                case E_LIST:
                    cpnt = new VList(cpnt_lbl, GetIndex(node));
                    break;
                case E_FIELD:
                    cpnt = new VField(cpnt_lbl, GetFieldType(node), GetFieldData(node), GetIndex(node));
                    break;
            }
            XmlNode childs = node.SelectSingleNode(E_CHILDS);
            if (childs != null) {
                if (cpnt is VComposite) {
                    VComposite cpsit = (VComposite)cpnt;
                    foreach (XmlNode child in childs.ChildNodes) {
                        cpsit.Add(CreateComponent(child));
                    }
                } else {
                    throw new ApplicationException("Le composant devrait pouvoir accueuillir des éléments enfants.");
                }
            }
            return cpnt;
        }
        private VFieldData GetFieldData(XmlNode node) {
            XmlNode value = node.SelectSingleNode(E_VALUE);
            if (value != null) {
                return new VFieldData(XmlFileReader.StringToByteArray(value.InnerText));
            } else {
                throw new ApplicationException("Impossible de récupérer la valeur de champ.");
            }
        }
        private VType GetFieldType(XmlNode node) {
            if (node.Attributes[A_TYPE] != null) {
                return (VType)Enum.Parse(typeof(VType), node.Attributes[A_TYPE].Value);
            } else {
                throw new ApplicationException("Impossible de récupérer le type de champ.");
            }
        }
        private uint GetStructType(XmlNode node) {
            if (node.Attributes[A_STRUCT_TYPE] != null) {
                return uint.Parse(node.Attributes[A_STRUCT_TYPE].Value);
            } else {
                throw new ApplicationException("Impossible de récupérer le type de la structure.");
            }
        }
        private uint GetIndex(XmlNode node) {
            if (node.Attributes[A_INDEX] != null) {
                return uint.Parse(node.Attributes[A_INDEX].Value);
            } else {
                throw new ApplicationException("Impossible de récupérer l'index.");
            }
        }
        private uint GetFieldFrameIndex(XmlNode node) {
            if (node.Attributes[A_FIELD_FRAME_INDEX] != null) {
                return uint.Parse(node.Attributes[A_FIELD_FRAME_INDEX].Value);
            } else {
                throw new ApplicationException("Impossible de récupérer l'index associé au field.");
            }
        }
        private string GetStructClass(XmlNode node) {
            if (node.Attributes[A_CLASS] != null) {
                return node.Attributes[A_CLASS].Value;
            } else {
                throw new ApplicationException("Impossible de récupérer la classe.");
            }
        }
        private VLabel GetLabel(XmlNode node) {
            XmlNodeList nl_lbl = node.SelectNodes(E_LABEL);
            if (nl_lbl.Count == 0) {
                return VLabel.EMPTY_LABEL;
            } else if (nl_lbl.Count == 1) {
                XmlNode n_lbl = nl_lbl[0];
                string lbl_text = null;
                int lbl_index;
                if (int.TryParse(n_lbl.Attributes[A_INDEX].Value, out lbl_index)) {
                    if (n_lbl.FirstChild != null) {
                        lbl_text = n_lbl.FirstChild.Value;
                        return new VLabel(lbl_text, (uint)lbl_index);
                    } else {
                        throw new ApplicationException("Label sans valeur.");
                    }
                } else {
                    throw new ApplicationException("Index invalide.");
                }
            } else {
                throw new ApplicationException("Nombre de label incorrect.");
            }
        }
    }
    public class XmlFileWriter : XmlFile {

        public XmlFileWriter()
            : base() {
        }

        public void Save(VRoot root, string path) {
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
                WriteStruct(x_child, (VStruct)v_child);
            } else if (v_child is VList) {
                x_child = xdoc.CreateElement(E_LIST);
                WriteList(x_child, v_child);
            } else {
                x_child = xdoc.CreateElement(E_FIELD);
                WriteField(x_child, v_child);
            }

            x_root.AppendChild(x_child);

            if (v_child is VComposite) {
                VComposite cpsit = (VComposite)v_child;

                XmlElement x_childs = xdoc.CreateElement(E_CHILDS);
                x_child.AppendChild(x_childs);

                foreach (VComponent grandson in cpsit.Get()) {
                    Write(x_childs, grandson);
                }
            }
        }

        private void WriteLabel(XmlElement x_child, VComponent v_child) {
            XmlElement x_label = xdoc.CreateElement(E_LABEL);
            x_label.SetAttribute(A_INDEX, v_child.Label.Index.ToString());
            XmlNode x_text = xdoc.CreateTextNode(v_child.Label.Text);
            x_label.AppendChild(x_text);
            x_child.AppendChild(x_label);
        }

        private void WriteStruct(XmlElement n, VStruct s) {
            if (s is VNormalStruct) {
                VNormalStruct ns = (VNormalStruct)s;
                WriteLabel(n, s);
                n.SetAttribute(A_CLASS, S_CLASS_NORMAL);
                n.SetAttribute(A_FIELD_FRAME_INDEX, ns.FieldFrameIndex.ToString());
                n.SetAttribute(A_INDEX, ns.Index.ToString());
                n.SetAttribute(A_STRUCT_TYPE, ns.StructType.ToString());
            } else if (s is VListedStruct) {
                VListedStruct ls = (VListedStruct)s;
                n.SetAttribute(A_CLASS, S_CLASS_LISTED);
                n.SetAttribute(A_INDEX, ls.Index.ToString());
                n.SetAttribute(A_STRUCT_TYPE, ls.StructType.ToString());
            } else if (s is VRoot) {
                n.SetAttribute(A_CLASS, S_CLASS_ROOT);
            }
        }
        private void WriteList(XmlElement n, VComponent c) {
            VList lst = (VList)c;
            WriteLabel(n, c);
            n.SetAttribute(A_INDEX, lst.Index.ToString());
        }
        private void WriteField(XmlElement n, VComponent c) {
            VField fld = (VField)c;
            WriteLabel(n, c);
            n.SetAttribute(A_INDEX, fld.Index.ToString());
            n.SetAttribute(A_TYPE, Enum.GetName(typeof(VType), fld.Type));
            XmlNode x_value = xdoc.CreateElement(E_VALUE);
            n.AppendChild(x_value);
            XmlNode x_value_text = xdoc.CreateTextNode(fld.FieldData.ToHexaString());
            x_value.AppendChild(x_value_text);
        }
    }
}
/*
VComponent v_child = null;
VLabel v_child_lbl = GetLabel(x_root);
switch (x_root.Name) {
    case E_FIELD:
        v_child = new VField(v_child_lbl, GetFieldType(x_root), GetFieldData(x_root), GetFieldIndex(x_root));
        break;
    case E_LIST:
        break;
    case E_STRUCT:
        break;
}
XmlNode x_childs = x_root.SelectSingleNode(E_CHILDS);
if (x_childs.HasChildNodes) {
    foreach (XmlNode node in x_childs.ChildNodes) {
        LoadNode(v_child, node);
    }
}*/
