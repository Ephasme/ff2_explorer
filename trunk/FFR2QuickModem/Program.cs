using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bioware.GFF;
using Bioware.GFF.XML;

namespace FFR2QuickModem {

    class QuickModem {

        public const string ERR_NO_DIRECTORY = "Erreur : l'application doit être placée dans un dossier contenant " + BASE_DIR + ".";

        public static string[] VALID_EXTENTION_LIST = { ".ifo", ".are", ".git", ".gic", ".utc", ".utd",
                                                        ".ute", ".uti", ".utp", ".uts", ".utm", ".utt",
                                                        ".utw", ".dlg", ".jrl", ".fac", ".itp", ".ptm",
                                                        ".ptt", ".bic" };
        public const int SEPARATOR_SIZE = 70;
        public const string CREATOR = "Peluso Loup-Stéphane";
        public const string VERSION = "1.0";
        public const string NAME = "FFR2QuickModem";
#if DEBUG
        public const string BASE_DIR = "ffr2_repository";
#else
        public const string BASE_DIR = "temp0";
#endif
        public const string LIBRARY_NAME = "GFFLibrary.dll";

        public const string XML_DIR = "/xml/";
        public const string GFF_DIR = "/gff/";
        public const string XML_EXT = ".xml";

        public const char MODELISATION = 'M';
        public const char DEMODELISATION = 'D';
        public const char QUITTER = 'Q';

        public string ModemPath { get; set; }
        public bool Again { get; set; }

        public QuickModem() {
            Initialize();
        }

        private void Initialize() {
#if DEBUG
            ModemPath = "D:/NWN/modules/";
            Console.WriteLine("ATTENTION ! Ceci est une version de Débogage.");
            Again = true;
#else
            if (Directory.Exists("./" + BASE_DIR)) {
                ModemPath = Path.GetFullPath("./");
                Again = true;
            } else {
                Console.WriteLine(ERR_NO_DIRECTORY);
                PushToContinue();
                Again = false;
            }
#endif
        }

        public void WriteHeader() {
            Console.Clear();
            Console.WriteLine("Bienvenue sur " + NAME + " par " + CREATOR + " (c) 2010 (Version " + VERSION + ")\nLe chemin actuel est :\n" + ModemPath);
            WriteSeparator();
            Console.WriteLine("Système de modélisation/démodélisation XML :\n" + MODELISATION + " : Modélisation GFF->XML\n" + DEMODELISATION + " : Démodélisation XML->GFF\n" + QUITTER + " : Quitter");
            WriteSeparator();
            Console.WriteLine();
        }

        public void WriteSeparator() {
            int i = 0;
            while (i++ < SEPARATOR_SIZE) {
                Console.Write("-");
            }
            Console.WriteLine();
        }

        public delegate bool DoActionMethod(FileInfo file);
        public delegate void ActionMethod(FileInfo file);

        internal void Start() {
            while (Again) {
                WriteHeader();
                Console.WriteLine("Votre choix : ");
                int result = Convert.ToChar(Console.Read());
                switch (result) {
                    case MODELISATION:
                        CreateDirectory(XML_DIR);
                        DoOnFiles(ModemPath + BASE_DIR, XML_DIR, IsGFF, ModelFile);
                        PushToContinue();
                        break;
                    case DEMODELISATION:
                        CreateDirectory(GFF_DIR);
                        DoOnFiles(ModemPath + XML_DIR, GFF_DIR, IsXML, DemodFile);
                        PushToContinue();
                        break;
                    case QUITTER:
                        SetToClose();
                        break;
                }
            }
        }
        public void DoOnFiles(string path, string copy_path, DoActionMethod doAction, ActionMethod actionMethod) {
            if (Directory.Exists(path)) {
                string[] l_sfi = Directory.GetFiles(path);
                int cent = 0;
                for (int i = 0; i < l_sfi.Length; i++) {
                    if (cent <= ((int)(SEPARATOR_SIZE * i) / l_sfi.Length)) {
                        cent++;
                        Console.Write(".");
                    }
                    FileInfo fi = new FileInfo(l_sfi[i]);
                    if (doAction(fi)) {
                        actionMethod(fi);
                    } else {
                        File.Copy(fi.FullName, ModemPath + copy_path + fi.Name, true);
                    }
                }
                Console.Write("\n");
            }
        }

        private bool IsXML(FileInfo fi) {
            return fi.Extension.ToLower() == XML_EXT.ToLower();
        }
        private bool IsGFF(FileInfo fi) {
            return Array.IndexOf(VALID_EXTENTION_LIST, fi.Extension.ToLower()) != -1;
        }

        private void ModelFile(FileInfo fi) {
            string path = ModemPath + XML_DIR + fi.Name + XML_EXT;
            GDocument gdoc = new GDocument(fi.FullName);
            GXmlDocument xdoc = new GXmlDocument();
            xdoc.Save(gdoc.RootStruct, path);
        }
        private void DemodFile(FileInfo fi) {
            string sname = fi.Name.Remove(fi.Name.Length - 4);
            GXmlDocument xdoc = new GXmlDocument(fi.FullName);
            GDocument gdoc = new GDocument();
            gdoc.Save(xdoc.RootStruct, ModemPath + GFF_DIR + sname);
        }

        private void SetToClose() {
            Console.WriteLine("Fermeture de l'application.");
            Again = false;
        }

        private static void PushToContinue() {
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey(false);
        }
        private void CreateDirectory(string add) {
            DirectoryInfo di = Directory.CreateDirectory(ModemPath + add);
            Console.WriteLine("Création du répertoire : {0}", di.FullName);
        }
    }

    class Program {
        static void Main(string[] args) {
            QuickModem qm = new QuickModem();
            qm.Start();
        }
    }
}
