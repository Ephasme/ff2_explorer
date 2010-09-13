using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

using DWORD = System.Int32;

namespace FFR2Explorer {
    public class GFile {

        #region Constantes.
        public const string BACKUP_EXT = ".back";
        public const string XML_EXT = ".xml";
        #endregion

        #region Propriétés.
        public GHeader Header { private set; get; }
        public String FilePath { private set; get; }
        public GBinaryReader GBinReader { private set; get; }
        public BinaryWriter BinWriter { private set; get; }
        public FileStream FileStrm { private set; get; }
        #endregion

        public GFile(String path) {
            if (File.Exists(path)) {
                FilePath = path;
                FileStrm = new FileStream(path, FileMode.Open);
                GBinReader = new GBinaryReader(FileStrm);
                BinWriter = new BinaryWriter(FileStrm);
                Header = new GHeader(GBinReader);
            }
        }

        /*public void saveToGFF(bool backup) {
/*            FileStrm.Close();
            FileStrm = File.Create(FilePath + ".newGFF");
            BinaryWriter bw = new BinaryWriter(FileStrm);

            GList<byte[]> listIndexes = new GList<byte[]>();

            analyseField(rt);

            foreach (GList list in listList) {
                bw.Write((uint)list.Childs.Count);
                foreach (GStruct str in list.Childs) {
                    byte[] index = BitConverter.GetBytes(structList.IndexOf(str));
                }
            }
        }

        public void analyseField(GField fld) {
            #region Initialisation des listes.
            structList = new GList<GStruct>();
            listList = new GList<GList>();
            fieldList = new GList<GField>();
            labelList = new GList<string>();
            #endregion
            if (fld is GCompositeField) {
                GCompositeField cpsit = (GCompositeField)fld;
                if (cpsit is GStruct) {
                    structList.Add((GStruct)cpsit);
                } else if (cpsit is GList) {
                    listList.Add((GList)cpsit);
                }
                foreach (GField child in cpsit.Childs) {
                    analyseField(child);
                }
            } else if (fld is GField) {
                if (!labelList.Contains(fld.Label)) {
                    labelList.Add(fld.Label);
                }
                fieldList.Add((GField)fld);
            }
        }

        public void saveToXML() {
            string path = FileStrm.Name + XML_EXT;
            StreamWriter strW = new StreamWriter(path, false);
        }*/

        internal void Close() {
            FileStrm.Close();
            GBinReader.Close();
        }
    }
}
