using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace FFR2Explorer.gff_specific {

    public class GFileSaver {
        private List<string> l_label;

        /*
        private string path;
        private GStruct root;
        private string ext;

        private char[] type = new char[GHeader.FILE_TYPE_SIZE];
        private char[] version = new char[GHeader.FILE_VERSION_SIZE];

        private uint structOffset;
        private int structCount;

        private uint fieldOffset;
        private int fieldCount;

        private uint fieldIndicesOffset;
        private int fieldIndicesCount;

        private uint labelOffset;
        private int labelCount;

        private uint fieldDataOffset;
        private int fieldDataCount;

        private uint listIndicesOffset;
        private int listIndicesCount;

        private List<GStruct> l_str_fld;
        private List<GList> l_list_fld;

        public GFileSaver(string path, string ext, GStruct root) {
            l_str_fld = new List<GStruct>();
            l_list_fld = new List<GList>();
            this.path = path;
            this.ext = ext;
            this.root = root;
        }

        public void save() {
            analyse(root);
            calculateHeader();
            populate();
        }

        FileStream fs;
        BinaryWriter br;

        private void populate() {
            if (File.Exists(path)) {
                fs = File.Open(path, FileMode.Truncate);
            } else {
                fs = File.Open(path, FileMode.CreateNew);
            }
            br = new BinaryWriter(fs);
            br.BaseStream.Position = 0;
            ext = ext.Remove(0, 1);
            ext = ext.ToUpper();
            ext = ext.PadRight(4);
            br.Write(ext.ToCharArray());
            string version = "V3.2";
            br.Write(version.ToCharArray());

            // Write structures.
            foreach (GStruct str in l_str_fld) {
                writeStruct(str);
            }

        }
        private void calculateHeader() {
            structOffset = GHeader.FILE_TYPE_SIZE + GHeader.FILE_VERSION_SIZE + 12 * sizeof(uint);
            structCount = l_str_fld.Count;
            fieldOffset = structOffset + (uint)structCount * GConst.STRUCT_SIZE;
            fieldCount = l_simple_fld.Count + l_complex_fld.Count + l_list_fld.Count;
            labelOffset = fieldOffset + (uint)fieldCount * GConst.STRUCT_SIZE;
            labelCount = l_label.Count;
            fieldDataOffset = labelOffset + (uint)l_label.Count * GConst.LABEL_LENGTH;
            fieldDataCount = calculateFieldDataCount();
            fieldIndicesOffset = fieldDataOffset + (uint)fieldDataCount;
            fieldIndicesCount = l_complex_fld.Count;
            listIndicesOffset = fieldIndicesOffset + (uint)fieldIndicesCount * sizeof(UInt32);
            listIndicesCount = calculateListIndicesCount();
        }
        private int calculateListIndicesCount() {
            int size = 0;
            foreach (GList list in l_list_fld) {
                size += list.get().Count + sizeof(UInt32);
            }
            return size;
        }

        private int calculateFieldDataCount() {
            int size = 0;
            size += l_dword64.Count * sizeof(UInt64);
            size += l_int64.Count * sizeof(Int64);
            size += l_double.Count * sizeof(double);
            foreach (GCExoString exostr in l_exostr) {
                String str = exostr.Value;
                size += sizeof(int);
                size += str.Length;
            }
            size += l_resref.Count * GConst.RESREF_MAX_LENGTH + 1;
            foreach (GCExoLocString exolocstr in l_exolocstr) {
                size += 3 * sizeof(int);
                Dictionary<int, string> dic = exolocstr.Value;
                foreach (KeyValuePair<int, string> kvp in dic) {
                    size += kvp.Value.Length + 2 * sizeof(int);
                }
            }
            foreach (GVoid v in l_void) {
                size += v.Value.Length;
            }
            return size;
        }

        }*/

        GHeaderSTR _h;

        #region Stream et writer.
        FileStream _fs;
        BinaryWriter _bw;
        #endregion

        #region Infos générales.
        GStruct root;
        string path;
        string ext;
        string ver;
        #endregion

        public struct Position {
            public int index;
            public long offset;
        }

        #region Dictionnaires de positionnement.
        Dictionary<GStruct, Position> d_str_pos;
        Dictionary<GStruct, long> d_str_fldind;
        Dictionary<GList, long> d_lst_lstidx;
        Dictionary<GField, Position> d_fld_pos;
        Dictionary<GField, Position> d_fld_flddat;
        Dictionary<string, Position> d_lab_pos;
        #endregion

        #region Listes des structures.
        List<GStructSTR> l_sstr;
        List<GFieldSTR> l_fstr;
        #endregion

        #region Listes par type de champ.
        private List<GField> l_simple_fld;
        private List<GField> l_complex_fld;
        private List<GField> l_cpsit_fld;
        #endregion

        #region Listes des listes et des structures.
        private List<GStruct> l_str;
        private List<GList> l_lst;
        #endregion

        #region Listes des champs complexes.
        private List<GDword64> l_dword64;
        private List<GInt64> l_int64;
        private List<GDouble> l_double;
        private List<GCExoString> l_exostr;
        private List<GResRef> l_resref;
        private List<GCExoLocString> l_exolocstr;
        private List<GVoid> l_void;
        #endregion

        public GFileSaver(GStruct root, string path, string ext) {
            initVars(root, path, ext);
        }
        private void initVars(GStruct root, string path, string ext) {
            this.root = root;
            this.path = path;
            this.ext = ext;
            this.ver = GConst.VERSION;
            _h = (new GHeaderSTR()).init();
            if (File.Exists(path)) {
                _fs = File.Open(path, FileMode.Truncate);
            } else {
                _fs = File.Open(path, FileMode.CreateNew);
            }
            _bw = new BinaryWriter(_fs);

            d_str_pos = new Dictionary<GStruct, Position>();
            d_str_fldind = new Dictionary<GStruct, long>();
            d_lst_lstidx = new Dictionary<GList, long>();
            d_fld_pos = new Dictionary<GField, Position>();
            d_fld_flddat = new Dictionary<GField, Position>();
            d_lab_pos = new Dictionary<string, Position>();

            l_fstr = new List<GFieldSTR>();
            l_sstr = new List<GStructSTR>();

            l_simple_fld = new List<GField>();
            l_complex_fld = new List<GField>();
            l_cpsit_fld = new List<GField>();

            l_str = new List<GStruct>();
            l_lst = new List<GList>();

            l_dword64 = new List<GDword64>();
            l_int64 = new List<GInt64>();
            l_double = new List<GDouble>();
            l_exostr = new List<GCExoString>();
            l_resref = new List<GResRef>();
            l_exolocstr = new List<GCExoLocString>();
            l_void = new List<GVoid>();

            l_label = new List<string>();
        }

        public void save() {
            // On laisse la place pour les infos d'en-tête.
            _bw.Seek(GHeaderSTR.SIZE, SeekOrigin.Begin);
            // On décortique la structure racine.
            analyseField(root);
            // On écrit le fichier.
            writeAllStructs();
            writeAllFields();
            writeLabels();
            writeLargeFields();
            writeFieldIndices();
            writeListIndices();
            writeHeader();

            linkStructs();
            linkFields();
        }

        private void writeAllFields() {
            int index = 0;
            _h.Infos[GHeaderSTR.FIELD_OFFSET] = (uint)_bw.BaseStream.Position;
            #region A améliorer... mais fonctionnel.
            foreach (GField fld in l_simple_fld) {
                GFieldSTR sfld = new GFieldSTR();
                byte[] bytes = new byte[sizeof(UInt32)];
                sfld.DataOrOffset = 0;
                if (fld is GByte) {
                    sfld.Type = (uint)GConst.BYTE;
                    byte[] tp = BitConverter.GetBytes(((GByte)fld).Value);
                    for (int i = 0; i < sizeof(UInt32) && i < tp.Length; i++) {
                        bytes[i] = tp[i];
                    }
                } else if (fld is GChar) {
                    sfld.Type = (uint)GConst.CHAR;
                    byte[] tp = BitConverter.GetBytes(((GChar)fld).Value);
                    for (int i = 0; i < sizeof(UInt32) && i < tp.Length; i++) {
                        bytes[i] = tp[i];
                    }
                } else if (fld is GWord) {
                    sfld.Type = (uint)GConst.WORD;
                    byte[] tp = BitConverter.GetBytes(((GWord)fld).Value);
                    for (int i = 0; i < 4 && i < tp.Length; i++) {
                        bytes[i] = tp[i];
                    }
                } else if (fld is GShort) {
                    sfld.Type = (uint)GConst.SHORT;
                    byte[] tp = BitConverter.GetBytes(((GShort)fld).Value);
                    for (int i = 0; i < sizeof(UInt32) && i < tp.Length; i++) {
                        bytes[i] = tp[i];
                    }
                } else if (fld is GDword) {
                    sfld.Type = GConst.DWORD;
                    byte[] tp = BitConverter.GetBytes(((GDword)fld).Value);
                    for (int i = 0; i < sizeof(UInt32) && i < tp.Length; i++) {
                        bytes[i] = tp[i];
                    }
                } else if (fld is GInt) {
                    sfld.Type = GConst.INT;
                    byte[] tp = BitConverter.GetBytes(((GInt)fld).Value);
                    for (int i = 0; i < sizeof(UInt32) && i < tp.Length; i++) {
                        bytes[i] = tp[i];
                    }
                } else if (fld is GFloat) {
                    sfld.Type = GConst.FLOAT;
                    byte[] tp = BitConverter.GetBytes(((GFloat)fld).Value);
                    for (int i = 0; i < sizeof(UInt32) && i < tp.Length; i++) {
                        bytes[i] = tp[i];
                    }
                }
                sfld.DataOrOffset = BitConverter.ToUInt32(bytes, 0);
                sfld.LabelIndex = BitConverter.ToUInt32(BitConverter.GetBytes(l_label.IndexOf(fld.Label)), 0);
                Position p = new Position();
                p.index = index++;
                p.offset = _bw.BaseStream.Position;
                d_fld_pos.Add(fld, p);
                writeField(sfld);
            }
            foreach (GField fld in l_complex_fld) {
                GFieldSTR sfld = new GFieldSTR();
                sfld.DataOrOffset = (uint)0;
                if (fld is GDword64) {
                    sfld.Type = (uint)GConst.DWORD64;
                } else if (fld is GInt64) {
                    sfld.Type = (uint)GConst.INT64;
                } else if (fld is GDouble) {
                    sfld.Type = (uint)GConst.DOUBLE;
                } else if (fld is GCExoString) {
                    sfld.Type = (uint)GConst.CEXOSTRING;
                } else if (fld is GResRef) {
                    sfld.Type = (uint)GConst.RESREF;
                } else if (fld is GCExoLocString) {
                    sfld.Type = (uint)GConst.CEXOLOCSTRING;
                } else if (fld is GVoid) {
                    sfld.Type = (uint)GConst.VOID;
                }
                sfld.LabelIndex = BitConverter.ToUInt32(BitConverter.GetBytes(l_label.IndexOf(fld.Label)), 0);
                Position p = new Position();
                p.index = index++;
                p.offset = _bw.BaseStream.Position;
                d_fld_pos.Add(fld, p);
                writeField(sfld);
            }
            foreach (GStruct str in l_str) {
                // ATTENTION //
                // La structure n'est ajoutée ici qu'à condition qu'elle ne soit pas nulle et pas possédée par une liste !! //
                if (str.Owner != null && str.Owner is GStruct) {
                    GFieldSTR sfld = new GFieldSTR();
                    sfld.DataOrOffset = (uint)l_str.IndexOf(str);
                    sfld.Type = (uint)GConst.STRUCT;
                    if (l_label.IndexOf(str.Label) != -1) {
                        sfld.LabelIndex = BitConverter.ToUInt32(BitConverter.GetBytes(l_label.IndexOf(str.Label)), 0);
                    } else {
                        sfld.LabelIndex = (uint)0;
                    }
                    Position p = new Position();
                    p.index = index++;
                    p.offset = _bw.BaseStream.Position;
                    d_fld_pos.Add(str, p);
                    writeField(sfld);
                }
            }
            foreach (GList lst in l_lst) {
                GFieldSTR sfld = new GFieldSTR();
                sfld.DataOrOffset = (uint)0;
                sfld.Type = (uint)GConst.LIST;
                sfld.LabelIndex = BitConverter.ToUInt32(BitConverter.GetBytes(l_label.IndexOf(lst.Label)), 0);
                Position p = new Position();
                p.index = index++;
                p.offset = _bw.BaseStream.Position;
                d_fld_pos.Add(lst, p);
                writeField(sfld);
            }
            #endregion
            _h.Infos[GHeaderSTR.FIELD_COUNT] = (uint)index;
        }
        private void writeAllStructs() {
            int index = 0;
            _h.Infos[GHeaderSTR.STRUCT_OFFSET] = (uint)_bw.BaseStream.Position;
            #region A améliorer... mais fonctionnel.
            foreach (GStruct str in l_str) {
                GStructSTR sstr = new GStructSTR();
                sstr.FieldCount = BitConverter.ToUInt32(BitConverter.GetBytes(str.get().Count), 0);
                int type = (str.Owner == null) ? (-1) : (1);
                sstr.Type = BitConverter.ToUInt32(BitConverter.GetBytes(type), 0);
                sstr.DataOrOffset = BitConverter.ToUInt32(BitConverter.GetBytes(0), 0);
                Position p = new Position();
                p.index = index++;
                p.offset = _bw.BaseStream.Position;
                d_str_pos.Add(str, p);
                writeStruct(sstr);
            }
            #endregion
            _h.Infos[GHeaderSTR.STRUCT_COUNT] = (uint)index;
        }
        private void writeLabels() {
            int index = 0;
            _h.Infos[GHeaderSTR.LABEL_OFFSET] = (uint)_bw.BaseStream.Position;
            #region A améliorer mais fonctionnel.
            foreach (string label in l_label) {
                string m_label = label.PadRight(GConst.LABEL_LENGTH, '\0');
                Position p = new Position();
                p.index = index++;
                p.offset = _bw.BaseStream.Position;
                d_lab_pos.Add(m_label, p);
                _bw.Write(m_label.ToCharArray());

            }
            #endregion
            _h.Infos[GHeaderSTR.LABEL_COUNT] = (uint)index;
        }
        private void writeLargeFields() {
            int index = 0;
            _h.Infos[GHeaderSTR.FIELD_DATA_OFFSET] = (uint)_bw.BaseStream.Position;
            #region A améliorer... mais fonctionnel.
            foreach (GField lfld in l_complex_fld) {
                Position p = new Position();
                p.index = index++;
                p.offset = _bw.BaseStream.Position;
                d_fld_flddat.Add(lfld, p);
                if (lfld is GDword64) {
                    _bw.Write(((GDword64)lfld).Value);
                } else if (lfld is GInt64) {
                    _bw.Write(((GInt64)lfld).Value);
                } else if (lfld is GDouble) {
                    _bw.Write(BitConverter.DoubleToInt64Bits(((GDouble)lfld).Value));
                } else if (lfld is GCExoString) {
                    string str = ((GCExoString)lfld).Value;
                    _bw.Write(str.Length);
                    _bw.Write(str.ToCharArray());
                } else if (lfld is GResRef) {
                    string rf = ((GResRef)lfld).Value;
                    byte[] bytes = BitConverter.GetBytes(rf.Length);
                    _bw.Write(bytes[0]);
                    _bw.Write(rf.ToCharArray());
                } else if (lfld is GCExoLocString) {
                    long start = _bw.BaseStream.Position;
                    GCExoLocString exolstr = (GCExoLocString)lfld;
                    uint totalSize = 8;
                    uint strCount = (uint)exolstr.Value.Count;
                    uint strRef = exolstr.StringRef;
                    _bw.Write(totalSize);
                    _bw.Write(strRef);
                    _bw.Write(strCount);
                    Dictionary<int, string> dic = exolstr.Value;
                    foreach (KeyValuePair<int, string> kvp in dic) {
                        totalSize += (uint)(8 + kvp.Value.Length);
                        _bw.Write(kvp.Key);
                        _bw.Write(kvp.Value.Length);
                        _bw.Write(kvp.Value.ToCharArray());
                    }
                    long pos = _bw.BaseStream.Position;
                    _bw.BaseStream.Position = start;
                    _bw.Write(totalSize);
                    _bw.BaseStream.Position = pos;
                } else if (lfld is GVoid) {
                    _bw.Write(((GVoid)lfld).Value);
                }
            }
            #endregion
            _h.Infos[GHeaderSTR.FIELD_DATA_COUNT] = ((uint)_bw.BaseStream.Position) - _h.Infos[GHeaderSTR.FIELD_DATA_OFFSET];
        }
        private void writeFieldIndices() {
            _h.Infos[GHeaderSTR.FIELD_INDICES_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (GStruct str in l_str) {
                if (str.get().Count > 1) {
                    d_str_fldind.Add(str, _bw.BaseStream.Position);
                    foreach (GField fld in str.get()) {
                        Position p = d_fld_pos[fld];
                        _bw.Write(p.index);
                    }
                }
            }
            _h.Infos[GHeaderSTR.FIELD_INDICES_COUNT] = ((uint)_bw.BaseStream.Position) - _h.Infos[GHeaderSTR.FIELD_INDICES_OFFSET];
        }
        private void writeListIndices() {
            _h.Infos[GHeaderSTR.LIST_INDICES_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (GList lst in l_lst) {
                d_lst_lstidx.Add(lst, _bw.BaseStream.Position);
                _bw.Write(lst.get().Count);
                foreach (GField fld in lst.get()) {
                    GStruct str = (GStruct)fld;
                    Position p = d_str_pos[str];
                    _bw.Write(p.index);
                }
            }
            _h.Infos[GHeaderSTR.LIST_INDICES_COUNT] = ((uint)_bw.BaseStream.Position) - _h.Infos[GHeaderSTR.LIST_INDICES_OFFSET];
        }
        private void writeHeader() {
            long pos = _bw.BaseStream.Position;
            _bw.BaseStream.Position = 0;
            ext = ext.ToUpper();
            ext = ext.Remove(0, 1);
            ext = ext.PadRight(GHeader.FILE_TYPE_SIZE);
            _bw.Write(ext.ToCharArray());
            _bw.Write(ver.ToCharArray());
            _bw.Write(_h.Infos[GHeader.STRUCT_OFFSET]);
            _bw.Write(_h.Infos[GHeader.STRUCT_COUNT]);
            _bw.Write(_h.Infos[GHeader.FIELD_OFFSET]);
            _bw.Write(_h.Infos[GHeader.FIELD_COUNT]);
            _bw.Write(_h.Infos[GHeader.LABEL_OFFSET]);
            _bw.Write(_h.Infos[GHeader.LABEL_COUNT]);
            _bw.Write(_h.Infos[GHeader.FIELD_DATA_OFFSET]);
            _bw.Write(_h.Infos[GHeader.FIELD_DATA_COUNT]);
            _bw.Write(_h.Infos[GHeader.FIELD_INDICES_OFFSET]);
            _bw.Write(_h.Infos[GHeader.FIELD_INDICES_COUNT]);
            _bw.Write(_h.Infos[GHeader.LIST_INDICES_OFFSET]);
            _bw.Write(_h.Infos[GHeader.LIST_INDICES_COUNT]);
            _bw.BaseStream.Position = pos;
        }

        private void linkStructs() {
            // On relie toutes les structures.
            foreach (KeyValuePair<GStruct, Position> kvp in d_str_pos) {
                GStruct str = kvp.Key;
                long offset = kvp.Value.offset;
                int index = kvp.Value.index;
                if (str.get().Count > 1) {
                    // On le relie à une liste de fields.
                    long l_offset = d_str_fldind[str];
                    long pos = _bw.BaseStream.Position;
                    _bw.BaseStream.Position = offset + sizeof(UInt32);
                    uint towrite = (uint)l_offset - _h.Infos[GHeaderSTR.FIELD_INDICES_OFFSET];
                    _bw.Write(towrite);
                    _bw.BaseStream.Position = pos;
                } else {
                    // On le relie à un index de field.
                    int f_idx = d_fld_pos[str].index;
                    long pos = _bw.BaseStream.Position;
                    _bw.BaseStream.Position = offset + sizeof(UInt32);
                    _bw.Write((uint)f_idx);
                    _bw.BaseStream.Position = pos;
                }
            }
        }
        private void linkFields() {
            foreach (KeyValuePair<GField, Position> kvp in d_fld_pos) {
                GField fld = kvp.Key;
                long offset = kvp.Value.offset;
                int index = kvp.Value.index;
                if (fld is GStruct) {
                    GStruct str = (GStruct)fld;
                    // On le lie avec son index dans la liste des structures.
                    int s_index = d_str_pos[str].index;
                    long pos = _bw.BaseStream.Position;
                    _bw.BaseStream.Position = offset + 2 * sizeof(UInt32);
                    _bw.Write((uint)s_index);
                    _bw.BaseStream.Position = pos;
                } else if (fld is GList) {
                    GList lst = (GList)fld;
                    uint l_pos = (uint)d_lst_lstidx[lst] - _h.Infos[GHeaderSTR.LIST_INDICES_OFFSET];
                    long pos = _bw.BaseStream.Position;
                    _bw.BaseStream.Position = offset + 2 * sizeof(UInt32);
                    _bw.Write(l_pos);
                    _bw.BaseStream.Position = pos;
                } else {
                    if (d_fld_flddat.ContainsKey(fld)) {
                        uint fd_pos = (uint)d_fld_flddat[fld].offset - _h.Infos[GHeaderSTR.FIELD_DATA_OFFSET];
                        long pos = _bw.BaseStream.Position;
                        _bw.BaseStream.Position = offset + 2 * sizeof(UInt32);
                        _bw.Write(fd_pos);
                        _bw.BaseStream.Position = pos;
                    }
                }
            }
        }

        private void writeField(GFieldSTR sfld) {
            // Ne pas changer d'ordre !
            _bw.Write(sfld.Type);
            _bw.Write(sfld.LabelIndex);
            _bw.Write(sfld.DataOrOffset);
        }
        private void writeStruct(GStructSTR sstr) {
            // Ne pas changer d'ordre !
            _bw.Write(sstr.Type);
            _bw.Write(sstr.DataOrOffset);
            _bw.Write(sstr.FieldCount);
        }

        private void analyseField(GField fld) {
            if (fld is GByte) {
                l_simple_fld.Add(fld);
            } else if (fld is GChar) {
                l_simple_fld.Add(fld);
            } else if (fld is GWord) {
                l_simple_fld.Add(fld);
            } else if (fld is GShort) {
                l_simple_fld.Add(fld);
            } else if (fld is GDword) {
                l_simple_fld.Add(fld);
            } else if (fld is GInt) {
                l_simple_fld.Add(fld);
            } else if (fld is GDword64) {
                l_complex_fld.Add(fld);
                l_dword64.Add((GDword64)fld);
            } else if (fld is GInt64) {
                l_complex_fld.Add(fld);
                l_int64.Add((GInt64)fld);
            } else if (fld is GFloat) {
                l_simple_fld.Add(fld);
            } else if (fld is GDouble) {
                l_complex_fld.Add(fld);
                l_double.Add((GDouble)fld);
            } else if (fld is GCExoString) {
                l_complex_fld.Add(fld);
                l_exostr.Add((GCExoString)fld);
            } else if (fld is GResRef) {
                l_complex_fld.Add(fld);
                l_resref.Add((GResRef)fld);
            } else if (fld is GCExoLocString) {
                l_complex_fld.Add(fld);
                l_exolocstr.Add((GCExoLocString)fld);
            } else if (fld is GVoid) {
                l_complex_fld.Add(fld);
                l_void.Add((GVoid)fld);
            } else if (fld is GStruct) {
                l_cpsit_fld.Add(fld);
                l_str.Add((GStruct)fld);
            } else if (fld is GList) {
                l_cpsit_fld.Add(fld);
                l_lst.Add((GList)fld);
            }
            if (!(fld is GStruct)) {
                if (!l_label.Contains(fld.Label)) {
                    l_label.Add(fld.Label);
                }
            }
            if (fld is GCompositeField) {
                GCompositeField cpsit = (GCompositeField)fld;
                foreach (GField child in cpsit.get()) {
                    analyseField(child);
                }
            }
        }
    }
}
