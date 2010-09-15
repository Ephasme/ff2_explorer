using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;
using System.Collections;
using Bioware.Virtual;

namespace Bioware.GFF {
    public class GFileReader {
        #region Variables.
        private GBinaryReader _rd;
        private GHeader _h;
        private GFieldFactory _ff;

        private List<GStructSTR> l_sstr;
        private List<VStruct> l_str;
        private List<GFieldSTR> l_fstr;
        private List<VField> l_fld;

        private Dictionary<VStruct, List<uint>> dic_str;
        private Dictionary<VList, List<uint>> dic_lst;

        private List<string> l_label;
        #endregion

        public GFileReader(string path) {
            if (File.Exists(path)) {
                initVars(new GBinaryReader(File.OpenRead(path)));
                read();
                load();
                setHierarchy();
            }
        }

        public static List<VField> getOrphelins(List<VField> l_fld, List<VStruct> l_str) {
            List<VField> l = new List<VField>();
            foreach (VField fld in l_fld) {
                if (fld.Owner == null) {
                    l.Add(fld);
                }
            }
            foreach (VStruct str in l_str) {
                if (str.Owner == null) {
                    l.Add((VField)str);
                }
            }
            return l;
        }
        
        public VStruct getRootStruct() {
            return l_str[GConst.ROOT_STRUCT_INDEX];
        }

        #region Méthodes privées.

        #region Initialisation des variables.
        private void initVars(GBinaryReader gbRead) {
            this._rd = gbRead;
            _h = new GHeader(_rd);
            _ff = new GFieldFactory(_rd, _h);
            l_sstr = new List<GStructSTR>();
            l_str = new List<VStruct>();
            l_fstr = new List<GFieldSTR>();
            l_fld = new List<VField>();
            l_label = new List<string>();
            dic_str = new Dictionary<VStruct, List<uint>>();
            dic_lst = new Dictionary<VList, List<uint>>();
        }
        #endregion
        #region Méthodes de lecture du fichier.
        private void read() {
            readStructs();
            readFields();
            readLabels();
        }
        private void readStructs() {
            _rd.Stream.Position = _h.StructOffset;
            for (int i = 0; i < _h.StructCount; i++) {
                Queue<DWORD> q = _rd.EnqueueDWORDs(GConst.STRUCT_VALUE_COUNT);
                GStructSTR sstr = new GStructSTR();
                sstr.Type = q.Dequeue();
                sstr.DataOrOffset = q.Dequeue();
                sstr.FieldCount = q.Dequeue();
                l_sstr.Add(sstr);
            }
        }
        private void readFields() {
            _rd.Stream.Position = _h.FieldOffset;
            for (int i = 0; i < _h.FieldCount; i++) {
                Queue<DWORD> q = _rd.EnqueueDWORDs(GConst.STRUCT_VALUE_COUNT);
                GFieldSTR fstr = new GFieldSTR();
                fstr.Type = (VType)q.Dequeue();
                fstr.LabelIndex = q.Dequeue();
                fstr.DataOrOffset = q.Dequeue();
                l_fstr.Add(fstr);
            }
        }
        private void readLabels() {
            char[] trim = { '\0' };
            _rd.Stream.Seek(_h.LabelOffset, SeekOrigin.Begin);
            for (int i = 0; i < _h.LabelCount; i++) {
                string lab = new String(_rd.ReadChars(GConst.LABEL_LENGTH));
                lab = lab.TrimEnd(trim);
                l_label.Add(lab);
            }
        }
        #endregion
        #region Méthodes de chargement des données.
        private void load() {
            loadStructs();
            loadFields();
        }
        /// <summary>
        /// Cette méthode va charger en mémoire des objets correspondants aux structures trouvées dans le fichier GFF.
        /// </summary>
        private void loadStructs() {
            foreach (GStructSTR sstr in l_sstr) {
                VStruct str = _ff.createStruct();
                l_str.Add(str);
                List<uint> idxs = getFieldIndices(sstr);
                dic_str.Add(str, idxs);
            }
        }
        /// <summary>
        /// Cette méthode va instancier les classes représentatives des champs d'un fichier GFF dans une liste en mémoire.
        /// </summary>
        private void loadFields() {
            /* Pour chaque structure préalablement lue dans le fichier, on effectue une boucle. */
            foreach (GFieldSTR fstr in l_fstr) {
                VField fld; // On crée une instance nulle qui recevra les données du fichier.
                if (fstr.Type == VType.STRUCT) {
                    /* Si le type trouvé dans les données lues est "structure". */
                    VStruct str = l_str[(int)fstr.DataOrOffset]; // On récupère la structure car elle a déjà été modélisée.
                    str.setLabel(_ff.getLabel(fstr)); // On défini son label en fonction des données trouvées dans le fichier.
                    fld = (VField)str; // On la caste vers un format inférieur pour pouvoir la stocker dans la liste de champs.
                } else {
                    /* Si le type n'est pas une structure. */
                    fld = _ff.createField(fstr); // On fait générer un objet correspondant au type trouvé dans le fichier.
                }
                l_fld.Add(fld); // On ajoute cet objet à la liste.
                if (fstr.Type == VType.LIST) {
                    /* Si le type trouvé dans les données lues est "liste". */
                    List<uint> idxs = getStructIndexes(fstr); // On fait la liste de tous les champs qu'elle contient.
                    dic_lst.Add((VList)fld, idxs); // On l'ajoute dans un dictionnaire avec cette liste.
                }
            }
        }
        #endregion
        #region Méthodes d'organisation de la hiérachie.
        private void setHierarchy() {
            setStructHierarchy();
            setListHierarchy();
        }
        private void setListHierarchy() {
            foreach (KeyValuePair<VList, List<uint>> kvp in dic_lst) {
                VList lst = kvp.Key;
                List<uint> idxs = kvp.Value;
                foreach (uint idx in idxs) {
                    lst.add(l_str[(int)idx]);
                }
            }
        }
        private void setStructHierarchy() {
            foreach (KeyValuePair<VStruct, List<uint>> kvp in dic_str) {
                VStruct str = kvp.Key;
                List<uint> idxs = kvp.Value;
                foreach (uint idx in idxs) {
                    VField fld = l_fld[(int)idx];
                    str.add(l_fld[(int)idx]);
                }
            }
        }
        #endregion
        #region Méthodes de récupération de listes d'indices.
        public List<uint> getFieldIndices(GStructSTR s_str) {
            List<uint> l_fld_i = new List<uint>();
            if (s_str.FieldCount == 1) {
                l_fld_i.Add(s_str.DataOrOffset);
            } else {
                _rd.Stream.Position = _h.FieldIndicesOffset + s_str.DataOrOffset;
                l_fld_i = _rd.ListDWORDS((int)s_str.FieldCount);
            }
            return l_fld_i;
        }
        public List<uint> getStructIndexes(GFieldSTR s_list) {
            if (s_list.Type == VType.LIST) {
                _rd.Stream.Position = _h.ListIndicesOffset + s_list.DataOrOffset;
                int size = (int)_rd.ReadDWORD();
                List<uint> l_idx = new List<uint>(size);
                l_idx = _rd.ListDWORDS(size);
                return l_idx;
            }
            return null;
        }
        #endregion

        #endregion
    }
}
