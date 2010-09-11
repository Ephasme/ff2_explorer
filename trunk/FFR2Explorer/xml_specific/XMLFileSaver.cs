using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;

namespace FFR2Explorer {


    public class XMLFileSaver {
        class Scale {
            public string SCALE_STRING { get; private set; }
            private string scale;
            public Scale(string scaleString) { SCALE_STRING = scaleString; }
            public static Scale operator ++(Scale s1) {
                s1.scale += s1.SCALE_STRING;
                return s1;
            }
            public static Scale operator --(Scale s1) {
                s1.scale = s1.scale.Remove(s1.scale.Length - s1.SCALE_STRING.Length);
                return s1;
            }
            public string get() {
                return scale;
            }
        }

        private const string SCALING_STRING = "\t";
        private const string XML_FIELD = "field";
        private const string XML_STRING = "string";
        private const string XML_STRUCT = "struct";
        private const string XML_LIST = "list";

        private const string XML_PAR_KEY = "type";
        private const string XML_PAR_TYPE = "type";
        private const string XML_PAR_LABEL = "label";
        private const string XML_GFFTYPE_CEXOLOCSTRING = "CExoLocString";

        private const string XML_VALUE_NOT_FOUND = "Valeur non trouvée.";
        private const string XML_UNKNOWN_TYPE = "Type inconnu.";

        private const string XML_TYPE_INT = "Int";
        private const string XML_TYPE_DWORD = "Dword";
        private const string XML_TYPE_SHORT = "Short";
        private const string XML_TYPE_CHAR = "Char";
        private const string XML_TYPE_BYTE = "Byte";
        private const string XML_TYPE_DWORD64 = "Dword64";
        private const string XML_TYPE_INT64 = "Int64";
        private const string XML_TYPE_FLOAT = "Float";
        private const string XML_TYPE_DOUBLE = "Double";
        private const string XML_TYPE_CEXOSTRING = "CExoString";
        private const string XML_TYPE_RESREF = "ResRef";
        private const string XML_TYPE_VOID = "Void";

        private FileStream fs;
        private StreamWriter sw;

        private Scale scale = new Scale(SCALING_STRING);

        public Struct RootStruct { get; set; }
        public string SavingPath { get; set; }

        public XMLFileSaver(string path, Struct root) {
            SavingPath = path;
            RootStruct = root;
        }

        public void save() {
            if (File.Exists(SavingPath)) {
                fs = File.Open(SavingPath, FileMode.Truncate);
            } else {
                fs = File.Open(SavingPath, FileMode.OpenOrCreate);
            }
            sw = new StreamWriter(fs);

            writeField(RootStruct);
            sw.Close();
            fs.Close();
        }

        public void writeField(Field field) {
            if (field is Struct) {
                writeStruct((Struct)field);
            } else if (field is List) {
                writeList((List)field);
            } else if (field is CExoLocString) {
                CExoLocString els = (CExoLocString)field;
                writeLine("<"+XML_FIELD+" "+XML_PAR_TYPE+"='"+XML_GFFTYPE_CEXOLOCSTRING+"' "+XML_PAR_LABEL+"='" + field.Label + "'>");
                scale++;
                foreach (KeyValuePair<int, string> kvp in els.Value) {
                    writeLine("<" + XML_STRING + " " + XML_PAR_KEY + "='" + kvp.Key + "'>" + kvp.Value + "</" + XML_STRING + ">");
                }
                scale--;
            } else {
                object value = XML_VALUE_NOT_FOUND;
                string type = XML_UNKNOWN_TYPE;
                if (field is SimpleField<int>) {
                    value = (int)((SimpleField<int>)field).Value;
                    type = XML_TYPE_INT;
                } else if (field is SimpleField<uint>) {
                    value = (uint)((SimpleField<uint>)field).Value;
                    type = XML_TYPE_DWORD;
                } else if (field is SimpleField<short>) {
                    value = (short)((SimpleField<short>)field).Value;
                    type = XML_TYPE_SHORT;
                } else if (field is SimpleField<char>) {
                    value = (char)((SimpleField<char>)field).Value;
                    type = XML_TYPE_CHAR;
                } else if (field is SimpleField<byte>) {
                    value = (byte)((SimpleField<byte>)field).Value;
                    type = XML_TYPE_BYTE;
                } else if (field is LargeField<UInt64>) {
                    value = (UInt64)((LargeField<UInt64>)field).Value;
                    type = XML_TYPE_DWORD64;
                } else if (field is LargeField<Int64>) {
                    value = (Int64)((LargeField<Int64>)field).Value;
                    type = XML_TYPE_INT64;
                } else if (field is SimpleField<float>) {
                    value = (float)((SimpleField<float>)field).Value;
                    type = XML_TYPE_FLOAT;
                } else if (field is LargeField<double>) {
                    value = (double)((LargeField<double>)field).Value;
                    type = XML_TYPE_DOUBLE;
                } else if (field is CExoString) {
                    value = (string)((CExoString)field).Value;
                    type = XML_TYPE_CEXOSTRING;
                } else if (field is ResRef) {
                    value = (string)((ResRef)field).Value;
                    type = XML_TYPE_RESREF;
                } else if (field is LargeField<byte[]>) {
                    value = (byte[])((SimpleField<byte[]>)field).Value;
                    type = XML_TYPE_VOID;
                }
                writeLine("<"+XML_FIELD+" "+XML_PAR_TYPE+"='" + type.ToString() + "' "+XML_PAR_LABEL+"='" + field.Label + "'>" + value.ToString() + "</"+XML_FIELD+">");
            }
        }

        private void writeList(List list) {
            writeLine("<"+XML_LIST+">");
            writeChilds(list);
            writeLine("</"+XML_LIST+">");
        }

        public void writeStruct(Struct str) {
            writeLine("<"+XML_STRUCT+">");
            writeChilds(str);
            writeLine("</"+XML_STRUCT+">");
        }

        private void writeChilds(CompositeField cpsit) {
            scale++;
            foreach (Field fld in cpsit.Childs) {
                writeField(fld);
            }
            scale--;
        }

        private void writeLine(string value) {
            sw.WriteLine(scale.get() + value);
        }
    }
}
