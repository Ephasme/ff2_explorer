using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;
using Bioware.Virtual;
using System.Xml;

namespace Bioware.XML {

    public class XMLFileSaver {

        public const string E_FIELD = "field";
        public const string E_STRUCT = "struct";
        public const string E_LIST = "list";
        public const string E_STRING = "string";

        public const string A_LABEL = "label";
        public const string A_TYPE = "type";
        public const string A_KEY = "key";

        XmlDocument doc;

        private string path;
        private VStruct v_root;
        private XmlElement x_root;
        private XmlElement x_cur;

        public XMLFileSaver(String path, VStruct root) {
            initialize(path, root);
        }

        private void initialize(String path, VStruct root) {
            this.path = path;
            this.v_root = root;
            doc = new XmlDocument();
            XmlNode xmlnode = doc.CreateNode(XmlNodeType.XmlDeclaration, "", "");
            doc.AppendChild(xmlnode);
        }

        public void save() {
            createFieldElement(v_root);
            doc.Save(path);
        }

        private void createFieldElement(VField fld) {
            XmlElement elmt;
            if (VField.isComposite(fld.Type)) {
                VCpsitField cpsit = (VCpsitField)fld;
                if (cpsit is VStruct) {
                    elmt = doc.CreateElement(E_STRUCT);
                } else {
                    elmt = doc.CreateElement(E_LIST);
                }
                XmlElement parent = x_cur;
                x_cur = elmt;
                foreach (VField child in cpsit.get()) {
                    createFieldElement(child);
                }
                x_cur = parent;
            } else {
                elmt = doc.CreateElement(E_FIELD);
                elmt.SetAttribute(A_TYPE, Enum.GetName(fld.Type.GetType(), fld.Type));
                string value = "";
                #region Switch d'écriture de la valeur.
                switch (fld.Type) {
                    case VType.BYTE:
                        value = ((VByte)fld).Value.ToString();
                        break;
                    case VType.CHAR:
                        value = ((VChar)fld).Value.ToString();
                        break;
                    case VType.WORD:
                        value = ((VWord)fld).Value.ToString();
                        break;
                    case VType.SHORT:
                        value = ((VShort)fld).Value.ToString();
                        break;
                    case VType.DWORD:
                        value = ((VDword)fld).Value.ToString();
                        break;
                    case VType.INT:
                        value = ((VInt)fld).Value.ToString();
                        break;
                    case VType.DWORD64:
                        value = ((VDword64)fld).Value.ToString();
                        break;
                    case VType.INT64:
                        value = ((VInt64)fld).Value.ToString();
                        break;
                    case VType.FLOAT:
                        value = ((VFloat)fld).Value.ToString();
                        break;
                    case VType.DOUBLE:
                        value = ((VDouble)fld).Value.ToString();
                        break;
                    case VType.CEXOSTRING:
                        value = ((VExoString)fld).Value.ToString();
                        break;
                    case VType.RESREF:
                        value = ((VResRef)fld).Value.ToString();
                        break;
                    case VType.CEXOLOCSTRING:
                        Dictionary<int, string> dic = ((VExoLocString)fld).Value;
                        foreach (KeyValuePair<int, string> kvp in dic) {
                            XmlElement lstr_elmt = doc.CreateElement(E_STRING);
                            lstr_elmt.AppendChild(doc.CreateTextNode(kvp.Value));
                            lstr_elmt.SetAttribute(A_KEY, kvp.Key.ToString());
                            elmt.AppendChild(lstr_elmt);
                        }
                        break;
                    case VType.VOID:
                        value = ((VVoid)fld).Value.ToString();
                        break;
                }
                #endregion
                if (value != "") {
                    elmt.AppendChild(doc.CreateTextNode(value));
                }
            }
            if (fld.Label != VStruct.DEFAULT_LABEL) {
                elmt.SetAttribute(A_LABEL, fld.Label);
            }
            if (x_cur == null) {
                doc.AppendChild(elmt);
            } else {
                x_cur.AppendChild(elmt);
            }
        }
    }
}
