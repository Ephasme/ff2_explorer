namespace Bioware.GFF
{
    public class GffHeader
    {
        public const int FileTypeSize = 4;
        public const int FileVersionSize = 4;
        public const int DwordTableSize = 12;
        public const int Size = DwordTableSize*sizeof (uint) + FileTypeSize + FileVersionSize;

        public GffHeader()
        {
            Infos = new uint[DwordTableSize];
        }

        public uint[] Infos { get; }
        public string Type { private set; get; }
        public string Version { private set; get; }

        /// <summary>
        ///     Accès au type de fichier.
        /// </summary>
        public string FileType
        {
            get { return Type; }
            set { Type = value; }
        }

        /// <summary>
        ///     Accès à la version de la structure GFF (toujours V3.2).
        /// </summary>
        public string FileVersion
        {
            get { return Version; }
            set { Version = value; }
        }

        /// <summary>
        ///     Accès à la position de la première structure.
        /// </summary>
        public uint StructOffset
        {
            get { return Infos[0]; }
            set { Infos[0] = value; }
        }

        /// <summary>
        ///     Accès au nombre de structures.
        /// </summary>
        public uint StructCount
        {
            get { return Infos[1]; }
            set { Infos[1] = value; }
        }

        /// <summary>
        ///     Accès à la position du premier champ.
        /// </summary>
        public uint FieldOffset
        {
            get { return Infos[2]; }
            set { Infos[2] = value; }
        }

        /// <summary>
        ///     Accès au nombre de champ.
        /// </summary>
        public uint FieldCount
        {
            get { return Infos[3]; }
            set { Infos[3] = value; }
        }

        /// <summary>
        ///     Accès à la position du premier champ.
        /// </summary>
        public uint LabelOffset
        {
            get { return Infos[4]; }
            set { Infos[4] = value; }
        }

        /// <summary>
        ///     Accès au nombre de champ.
        /// </summary>
        public uint LabelCount
        {
            get { return Infos[5]; }
            set { Infos[5] = value; }
        }

        /// <summary>
        ///     Accès à la position du premier champ complexe.
        /// </summary>
        public uint FieldDataOffset
        {
            get { return Infos[6]; }
            set { Infos[6] = value; }
        }

        /// <summary>
        ///     Accès au nombre de champs complexes.
        /// </summary>
        public uint FieldDataCount
        {
            get { return Infos[7]; }
            set { Infos[7] = value; }
        }

        /// <summary>
        ///     Accès à la position du premier indice de champ.
        /// </summary>
        public uint FieldIndicesOffset
        {
            get { return Infos[8]; }
            set { Infos[8] = value; }
        }

        /// <summary>
        ///     Accès au nombre d'indice de champ.
        /// </summary>
        public uint FieldIndicesCount
        {
            get { return Infos[9]; }
            set { Infos[9] = value; }
        }

        /// <summary>
        ///     Accès à la position de la première liste d'indices.
        /// </summary>
        public uint ListIndicesOffset
        {
            get { return Infos[10]; }
            set { Infos[10] = value; }
        }

        /// <summary>
        ///     Accès à la liste des indices de liste.
        /// </summary>
        public uint ListIndicesCount
        {
            get { return Infos[11]; }
            set { Infos[11] = value; }
        }

        public void Load(LatinBinaryReader br)
        {
            var pos = br.Stream.Position;
            br.Stream.Position = 0;
            Type = new string(br.ReadChars(FileTypeSize)).Trim();
            Version = new string(br.ReadChars(FileVersionSize)).Trim();

            var q = br.GetUInt32Queue(DwordTableSize);
            var i = 0;
            while (q.Count > 0)
            {
                Infos[i++] = q.Dequeue();
            }
            br.Stream.Position = pos;
        }
    }
}