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
        private const string XML_GFFTYPE_CEXOLOCSTRING = "GCExoLocString";

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
        private const string XML_TYPE_CEXOSTRING = "GCExoString";
        private const string XML_TYPE_RESREF = "GResRef";
        private const string XML_TYPE_VOID = "Void";

        private FileStream fs;
        private StreamWriter sw;

        private Scale scale = new Scale(SCALING_STRING);

        public GStruct RootStruct { get; set; }
        public string SavingPath { get; set; }

        public XMLFileSaver(string path, GStruct root) {
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

        public void writeField(GField field) {
            if (field is GStruct) {
                writeStruct((GStruct)field);
            } else if (field is GList) {
                writeList((GList)field);
            } else if (field is GCExoLocString) {
                GCExoLocString els = (GCExoLocString)field;
                writeLine("<" + XML_FIELD + " " + XML_PAR_TYPE + "='" + XML_GFFTYPE_CEXOLOCSTRING + "' " + XML_PAR_LABEL + "='" + field.Label + "'>");
                scale++;
                foreach (KeyValuePair<int, string> kvp in els.Value) {
                    writeLine("<" + XML_STRING + " " + XML_PAR_KEY + "='" + kvp.Key + "'>" + kvp.Value + "</" + XML_STRING + ">");
                }
                scale--;
            } else {
                object value = XML_VALUE_NOT_FOUND;
                string type = XML_UNKNOWN_TYPE;
                if (field is GSimpleField<int>) {
                    value = (int)((GSimpleField<int>)field).Value;
                    type = XML_TYPE_INT;
                } else if (field is GSimpleField<uint>) {
                    value = (uint)((GSimpleField<uint>)field).Value;
                    type = XML_TYPE_DWORD;
                } else if (field is GSimpleField<short>) {
                    value = (short)((GSimpleField<short>)field).Value;
                    type = XML_TYPE_SHORT;
                } else if (field is GSimpleField<char>) {
                    value = (char)((GSimpleField<char>)field).Value;
                    type = XML_TYPE_CHAR;
                } else if (field is GSimpleField<byte>) {
                    value = (byte)((GSimpleField<byte>)field).Value;
                    type = XML_TYPE_BYTE;
                } else if (field is GLargeField<UInt64>) {
                    value = (UInt64)((GLargeField<UInt64>)field).Value;
                    type = XML_TYPE_DWORD64;
                } else if (field is GLargeField<Int64>) {
                    value = (Int64)((GLargeField<Int64>)field).Value;
                    type = XML_TYPE_INT64;
                } else if (field is GSimpleField<float>) {
                    value = (float)((GSimpleField<float>)field).Value;
                    type = XML_TYPE_FLOAT;
                } else if (field is GLargeField<double>) {
                    value = (double)((GLargeField<double>)field).Value;
                    type = XML_TYPE_DOUBLE;
                } else if (field is GCExoString) {
                    value = (string)((GCExoString)field).Value;
                    type = XML_TYPE_CEXOSTRING;
                } else if (field is GResRef) {
                    value = (string)((GResRef)field).Value;
                    type = XML_TYPE_RESREF;
                } else if (field is GLargeField<byte[]>) {
                    value = (byte[])((GSimpleField<byte[]>)field).Value;
                    type = XML_TYPE_VOID;
                }
                writeLine("<" + XML_FIELD + " " + XML_PAR_TYPE + "='" + type.ToString() + "' " + XML_PAR_LABEL + "='" + field.Label + "'>" + value.ToString() + "</" + XML_FIELD + ">");
            }
        }

        private void writeList(GList list) {
            writeLine("<" + XML_LIST + " " + XML_PAR_LABEL + "='" + list.Label + "'>");
            writeChilds(list);
            writeLine("</" + XML_LIST + ">");
        }

        public void writeStruct(GStruct str) {
            writeLine("<" + XML_STRUCT + ">");
            writeChilds(str);
            writeLine("</" + XML_STRUCT + ">");
        }

        private void writeChilds(GCompositeField cpsit) {
            scale++;
            foreach (GField fld in cpsit.get()) {
                writeField(fld);
            }
            scale--;
        }

        private void writeLine(string value) {
            sw.WriteLine(scale.get() + value);
        }
    }
}
