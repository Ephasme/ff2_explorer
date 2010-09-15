using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;

namespace FFR2Explorer.gff_specific {
    public class GFileSaver {
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
            public Position(int index, long offset) {
                this.index = index;
                this.offset = offset;
            }
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
        private List<GField> l_large_fld;
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

        private List<GField> l_fld;
        private List<string> l_label;

        public GFileSaver(GStruct root, string path, string ext) {
            initVars(root, path, ext);
        }
        private void initVars(GStruct root, string path, string ext) {
            this.root = root;
            this.path = path;
            this.ext = ext;
            this.ver = GHeaderSTR.VERSION;
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
            l_large_fld = new List<GField>();
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
            l_fld = new List<GField>();
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

        private void writeAllStructs() {
            int index = 0;
            _h.Infos[GHeaderSTR.STRUCT_OFFSET] = (uint)_bw.BaseStream.Position;
            foreach (GStruct str in l_str) {
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
            foreach (GField fld in l_fld) {
                long offset = _bw.BaseStream.Position;
                if (writeField(fld)) {
                    d_fld_pos.Add(fld, new Position(index, offset));
                    index++;
                }
            }
            _h.Infos[GHeaderSTR.FIELD_COUNT] = (uint)index;
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
                long pos = _bw.BaseStream.Position;
                _bw.BaseStream.Position = offset + 2 * sizeof(UInt32);
                if (fld is GStruct) {
                    GStruct str = (GStruct)fld;
                    int s_index = d_str_pos[str].index;
                    _bw.Write((uint)s_index);
                } else if (fld is GList) {
                    GList lst = (GList)fld;
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

        private GFieldSTR createFieldSTR(GField fld) {
            // On crée la structure à renvoyer.
            GFieldSTR result = new GFieldSTR();
            // On crée les variables qui vont accueuillir les données de la structure.
            byte[] dataOrOffset = new byte[sizeof(uint)];
            int labelIndex = l_label.IndexOf(fld.Label);
            GType type = getType(fld);
            // On détermine l'algorithme en fonction du typage du champ.
            if (fld is GCompositeField) {
                if (fld is GStruct) {
                    // ATTENTION ! Ce code ne devrait JAMAIS ÊTRE ATTEINT si la structure RACINE, ou ENFANT D'UNE LISTE !
                    // Dans le cas contraire, on défini la valeur de DataOrOffset comme étant l'index de la structure
                    // dans la liste des structures.
                    GStruct str = (GStruct)fld;
                    dataOrOffset = BitConverter.GetBytes((uint)l_str.IndexOf(str));
                    labelIndex = (labelIndex == -1) ? (0) : (labelIndex);
                } else {
                    // Dans le cas d'une liste, on liera avec la liste d'indices de champ après.
                    dataOrOffset = BitConverter.GetBytes((uint)0);
                }
            } else {
                if (isComplex(result.Type)) {
                    // On liera avec ses données plus tard.
                    dataOrOffset = BitConverter.GetBytes((uint)0);
                } else {
                    // On stocke directement les données.
                    #region Switch de conversion de la valeur.
                    switch (type) {
                        case GType.BYTE:
                            dataOrOffset = BitConverter.GetBytes(((GByte)fld).Value);
                            break;
                        case GType.CHAR:
                            dataOrOffset = BitConverter.GetBytes(((GChar)fld).Value);
                            break;
                        case GType.WORD:
                            dataOrOffset = BitConverter.GetBytes(((GWord)fld).Value);
                            break;
                        case GType.SHORT:
                            dataOrOffset = BitConverter.GetBytes(((GShort)fld).Value);
                            break;
                        case GType.DWORD:
                            dataOrOffset = BitConverter.GetBytes(((GDword)fld).Value);
                            break;
                        case GType.INT:
                            dataOrOffset = BitConverter.GetBytes(((GInt)fld).Value);
                            break;
                        case GType.FLOAT:
                            dataOrOffset = BitConverter.GetBytes(((GFloat)fld).Value);
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
        private bool writeField(GField fld) {
            // Dans le cas spécial d'une structure, on n'écrit un champ associé uniquement
            // si la structure est possédée par une autre structure et n'est pas racine.
            if (!(fld is GStruct) || (fld is GStruct && (fld.Owner != null && fld.Owner is GStruct))) {
                writeFieldSTR(createFieldSTR(fld));
                return true;
            }
            return false;
        }

        private GStructSTR createStructSTR(GStruct str) {
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
        private bool writeStruct(GStruct str) {
            writeStructSTR(createStructSTR(str));
            return true;
        }

        public static GType getType(GField fld) {
            if (fld is GByte) {
                return GType.BYTE;
            } else if (fld is GChar) {
                return GType.CHAR;
            } else if (fld is GWord) {
                return GType.WORD;
            } else if (fld is GShort) {
                return GType.SHORT;
            } else if (fld is GDword) {
                return GType.DWORD;
            } else if (fld is GInt) {
                return GType.INT;
            } else if (fld is GFloat) {
                return GType.FLOAT;
            } else if (fld is GDword64) {
                return GType.DWORD64;
            } else if (fld is GInt64) {
                return GType.INT64;
            } else if (fld is GDouble) {
                return GType.DOUBLE;
            } else if (fld is GCExoString) {
                return GType.CEXOSTRING;
            } else if (fld is GResRef) {
                return GType.RESREF;
            } else if (fld is GCExoLocString) {
                return GType.CEXOLOCSTRING;
            } else if (fld is GVoid) {
                return GType.VOID;
            } else if (fld is GStruct) {
                return GType.STRUCT;
            } else if (fld is GList) {
                return GType.LIST;
            } else {
                return GType.INVALID;
            }
        }
        public static bool isComplex(GType type) {
            switch (type) {
                case GType.BYTE:
                case GType.CHAR:
                case GType.WORD:
                case GType.SHORT:
                case GType.DWORD:
                case GType.INT:
                case GType.FLOAT:
                    return false;
                default:
                    return true;
            }
        }
        public static bool isComposite(GField fld) {
            return (fld is GCompositeField);
        }

        private void analyseField(GField fld) {
            l_fld.Add(fld);
            if (isComplex(getType(fld))) {
                l_complex_fld.Add(fld);
                if (isComposite(fld)) {
                    GCompositeField cpsit = (GCompositeField)fld;
                    l_cpsit_fld.Add(fld);
                    if (cpsit is GStruct) {
                        l_str.Add((GStruct)cpsit);
                    } else {
                        l_lst.Add((GList)cpsit);
                    }
                    foreach (GField child in cpsit.get()) {
                        analyseField(child);
                    }
                } else {
                    l_large_fld.Add(fld);
                }
            } else {
                l_simple_fld.Add(fld);
            }
            if (!l_label.Contains(fld.Label)) {
                l_label.Add(fld.Label);
            }
        }
    }
}
