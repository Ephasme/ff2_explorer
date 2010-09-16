using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;
using System.Collections;
using Bioware.Virtual;

namespace Bioware.GFF {
    public class GffFileReader : IFileReader {

        #region Variables.
        private GffBinaryReader _rd;
        private GffHeader _h;
        private GffFieldFactory _ff;

        private Dictionary<int, GffStructStr> dic_sstr;
        private Dictionary<int, VStruct> dic_str;
        private Dictionary<int, GffFieldStr> dic_fstr;
        private Dictionary<int, VField> dic_fld;

        private Dictionary<VStruct, List<DWORD>> dic_str_childs;
        private Dictionary<VList, List<DWORD>> dic_lst_childs;

        private Dictionary<int, string> dic_label;
        #endregion

        public GffFileReader(string path) {
            if (File.Exists(path)) {
                initVars(new GffBinaryReader(File.OpenRead(path)));
                read();
                load();
                setHierarchy();
            }
        }
        public VStruct getRootStruct() {
            return dic_str[GffConst.ROOT_STRUCT_INDEX];
        }

        #region Méthodes privées.

        #region Initialisation des variables.
        private void initVars(GffBinaryReader gbRead) {
            this._rd = gbRead;
            _h = new GffHeader(_rd);
            _ff = new GffFieldFactory(_rd, _h);
            dic_sstr = new Dictionary<int, GffStructStr>();
            dic_str = new Dictionary<int, VStruct>();
            dic_fstr = new Dictionary<int, GffFieldStr>();
            dic_fld = new Dictionary<int, VField>();
            dic_label = new Dictionary<int, string>();
            dic_str_childs = new Dictionary<VStruct, List<DWORD>>();
            dic_lst_childs = new Dictionary<VList, List<DWORD>>();
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
                Queue<DWORD> q = _rd.EnqueueDWORDs(GffConst.STRUCT_VALUE_COUNT);
                GffStructStr sstr = new GffStructStr();
                sstr.Type = q.Dequeue();
                sstr.DataOrOffset = q.Dequeue();
                sstr.FieldCount = q.Dequeue();
                dic_sstr.Add(i, sstr);
            }
        }
        private void readFields() {
            _rd.Stream.Position = _h.FieldOffset;
            for (int i = 0; i < _h.FieldCount; i++) {
                Queue<DWORD> q = _rd.EnqueueDWORDs(GffConst.STRUCT_VALUE_COUNT);
                GffFieldStr fstr = new GffFieldStr();
                fstr.Type = (VType)q.Dequeue();
                fstr.LabelIndex = q.Dequeue();
                fstr.DataOrOffset = q.Dequeue();
                dic_fstr.Add(i, fstr);
            }
        }
        private void readLabels() {
            char[] trim = { '\0' };
            _rd.Stream.Seek(_h.LabelOffset, SeekOrigin.Begin);
            for (int i = 0; i < _h.LabelCount; i++) {
                string lab = new String(_rd.ReadChars(GffConst.LABEL_LENGTH));
                lab = lab.TrimEnd(trim);
                dic_label.Add(i, lab);
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
            foreach (KeyValuePair<int, GffStructStr> kvp in dic_sstr) {
                VStruct str = _ff.createStruct(kvp.Key);
                dic_str.Add(kvp.Key, str);
                List<DWORD> idxs = getFieldIndices(kvp.Value);
                dic_str_childs.Add(str, idxs);
            }
        }
        /// <summary>
        /// Cette méthode va instancier les classes représentatives des champs d'un fichier GFF dans une liste en mémoire.
        /// </summary>
        private void loadFields() {
            /* Pour chaque structure préalablement lue dans le fichier, on effectue une boucle. */
            foreach (KeyValuePair<int, GffFieldStr> kvp in dic_fstr) {
                int index = kvp.Key;
                GffFieldStr fstr = kvp.Value;
                VField fld; // On crée une instance nulle qui recevra les données du fichier.
                if (fstr.Type == VType.STRUCT) {
                    /* Si le type trouvé dans les données lues est "structure". */
                    VStruct str = dic_str[(int)fstr.DataOrOffset]; // On récupère la structure car elle a déjà été modélisée.
                    str.setLabel(_ff.getLabel(fstr)); // On défini son label en fonction des données trouvées dans le fichier.
                    fld = (VField)str; // On la caste vers un format inférieur pour pouvoir la stocker dans la liste de champs.
                } else {
                    /* Si le type n'est pas une structure. */
                    fld = _ff.createField(fstr, index); // On fait générer un objet correspondant au type trouvé dans le fichier.
                }
                dic_fld.Add(index, fld); // On ajoute cet objet à la liste.
                if (fstr.Type == VType.LIST) {
                    /* Si le type trouvé dans les données lues est "liste". */
                    List<DWORD> idxs = getStructIndexes(fstr); // On fait la liste de tous les champs qu'elle contient.
                    dic_lst_childs.Add((VList)fld, idxs); // On l'ajoute dans un dictionnaire avec cette liste.
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
            foreach (KeyValuePair<VList, List<DWORD>> kvp in dic_lst_childs) {
                VList lst = kvp.Key;
                List<DWORD> idxs = kvp.Value;
                foreach (DWORD idx in idxs) {
                    lst.add(dic_str[(int)idx]);
                }
            }
        }
        private void setStructHierarchy() {
            foreach (KeyValuePair<VStruct, List<DWORD>> kvp in dic_str_childs) {
                VStruct str = kvp.Key;
                List<DWORD> idxs = kvp.Value;
                foreach (DWORD idx in idxs) {
                    VField fld = dic_fld[(int)idx];
                    str.add(dic_fld[(int)idx]);
                }
            }
        }
        #endregion
        #region Méthodes de récupération de listes d'indices.
        public List<DWORD> getFieldIndices(GffStructStr s_str) {
            List<DWORD> l_fld_i = new List<DWORD>();
            if (s_str.FieldCount == 1) {
                l_fld_i.Add(s_str.DataOrOffset);
            } else {
                _rd.Stream.Position = _h.FieldIndicesOffset + s_str.DataOrOffset;
                l_fld_i = _rd.ListDWORDS((int)s_str.FieldCount);
            }
            return l_fld_i;
        }
        public List<DWORD> getStructIndexes(GffFieldStr s_list) {
            if (s_list.Type == VType.LIST) {
                _rd.Stream.Position = _h.ListIndicesOffset + s_list.DataOrOffset;
                int size = (int)_rd.ReadDWORD();
                List<DWORD> l_idx = new List<DWORD>(size);
                l_idx = _rd.ListDWORDS(size);
                return l_idx;
            }
            return null;
        }
        #endregion

        #endregion
        /* Fonction utilisée pour le débogage.
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
        }*/

    }
}
