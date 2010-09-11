using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;

namespace FFR2Explorer {
    /// <summary>
    /// Classe permettant de gérer l'en-tête.
    /// </summary>
    public class GHeader {
        /// <summary>
        /// Structure contenant les données de l'en-tête.
        /// </summary>
        #region Constantes du tableau des données de l'en-tête.
        public const int STRUCT_OFFSET = 0;
        public const int STRUCT_COUNT = 1;
        public const int FIELD_OFFSET = 2;
        public const int FIELD_COUNT = 3;
        public const int LABEL_OFFSET = 4;
        public const int LABEL_COUNT = 5;
        public const int FIELD_DATA_OFFSET = 6;
        public const int FIELD_DATA_COUNT = 7;
        public const int FIELD_INDICES_OFFSET = 8;
        public const int FIELD_INDICES_COUNT = 9;
        public const int LIST_INDICES_OFFSET = 10;
        public const int LIST_INDICES_COUNT = 11;
        #endregion
        #region Constantes d'information sur la structure.
        public const int FILE_TYPE_SIZE = 4;
        public const int FILE_VERSION_SIZE = 4;
        public const int DWORD_TABLE_SIZE = 12;
        public const int SIZE = DWORD_TABLE_SIZE * sizeof(DWORD) + (FILE_TYPE_SIZE + FILE_VERSION_SIZE) * sizeof(char);
        #endregion
        #region Variables contenant les données de l'en-tête.
        public UInt32[] Infos { private set; get; }
        public char[] Type { private set; get; }
        public char[] Version { private set; get; }
        #endregion

        public GHeader(GBinaryReader reader){
            init(reader);
            load(reader);
        }

        /// <summary>
        /// Initialise les structures de données.
        /// </summary>
        private void init(GBinaryReader reader) {
            Infos = new DWORD[DWORD_TABLE_SIZE];
            Type = new char[FILE_TYPE_SIZE];
            Version = new char[FILE_VERSION_SIZE];
        }
        /// <summary>
        /// Chargement en mémoire des informations de l'en-tête du fichier.
        /// </summary>
        protected void load(GBinaryReader reader) {
            long pos = reader.BaseStream.Position;
            reader.BaseStream.Position = 0;
            Type = reader.ReadChars(FILE_TYPE_SIZE);
            Version = reader.ReadChars(FILE_VERSION_SIZE);
            Queue<DWORD> q = reader.EnqueueDWORDs(DWORD_TABLE_SIZE);
            for (int i = 0; i < DWORD_TABLE_SIZE; i++) {
                Infos[i] = q.Dequeue();
            }
        }

        #region Propriétés d'accès à l'en-tête
        /// <summary>
        /// Accès au type de fichier.
        /// </summary>
        public char[] FileType {
            get {
                return Type;
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la version de la structure GFF (toujours V3.2).
        /// </summary>
        public char[] FileVersion {
            get {
                return Version;
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la position de la première structure.
        /// </summary>
        public DWORD StructOffset {
            get {
                return Infos[STRUCT_OFFSET];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès au nombre de structures.
        /// </summary>
        public DWORD StructCount {
            get {
                return Infos[STRUCT_COUNT];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la position du premier champ.
        /// </summary>
        public DWORD FieldOffset {
            get {
                return Infos[FIELD_OFFSET];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès au nombre de champ.
        /// </summary>
        public DWORD FieldCount {
            get {
                return Infos[FIELD_COUNT];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la position du premier champ.
        /// </summary>
        public DWORD LabelOffset {
            get {
                return Infos[LABEL_OFFSET];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès au nombre de champ.
        /// </summary>
        public DWORD LabelCount {
            get {
                return Infos[LABEL_COUNT];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la position du premier champ complexe.
        /// </summary>
        public DWORD FieldDataOffset {
            get {
                return Infos[FIELD_DATA_OFFSET];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès au nombre de champs complexes.
        /// </summary>
        public DWORD FieldDataCount {
            get {
                return Infos[FIELD_DATA_COUNT];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la position du premier indice de champ.
        /// </summary>
        public DWORD FieldIndicesOffset {
            get {
                return Infos[FIELD_INDICES_OFFSET];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès au nombre d'indice de champ.
        /// </summary>
        public DWORD FieldIndicesCount {
            get {
                return Infos[FIELD_INDICES_COUNT];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la position de la première liste d'indices.
        /// </summary>
        public DWORD ListIndicesOffset {
            get {
                return Infos[LIST_INDICES_OFFSET];
            }
            private set {

            }
        }
        /// <summary>
        /// Accès à la liste des indices de liste.
        /// </summary>
        public DWORD ListIndicesCount {
            get {
                return Infos[LIST_INDICES_COUNT];
            }
            private set {

            }
        }
        #endregion

    }
}
