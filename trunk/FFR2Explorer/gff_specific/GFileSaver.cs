using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace FFR2Explorer.gff_specific {
    public class GFileSaver {

        private string path;
        private Struct root;

        private List<Struct> l_str;
        private List<List> l_list;
        private List<Field> l_fld;

        private List<LargeField<UInt64>> l_dword64;
        private List<LargeField<Int64>> l_int64;
        private List<LargeField<double>> l_double;
        private List<CExoString> l_exostr;
        private List<ResRef> l_resref;
        private List<CExoLocString> l_exolocstr;
        private List<LargeField<byte[]>> l_void;


        public GFileSaver(string path, Struct root) {
            l_str = new List<Struct>();
            l_list = new List<List>();
            l_fld = new List<Field>();
            l_dword64 = new List<LargeField<UInt64>>();
            l_int64 = new List<LargeField<Int64>>();
            l_double = new List<LargeField<double>>();
            l_exostr = new List<CExoString>();
            l_resref = new List<ResRef>();
            l_exolocstr = new List<CExoLocString>();
            l_void = new List<LargeField<byte[]>>();
            this.path = path;
            this.root = root;
        }

        public void save() {
            analyse(root);
            calculateHeader();
        }

        private void calculateHeader() {

        }

        private void analyse(Field fld) {
            if (fld is CompositeField) {
                if (fld is Struct) {
                    l_str.Add((Struct)fld);
                } else if (fld is List) {
                    l_list.Add((List)fld);
                }
                CompositeField cpsit = (CompositeField)fld;
                foreach (Field child in cpsit.Childs) {
                    analyse(child);
                }
            } else {
                if (fld is LargeField<UInt64>) {
                    l_dword64.Add((LargeField<UInt64>)fld);
                } else if (fld is LargeField<Int64>) {
                    l_int64.Add((LargeField<Int64>)fld);
                } else if (fld is LargeField<double>) {
                    l_double.Add((LargeField<double>)fld);
                } else if (fld is CExoString) {
                    l_exostr.Add((CExoString)fld);
                } else if (fld is ResRef) {
                    l_resref.Add((ResRef)fld);
                } else if (fld is CExoLocString) {
                    l_exolocstr.Add((CExoLocString)fld);
                } else if (fld is LargeField<byte[]>) {
                    l_void.Add((LargeField<byte[]>)fld);
                }
                l_fld.Add(fld);
            }
        }
    }
}
