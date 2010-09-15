using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using Bioware.Virtual;

namespace Bioware.GFF {
    public class GFileSaver {
        GHeaderSTR _h;

        #region Stream et writer.
        FileStream _fs;
        BinaryWriter _bw;
        ASCIIEncoding _ascii;
        #endregion

        #region Infos générales.
        VStruct root;
        string path;
        string ext;
        string ver;
        #endregion

        public struct Position {
            public int index;
            public long offset;
            public Position(int index, long offset) {
                this.index = index;
                this.offset = offset;
            }
        }

        #region Dictionnaires de positionnement.
        Dictionary<VStruct, Position> d_str_pos;
        Dictionary<VStruct, long> d_str_fldind;
        Dictionary<VList, long> d_lst_lstidx;
        Dictionary<VField, Position> d_fld_pos;
        Dictionary<VField, Position> d_fld_flddat;
        Dictionary<string, Position> d_lab_pos;
        #endregion

        #region Listes des structures.
        List<GStructSTR> l_sstr;
        List<GFieldSTR> l_fstr;
        #endregion

        #region Listes par type de champ.
        private List<VField> l_simple_fld;
        private List<VField> l_complex_fld;
        private List<VField> l_large_fld;
        private List<VField> l_cpsit_fld;
        #endregion

        #region Listes des listes et des structures.
        private List<VStruct> l_str;
        private List<VList> l_lst;
        #endregion

        #region Listes des champs complexes.
        private List<VDword64> l_dword64;
        private List<VInt64> l_int64;
        private List<VDouble> l_double;
        private List<VExoString> l_exostr;
        private List<VResRef> l_resref;
        private List<VExoLocString> l_exolocstr;
        private List<VVoid> l_void;
        #endregion

        private List<VField> l_fld;
        private List<string> l_label;

        public GFileSaver(VStruct root, string path, string ext) {
            initVars(root, path, ext);
        }
        private void initVars(VStruct root, string path, string ext) {
            this.root = root;
            this.path = path;
            this.ext = ext;
            this.ver = GHeaderSTR.VERSION;
            _ascii = new ASCIIEncoding();
            _h = (new GHeaderSTR()).init();
            if (File.Exists(path)) {
                _fs = File.Open(path, FileMode.Truncate);
            } else {
                _fs = File.Open(path, FileMode.CreateNew);
            }
            _bw = new BinaryWriter(_fs);

            d_str_pos = new Dictionary<VStruct, Position>();
            d_str_fldind = new Dictionary<VStruct, long>();
            d_lst_lstidx = new Dictionary<VList, long>();
            d_fld_pos = new Dictionary<VField, Position>();
            d_fld_flddat = new Dictionary<VField, Position>();
            d_lab_pos = new Dictionary<string, Position>();

            l_fstr = new List<GFieldSTR>();
            l_sstr = new List<GStructSTR>();

            l_simple_fld = new List<VField>();
            l_complex_fld = new List<VField>();
            l_large_fld = new List<VField>();
            l_cpsit_fld = new List<VField>();

            l_str = new List<VStruct>();
            l_lst = new List<VList>();

            l_dword64 = new List<VDword64>();
            l_int64 = new List<VInt64>();
            l_double = new List<VDouble>();
            l_exostr = new List<VExoString>();
            l_resref = new List<VResRef>();
            l_exolocstr = new List<VExoLocString>();
            l_void = new List<VVoid>();

            l_label = new List<string>();
            l_fld = new List<VField>();
        }

        public void save() {
            // On laisse la place pour les infos d'en-tête.
            _bw.Seek(GHeaderSTR.SIZE, SeekOrigin.Begin);
            // On décortique la structure racine.
            analyseField(root);
            // On écrit le fichier.
            // Attention ! Ne jamais changer l'ordre d'écriture !
            writeAllStructs();
            writeAllFields();
            writeLabels();
            writeFieldDatas();
            writeFieldIndices();
            writeListIndices();
            writeHeader();

            linkStructs();
            linkFields();
        }

        private void writeAllStructs() {
            int index = 0;
            _h.Infos[GHeaderSTR.STRUCT_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (VStruct str in l_str) {
                long offset = _bw.BaseStream.Position;
                if (writeStruct(str)) {
                    d_str_pos.Add(str, new Position(index, offset));
                    index++;
                }
            }
            _h.Infos[GHeaderSTR.STRUCT_COUNT] = (uint)index;
        }
        private void writeAllFields() {
            int index = 0;
            _h.Infos[GHeaderSTR.FIELD_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (VField fld in l_fld) {
                long offset = _bw.BaseStream.Position;
                if (writeField(fld)) {
                    d_fld_pos.Add(fld, new Position(index, offset));
                    index++;
                } else {
                    d_fld_pos.Add(fld, new Position(0, offset));
                }
            }
            _h.Infos[GHeaderSTR.FIELD_COUNT] = (uint)index;
        }

        private void writeLabels() {
            int index = 0;
            _h.Infos[GHeaderSTR.LABEL_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (string label in l_label) {
                long label_offset = _bw.BaseStream.Position;
                string normalized_label = label.PadRight(GConst.LABEL_LENGTH, '\0');
                d_lab_pos.Add(normalized_label, new Position(index, label_offset));
                _bw.Write(normalized_label.ToCharArray());
                index++;
            }
            _h.Infos[GHeaderSTR.LABEL_COUNT] = (uint)index;
        }
        private void writeFieldDatas() {
            int index = 0;
            _h.Infos[GHeaderSTR.FIELD_DATA_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (VField lfld in l_large_fld) {
                d_fld_flddat.Add(lfld, new Position(index++, _bw.BaseStream.Position));
                switch (lfld.Type) {
                    case VType.DWORD64:
                        _bw.Write(((VDword64)lfld).Value);
                        break;
                    case VType.INT64:
                        _bw.Write(((VInt64)lfld).Value);
                        break;
                    case VType.DOUBLE:
                        _bw.Write(BitConverter.DoubleToInt64Bits(((VDouble)lfld).Value));
                        break;
                    case VType.CEXOSTRING:
                        string str = ((VExoString)lfld).Value;
                        _bw.Write((uint)_ascii.GetByteCount(str));
                        _bw.Write(_ascii.GetBytes(str));
                        break;
                    case VType.RESREF:
                        string rf = ((VResRef)lfld).Value;
                        _bw.Write((byte)_ascii.GetByteCount(rf));
                        _bw.Write(_ascii.GetBytes(rf));
                        break;
                    case VType.CEXOLOCSTRING:
                        writeCExoLocString((VExoLocString)lfld);
                        break;
                    case VType.VOID:
                        _bw.Write(((VVoid)lfld).Value);
                        break;
                }
            }
            _h.Infos[GHeaderSTR.FIELD_DATA_COUNT] = ((uint)_bw.BaseStream.Position) - _h.Infos[GHeaderSTR.FIELD_DATA_OFFSET];
        }
        private void writeCExoLocString(VExoLocString exlstr) {
            List<byte> buffer = new List<byte>();
            foreach (KeyValuePair<int, string> kvp in exlstr.Value) {
                buffer.AddRange(BitConverter.GetBytes((uint)kvp.Key));
                buffer.AddRange(BitConverter.GetBytes((uint)kvp.Value.Length));
                buffer.AddRange(_ascii.GetBytes(kvp.Value));
            }
            _bw.Write(buffer.Count + sizeof(uint) * 2);
            _bw.Write((uint)exlstr.StringRef);
            _bw.Write((uint)exlstr.Value.Count);
            _bw.Write(buffer.ToArray());
        }
        private void writeFieldIndices() {
            _h.Infos[GHeaderSTR.FIELD_INDICES_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (VStruct str in l_str) {
                if (str.get().Count > 1) {
                    d_str_fldind.Add(str, _bw.BaseStream.Position);
                    foreach (VField fld in str.get()) {
                        Position p = d_fld_pos[fld];
                        _bw.Write(p.index);
                    }
                }
            }
            _h.Infos[GHeaderSTR.FIELD_INDICES_COUNT] = ((uint)_bw.BaseStream.Position) - _h.Infos[GHeaderSTR.FIELD_INDICES_OFFSET];
        }
        private void writeListIndices() {
            _h.Infos[GHeaderSTR.LIST_INDICES_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (VList lst in l_lst) {
                d_lst_lstidx.Add(lst, _bw.BaseStream.Position);
                _bw.Write(lst.get().Count);
                foreach (VField fld in lst.get()) {
                    VStruct str = (VStruct)fld;
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
            foreach (KeyValuePair<VStruct, Position> kvp in d_str_pos) {
                VStruct str = kvp.Key;
                long str_offset = kvp.Value.offset;
                int str_index = kvp.Value.index;
                long saved_pos = _bw.BaseStream.Position;
                _bw.BaseStream.Position = str_offset + sizeof(UInt32);
                if (str.get().Count > 1) {
                    // On le relie à une liste de fields.
                    long list_offset = d_str_fldind[str];
                    _bw.Write((uint)list_offset - _h.Infos[GHeaderSTR.FIELD_INDICES_OFFSET]);
                } else {
                    // On le relie à un index de field.
                    int field_index = d_fld_pos[str].index;
                    _bw.Write((uint)field_index);
                }
                _bw.BaseStream.Position = saved_pos;
            }
        }
        private void linkFields() {
            foreach (KeyValuePair<VField, Position> kvp in d_fld_pos) {
                VField fld = kvp.Key;
                long offset = kvp.Value.offset;
                int index = kvp.Value.index;
                long pos = _bw.BaseStream.Position;
                _bw.BaseStream.Position = offset + 2 * sizeof(UInt32);
                if (fld is VStruct) {
                    VStruct str = (VStruct)fld;
                    int s_index = d_str_pos[str].index;
                    _bw.Write((uint)s_index);
                } else if (fld is VList) {
                    VList lst = (VList)fld;
                    uint l_pos = (uint)d_lst_lstidx[lst] - _h.Infos[GHeaderSTR.LIST_INDICES_OFFSET];
                    _bw.Write((uint)l_pos);
                } else {
                    if (d_fld_flddat.ContainsKey(fld)) {
                        uint fd_pos = (uint)d_fld_flddat[fld].offset - _h.Infos[GHeaderSTR.FIELD_DATA_OFFSET];
                        _bw.Write((uint)fd_pos);
                    }
                }
                _bw.BaseStream.Position = pos;
            }
        }

        private GFieldSTR createFieldSTR(VField fld) {
            // On crée la structure à renvoyer.
            GFieldSTR result = new GFieldSTR();
            // On crée les variables qui vont accueuillir les données de la structure.
            byte[] dataOrOffset = new byte[sizeof(uint)];
            int labelIndex = l_label.IndexOf(fld.Label);
            VType type = fld.Type;
            // On détermine l'algorithme en fonction du typage du champ.
            if (fld is VCpsitField) {
                if (fld is VStruct) {
                    // ATTENTION ! Ce code ne devrait JAMAIS ÊTRE ATTEINT si la structure RACINE, ou ENFANT D'UNE LISTE !
                    // Dans le cas contraire, on défini la valeur de DataOrOffset comme étant l'index de la structure
                    // dans la liste des structures.
                    VStruct str = (VStruct)fld;
                    dataOrOffset = BitConverter.GetBytes((uint)l_str.IndexOf(str));
                    labelIndex = (labelIndex == -1) ? (0) : (labelIndex);
                } else {
                    // Dans le cas d'une liste, on liera avec la liste d'indices de champ après.
                    dataOrOffset = BitConverter.GetBytes((uint)0);
                }
            } else {
                if (VField.isComplex(result.Type)) {
                    // On liera avec ses données plus tard.
                    dataOrOffset = BitConverter.GetBytes((uint)0);
                } else {
                    // On stocke directement les données.
                    #region Switch de conversion de la valeur.
                    switch (type) {
                        case VType.BYTE:
                            dataOrOffset = BitConverter.GetBytes(((VByte)fld).Value);
                            break;
                        case VType.CHAR:
                            dataOrOffset = BitConverter.GetBytes(((VChar)fld).Value);
                            break;
                        case VType.WORD:
                            dataOrOffset = BitConverter.GetBytes(((VWord)fld).Value);
                            break;
                        case VType.SHORT:
                            dataOrOffset = BitConverter.GetBytes(((VShort)fld).Value);
                            break;
                        case VType.DWORD:
                            dataOrOffset = BitConverter.GetBytes(((VDword)fld).Value);
                            break;
                        case VType.INT:
                            dataOrOffset = BitConverter.GetBytes(((VInt)fld).Value);
                            break;
                        case VType.FLOAT:
                            dataOrOffset = BitConverter.GetBytes(((VFloat)fld).Value);
                            break;
                    }
                    #endregion
                }
            }

            // On vérifie que la taille du tableau de bytes est bien de taille 'sizeof(uint)'
            if (dataOrOffset.Length < sizeof(uint)) {
                Array.Resize<byte>(ref dataOrOffset, sizeof(uint));
            } else if (dataOrOffset.Length > sizeof(uint)) {
                // Erreur ! Tableau trop grand !
            }
            // On stocke les données dans la structure.
            result.Type = type;
            result.LabelIndex = (uint)labelIndex;
            result.DataOrOffset = BitConverter.ToUInt32(dataOrOffset, 0);
            // On renvoie la structure.
            return result;
        }
        private void writeFieldSTR(GFieldSTR sfld) {
            // Ne pas changer d'ordre !
            _bw.Write((uint)sfld.Type);
            _bw.Write((uint)sfld.LabelIndex);
            _bw.Write((uint)sfld.DataOrOffset);
        }
        private bool writeField(VField fld) {
            // Dans le cas spécial d'une structure, on n'écrit un champ associé uniquement
            // si la structure est possédée par une autre structure et n'est pas racine.
            if (!(fld is VStruct) || (fld is VStruct && (fld.Owner != null && fld.Owner is VStruct))) {
                writeFieldSTR(createFieldSTR(fld));
                return true;
            }
            return false;
        }

        private GStructSTR createStructSTR(VStruct str) {
            GStructSTR sstr = new GStructSTR();
            sstr.FieldCount = (uint)str.get().Count;
            sstr.Type = (str.IsRoot) ? (GStructSTR.ROOT_TYPE) : (GStructSTR.STANDARD_TYPE);
            sstr.DataOrOffset = 0;
            return sstr;
        }
        private void writeStructSTR(GStructSTR sstr) {
            // Ne pas changer d'ordre !
            _bw.Write((uint)sstr.Type);
            _bw.Write((uint)sstr.DataOrOffset);
            _bw.Write((uint)sstr.FieldCount);
        }
        private bool writeStruct(VStruct str) {
            writeStructSTR(createStructSTR(str));
            return true;
        }

        private void analyseField(VField fld) {
            l_fld.Add(fld);
            if (VField.isComplex(fld.Type)) {
                l_complex_fld.Add(fld);
                if (VField.isComposite(fld.Type)) {
                    VCpsitField cpsit = (VCpsitField)fld;
                    l_cpsit_fld.Add(fld);
                    if (cpsit is VStruct) {
                        l_str.Add((VStruct)cpsit);
                    } else {
                        l_lst.Add((VList)cpsit);
                    }
                    foreach (VField child in cpsit.get()) {
                        analyseField(child);
                    }
                } else {
                    l_large_fld.Add(fld);
                }
            } else {
                l_simple_fld.Add(fld);
            }
            if (!l_label.Contains(fld.Label) && fld.Label != VStruct.DEFAULT_LABEL) {
                l_label.Add(fld.Label);
            }
        }
    }
}
