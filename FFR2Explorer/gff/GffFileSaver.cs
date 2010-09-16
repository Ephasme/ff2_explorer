using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using System.Collections;
using Bioware.Virtual;

using DWORD = System.UInt32;

namespace Bioware.GFF {
    public class GffFileSaver {
        GffHeaderStr _h;

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

        Dictionary<VStruct, long> dp_str;
        Dictionary<VField, long> dp_fld;
        Dictionary<string, Position> dp_lab;


        Dictionary<VField, Position> dl_FldToFDatas;
        Dictionary<VStruct, long> dl_StrToFIdc;
        Dictionary<VList, long> dl_LstTLInd;
        
        /*#region Dictionnaires de positionnement.
                //Dictionary<VField, int> dp_fld_str_id;
                #endregion*/

        #region Listes des structures.
        List<GffStructStr> l_sstr;
        List<GffFieldStr> l_fstr;
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

        public GffFileSaver(VStruct root, string path, string ext) {
            initVars(root, path, ext);
        }
        private void initVars(VStruct root, string path, string ext) {
            this.root = root;
            this.path = path;
            this.ext = ext;
            this.ver = GffHeaderStr.VERSION;
            _ascii = new ASCIIEncoding();
            _h = (new GffHeaderStr()).init();
            if (File.Exists(path)) {
                _fs = File.Open(path, FileMode.Truncate);
            } else {
                _fs = File.Open(path, FileMode.CreateNew);
            }
            _bw = new BinaryWriter(_fs);
            dp_str = new Dictionary<VStruct, long>();
            dp_fld = new Dictionary<VField, long>();
            dp_lab = new Dictionary<string, Position>();

            dl_FldToFDatas = new Dictionary<VField, Position>();
            dl_StrToFIdc = new Dictionary<VStruct, long>();
            dl_LstTLInd = new Dictionary<VList, long>();

            l_fstr = new List<GffFieldStr>();
            l_sstr = new List<GffStructStr>();

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
            _bw.Seek(GffHeaderStr.SIZE, SeekOrigin.Begin);
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
            _h.Infos[GffHeaderStr.STRUCT_OFFSET] = (DWORD)_bw.BaseStream.Position;
            int count = 0;
            foreach (VStruct str in l_str) {
                _bw.Seek(((int)_h.Infos[GffHeaderStr.STRUCT_OFFSET]) + str.Index * GffConst.STRUCT_SIZE, SeekOrigin.Begin);
                long offset = _bw.BaseStream.Position;
                if (writeStruct(str)) {
                    dp_str.Add(str, offset);
                    count++;
                }
            }
            _h.Infos[GffHeaderStr.STRUCT_COUNT] = (DWORD)count;
        }
        private void writeAllFields() {
            _h.Infos[GffHeaderStr.FIELD_OFFSET] = (DWORD)_bw.BaseStream.Position;
            int count = 0;
            foreach (VField fld in l_fld) {
                _bw.Seek(((int)_h.Infos[GffHeaderStr.FIELD_OFFSET]) + fld.Index * GffConst.STRUCT_SIZE, SeekOrigin.Begin);
                long offset = _bw.BaseStream.Position;
                if (writeField(fld)) {
                    dp_fld.Add(fld, offset);
                    count++;
                }
            }
            _h.Infos[GffHeaderStr.FIELD_COUNT] = (DWORD)count;
        }

        private void writeLabels() {
            int index = 0;
            _h.Infos[GffHeaderStr.LABEL_OFFSET] = (DWORD)_bw.BaseStream.Position;
            foreach (string label in l_label) {
                long label_offset = _bw.BaseStream.Position;
                string normalized_label = label.PadRight(GffConst.LABEL_LENGTH, '\0');
                dp_lab.Add(normalized_label, new Position(index, label_offset));
                _bw.Write(normalized_label.ToCharArray());
                index++;
            }
            _h.Infos[GffHeaderStr.LABEL_COUNT] = (DWORD)index;
        }
        private void writeFieldDatas() {
            int index = 0;
            _h.Infos[GffHeaderStr.FIELD_DATA_OFFSET] = (DWORD)_bw.BaseStream.Position;
            foreach (VField lfld in l_large_fld) {
                dl_FldToFDatas.Add(lfld, new Position(index++, _bw.BaseStream.Position));
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
                        _bw.Write((DWORD)_ascii.GetByteCount(str));
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
            _h.Infos[GffHeaderStr.FIELD_DATA_COUNT] = ((DWORD)_bw.BaseStream.Position) - _h.Infos[GffHeaderStr.FIELD_DATA_OFFSET];
        }
        private void writeCExoLocString(VExoLocString exlstr) {
            List<byte> buffer = new List<byte>();
            foreach (KeyValuePair<int, string> kvp in exlstr.Value) {
                buffer.AddRange(BitConverter.GetBytes((DWORD)kvp.Key));
                buffer.AddRange(BitConverter.GetBytes((DWORD)kvp.Value.Length));
                buffer.AddRange(_ascii.GetBytes(kvp.Value));
            }
            _bw.Write(buffer.Count + sizeof(DWORD) * 2);
            _bw.Write((DWORD)exlstr.StringRef);
            _bw.Write((DWORD)exlstr.Value.Count);
            _bw.Write(buffer.ToArray());
        }
        private void writeFieldIndices() {
            _h.Infos[GffHeaderStr.FIELD_INDICES_OFFSET] = (DWORD)_bw.BaseStream.Position;
            foreach (VStruct str in l_str) {
                if (str.get().Count > 1) {
                    dl_StrToFIdc.Add(str, _bw.BaseStream.Position);
                    foreach (VField fld in str.get()) {
                        _bw.Write(fld.Index);
                    }
                }
            }
            _h.Infos[GffHeaderStr.FIELD_INDICES_COUNT] = ((DWORD)_bw.BaseStream.Position) - _h.Infos[GffHeaderStr.FIELD_INDICES_OFFSET];
        }
        private void writeListIndices() {
            _h.Infos[GffHeaderStr.LIST_INDICES_OFFSET] = (DWORD)_bw.BaseStream.Position;
            foreach (VList lst in l_lst) {
                dl_LstTLInd.Add(lst, _bw.BaseStream.Position);
                _bw.Write(lst.get().Count);
                foreach (VField fld in lst.get()) {
                    VStruct str = (VStruct)fld;
                    _bw.Write(str.Index);
                }
            }
            _h.Infos[GffHeaderStr.LIST_INDICES_COUNT] = ((DWORD)_bw.BaseStream.Position) - _h.Infos[GffHeaderStr.LIST_INDICES_OFFSET];
        }
        private void writeHeader() {
            long pos = _bw.BaseStream.Position;
            _bw.BaseStream.Position = 0;
            ext = ext.ToUpper();
            ext = ext.Remove(0, 1);
            ext = ext.PadRight(GffHeader.FILE_TYPE_SIZE);
            _bw.Write(ext.ToCharArray());
            _bw.Write(ver.ToCharArray());
            _bw.Write(_h.Infos[GffHeader.STRUCT_OFFSET]);
            _bw.Write(_h.Infos[GffHeader.STRUCT_COUNT]);
            _bw.Write(_h.Infos[GffHeader.FIELD_OFFSET]);
            _bw.Write(_h.Infos[GffHeader.FIELD_COUNT]);
            _bw.Write(_h.Infos[GffHeader.LABEL_OFFSET]);
            _bw.Write(_h.Infos[GffHeader.LABEL_COUNT]);
            _bw.Write(_h.Infos[GffHeader.FIELD_DATA_OFFSET]);
            _bw.Write(_h.Infos[GffHeader.FIELD_DATA_COUNT]);
            _bw.Write(_h.Infos[GffHeader.FIELD_INDICES_OFFSET]);
            _bw.Write(_h.Infos[GffHeader.FIELD_INDICES_COUNT]);
            _bw.Write(_h.Infos[GffHeader.LIST_INDICES_OFFSET]);
            _bw.Write(_h.Infos[GffHeader.LIST_INDICES_COUNT]);
            _bw.BaseStream.Position = pos;
        }

        private void linkStructs() {
            // On relie toutes les structures.
            foreach (KeyValuePair<VStruct, long> kvp in dp_str) {
                VStruct str = kvp.Key;
                long str_offset = kvp.Value;
                int str_index = kvp.Key.Index;
                long saved_pos = _bw.BaseStream.Position;
                _bw.BaseStream.Position = str_offset + sizeof(DWORD);
                if (str.get().Count > 1) {
                    _bw.Write((DWORD)dl_StrToFIdc[str] - _h.Infos[GffHeaderStr.FIELD_INDICES_OFFSET]);
                } else {
                    _bw.Write((DWORD)str.get()[0].Index);
                }
                _bw.BaseStream.Position = saved_pos;
            }
        }
        private void linkFields() {
            foreach (KeyValuePair<VField, long> kvp in dp_fld) {
                VField fld = kvp.Key;
                long offset = kvp.Value;
                int index = kvp.Key.Index;
                long pos = _bw.BaseStream.Position;
                _bw.BaseStream.Position = offset + 2 * sizeof(DWORD);
                if (fld is VStruct) {
                    VStruct str = (VStruct)fld;
                    int s_index = str.Index;
                    _bw.Write((DWORD)s_index);
                } else if (fld is VList) {
                    VList lst = (VList)fld;
                    DWORD l_pos = (DWORD) dl_LstTLInd[lst] - _h.Infos[GffHeaderStr.LIST_INDICES_OFFSET];
                    _bw.Write((DWORD)l_pos);
                } else {
                    if (dl_FldToFDatas.ContainsKey(fld)) {
                        DWORD fd_pos = (DWORD)dl_FldToFDatas[fld].offset - _h.Infos[GffHeaderStr.FIELD_DATA_OFFSET];
                        _bw.Write((DWORD)fd_pos);
                    }
                }
                _bw.BaseStream.Position = pos;
            }
        }

        private GffFieldStr createFieldStr(VField fld) {
            // On crée la structure à renvoyer.
            GffFieldStr result = new GffFieldStr();
            // On crée les variables qui vont accueuillir les données de la structure.
            byte[] dataOrOffset = new byte[sizeof(DWORD)];
            int labelIndex = l_label.IndexOf(fld.Label);
            VType type = fld.Type;
            // On détermine l'algorithme en fonction du typage du champ.
            if (fld is VCpsitField) {
                if (fld is VStruct) {
                    // ATTENTION ! Ce code ne devrait JAMAIS ÊTRE ATTEINT si la structure RACINE, ou ENFANT D'UNE LISTE !
                    // Dans le cas contraire, on défini la valeur de DataOrOffset comme étant l'index de la structure
                    // dans la liste des structures.
                    VStruct str = (VStruct)fld;
                    dataOrOffset = BitConverter.GetBytes((DWORD)l_str.IndexOf(str));
                    labelIndex = (labelIndex == -1) ? (0) : (labelIndex);
                } else {
                    // Dans le cas d'une liste, on liera avec la liste d'indices de champ après.
                    dataOrOffset = BitConverter.GetBytes((DWORD)0);
                }
            } else {
                if (VField.isComplex(result.Type)) {
                    // On liera avec ses données plus tard.
                    dataOrOffset = BitConverter.GetBytes((DWORD)0);
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

            // On vérifie que la taille du tableau de bytes est bien de taille 'sizeof(DWORD)'
            if (dataOrOffset.Length < sizeof(DWORD)) {
                Array.Resize<byte>(ref dataOrOffset, sizeof(DWORD));
            } else if (dataOrOffset.Length > sizeof(DWORD)) {
                // Erreur ! Tableau trop grand !
            }
            // On stocke les données dans la structure.
            result.Type = type;
            result.LabelIndex = (DWORD)labelIndex;
            result.DataOrOffset = BitConverter.ToUInt32(dataOrOffset, 0);
            // On renvoie la structure.
            return result;
        }
        private void writeFieldStr(GffFieldStr sfld) {
            // Ne pas changer d'ordre !
            _bw.Write((DWORD)sfld.Type);
            _bw.Write((DWORD)sfld.LabelIndex);
            _bw.Write((DWORD)sfld.DataOrOffset);
        }
        private bool writeField(VField fld) {
            // Dans le cas spécial d'une structure, on n'écrit un champ associé uniquement
            // si la structure est possédée par une autre structure et n'est pas racine.
            if (!(fld is VStruct) || (fld is VStruct && (fld.Owner != null && fld.Owner is VStruct))) {
                writeFieldStr(createFieldStr(fld));
                return true;
            }
            return false;
        }

        private GffStructStr createStructStr(VStruct str) {
            GffStructStr sstr = new GffStructStr();
            sstr.FieldCount = (DWORD)str.get().Count;
            sstr.Type = (str.IsRoot) ? (GffStructStr.ROOT_TYPE) : (GffStructStr.STANDARD_TYPE);
            sstr.DataOrOffset = 0;
            return sstr;
        }
        private void writeStructSTR(GffStructStr sstr) {
            // Ne pas changer d'ordre !
            _bw.Write((DWORD)sstr.Type);
            _bw.Write((DWORD)sstr.DataOrOffset);
            _bw.Write((DWORD)sstr.FieldCount);
        }
        private bool writeStruct(VStruct str) {
            writeStructSTR(createStructStr(str));
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
