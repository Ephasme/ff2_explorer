using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using DWORD = System.UInt32;

namespace FFR2Explorer {
    public class GBinaryReader : BinaryReader {

        /// <summary>
        /// Accès au stream associé à ce lecteur.
        /// </summary>
        public FileStream Stream { set; get; }

        /// <summary>
        /// Lecteur de données binaires spécifique aux fichiers GFF.
        /// </summary>
        /// <param name="stream">Stream d'un fichier GFF.</param>
        public GBinaryReader(FileStream stream)
            : base(stream) {
            Stream = stream;
        }

        /// <summary>
        /// Lit et renvoie une valeur DWORD.
        /// </summary>
        public DWORD ReadDWORD() {
            return ReadUInt32();
        }

        /// <summary>
        /// Renvoie une queue de DWORD contenant [count] valeurs.
        /// </summary>
        /// <param name="count">Nombre de DWORD que la fonction doit lire et stocker.</param>
        public Queue<DWORD> EnqueueDWORDs(int count) {
            Queue<DWORD> q = new Queue<DWORD>(count);
            for (int i = 0; i < count; i++) {
                q.Enqueue(ReadDWORD());
            }
            return q;
        }

        /// <summary>
        /// Renvoie une liste de DWORD contenant [count] valeurs.
        /// </summary>
        /// <param name="count">Nombre de DWORD que la fonction doit lire et stocker.</param>
        public List<DWORD> ListDWORDS(int count) {
            List<DWORD> l = new List<DWORD>(count);
            for (int i = 0; i < count; i++) {
                l.Add(ReadDWORD());
            }
            return l;
        }
    }
}
