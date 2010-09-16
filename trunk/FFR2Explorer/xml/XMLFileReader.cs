using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bioware.Virtual;
using System.IO;
using System.Xml;

using WORD = System.UInt16;
using DWORD = System.UInt32;
using DWORD64 = System.UInt64;

namespace Bioware.XML {
    public class XmlFileReader : IFileReader {

        XmlDocument doc;
        VStruct v_root;

        public const string XML_EXT = ".xml";

        public XmlFileReader(String path) {
            initialize(path);
        }

        private VField createField(XmlNode node) {
            int index = getIndex(node);
            string sLabel = getLabel(node);
            // On récupère le type de noeud.
            string nName = node.Name;
            switch (nName) {
                case XmlField.E_LIST:
                    return new VList(sLabel, index);
                case XmlField.E_STRUCT:
                    return new VStruct(sLabel, index);
                case XmlField.E_FIELD:
                    string sValue = getValue(node);
                    switch (getVType(node)) {
                        case VType.BYTE:
                            byte bVal;
                            if (byte.TryParse(sValue, out bVal)) {
                                return new VByte(sLabel, bVal);
                            }
                            break;
                        case VType.CEXOSTRING:
                            return new VExoString(sLabel, sValue, index);
                        case VType.CEXOLOCSTRING:
                            uint strRef = uint.Parse(node.Attributes[XmlAttrib.A_STRREF].Value);
                            Dictionary<int, string> dicVal = new Dictionary<int, string>();
                            XmlNodeList childs = node.ChildNodes;
                            foreach (XmlNode child in childs) {
                                string str = XmlConst.EMPTY_STRING;
                                if (child.FirstChild != null) {
                                    str = child.FirstChild.Value;
                                }
                                int key = int.Parse(child.Attributes[XmlAttrib.A_KEY].Value);
                                dicVal.Add(key, str);
                            }
                            VExoLocString exl = new VExoLocString(sLabel, strRef, dicVal, index);
                            return exl;
                        case VType.CHAR:
                            char cVal;
                            VChar vChar = new VChar(sLabel, index);
                            if (char.TryParse(sValue, out cVal)) {
                                vChar.Value = cVal;
                                return vChar;
                            }
                            break;
                        case VType.DOUBLE:
                            double dVal;
                            VDouble vDouble = new VDouble(sLabel, index);
                            if (double.TryParse(sValue, out dVal)) {
                                vDouble.Value = dVal;
                                return vDouble;
                            }
                            break;
                        case VType.DWORD:
                            DWORD dwVal;
                            VDword vDword = new VDword(sLabel, index);
                            if (DWORD.TryParse(sValue, out dwVal)) {
                                vDword.Value = dwVal;
                                return vDword;
                            }
                            break;
                        case VType.DWORD64:
                            DWORD64 dw64Val;
                            VDword64 vDword64 = new VDword64(sLabel, index);
                            if (DWORD64.TryParse(sValue, out dw64Val)) {
                                vDword64.Value = dw64Val;
                                return vDword64;
                            }
                            break;
                        case VType.FLOAT:
                            float fVal;
                            VFloat vFloat = new VFloat(sLabel, index);
                            if (float.TryParse(sValue, out fVal)) {
                                vFloat.Value = fVal;
                                return vFloat;
                            }
                            break;
                        case VType.INT:
                            int iVal;
                            VInt vInt = new VInt(sLabel, index);
                            if (int.TryParse(sValue, out iVal)) {
                                vInt.Value = iVal;
                                return vInt;
                            }
                            break;
                        case VType.INT64:
                            Int64 i64Val;
                            VInt64 vInt64 = new VInt64(sLabel, index);
                            if (Int64.TryParse(sValue, out i64Val)) {
                                vInt64.Value = i64Val;
                                return vInt64;
                            }
                            break;
                        case VType.RESREF:
                            VResRef vResRef = new VResRef(sLabel, index);
                            vResRef.Value = sValue;
                            return vResRef;
                        case VType.SHORT:
                            short shVal;
                            VShort vShort = new VShort(sLabel, index);
                            if (short.TryParse(sValue, out shVal)) {
                                vShort.Value = shVal;
                                return vShort;
                            }
                            break;
                        case VType.VOID:
                            List<byte> vVal = new List<byte>();
                            VVoid vVoid = new VVoid(sLabel, index);
                            char[] charArray = sValue.ToCharArray();
                            foreach (char c in charArray) {
                                byte b;
                                if (byte.TryParse(c.ToString(), out b)) {
                                    vVal.Add(b);
                                } else {
                                    break;
                                }
                            }
                            vVoid.Value = vVal.ToArray();
                            return vVoid;
                        case VType.WORD:
                            WORD wVal;
                            VWord vWord = new VWord(sLabel, index);
                            if (WORD.TryParse(sValue, out wVal)) {
                                vWord.Value = wVal;
                                return vWord;
                            }
                            break;
                    }
                    break;
            }
            return null;
        }

        private int getIndex(XmlNode node) {
            return int.Parse(node.Attributes[XmlAttrib.A_INDEX].Value);
        }

        private VType getVType(XmlNode node) {
            string sType = node.Attributes[XmlAttrib.A_TYPE].Value;
            return (VType)Enum.Parse(typeof(VType), sType);
        }

        private string getValue(XmlNode node) {
            if (node.HasChildNodes) {
                return node.FirstChild.Value;
            }
            return XmlConst.EMPTY_STRING;
        }

        private string getLabel(XmlNode node) {
            XmlAttribute attr = node.Attributes[XmlAttrib.A_LABEL];
            string result = VStruct.DEFAULT_LABEL;
            if (attr != null) {
                result = attr.Value;
            }
            return result;
        }

        private void loadVDatas(VCpsitField vRoot, XmlNodeList xNodes) {
            if (xNodes != null) {
                foreach (XmlNode xNode in xNodes) {
                    VField vChild = createField(xNode);
                    if (vChild is VCpsitField) {
                        loadVDatas((VCpsitField)vChild, xNode.ChildNodes);
                    }
                    vRoot.add(vChild);
                }
            }
        }

        private void initialize(string path) {
            doc = new XmlDocument();
            v_root = new VStruct(VStruct.DEFAULT_LABEL, 0);
            doc.Load(path);
        }
        public VStruct getRootStruct() {
            XmlNode x_root = doc.SelectSingleNode("/" + XmlField.E_STRUCT);
            loadVDatas(v_root, x_root.ChildNodes);
            return v_root;
        }
    }
}