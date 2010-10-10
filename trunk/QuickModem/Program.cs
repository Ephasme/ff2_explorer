using System.IO;
using System;
using Bioware.GFF;
using Bioware.GFF.XML;
using System.Configuration;
namespace QuickModem {

    class QuickModem {

        public static string[] VALID_EXTENTION_LIST = { ".ifo", ".are", ".git", ".gic", ".utc", ".utd",
                                                        ".ute", ".uti", ".utp", ".uts", ".utm", ".utt",
                                                        ".utw", ".dlg", ".jrl", ".fac", ".itp", ".ptm",
                                                        ".ptt", ".bic" };
        public const int SEPARATOR_SIZE = 70;
        public const string CREATOR = "Peluso Loup-Stéphane";
        public const string VERSION = "1.0";
        public const string NAME = "QuickModem";
        public const string LIBRARY_NAME = "GFFLibrary.dll";

        public const string XML_EXT = ".xml";

        public const char MODELISATION = 'M';
        public const char DEMODELISATION = 'D';
        public const char QUITTER = 'Q';

        public bool Again { get; set; }

        public QuickModem() {
            Initialize();
        }

        string tempDirName, xmlDirName, repoDirName;

        private void Initialize() {
            xmlDirName = ConfigurationManager.AppSettings["XmlDir"];
            tempDirName = ConfigurationManager.AppSettings["TempDir"];
            repoDirName = ConfigurationManager.AppSettings["RepoDir"];
            if (Directory.Exists(repoDirName)) {
                DirectoryInfo di_repo = new DirectoryInfo(repoDirName);
                FileInfo[] l_repo_fi = di_repo.GetFiles();
                if (!Directory.Exists(tempDirName)) {
                    CreateDirectory(tempDirName);
                    foreach (FileInfo fi in l_repo_fi) {
                        fi.CopyTo(tempDirName + "/" + fi.Name);
                    }
                }
            }
            Again = true;
        }

        public void WriteHeader() {
            Console.Clear();
            Console.WriteLine("Bienvenue sur " + NAME + " par " + CREATOR + " (c) 2010 (Version " + VERSION + ")");
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
                        CreateDirectory(xmlDirName);
                        DoOnFiles(tempDirName, xmlDirName, IsGFF, ModelFile);
                        PushToContinue();
                        break;
                    case DEMODELISATION:
                        CreateDirectory(tempDirName);
                        DoOnFiles(xmlDirName, tempDirName, IsXML, DemodFile);
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
                        File.Copy(fi.FullName, copy_path + "/" + fi.Name, true);
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
            string path = xmlDirName + "/" + fi.Name + XML_EXT;
            GDocument gdoc = new GDocument(fi.FullName);
            GXmlDocument xdoc = new GXmlDocument();
            xdoc.Save(gdoc.RootStruct, path);
        }
        private void DemodFile(FileInfo fi) {
            string sname = fi.Name.Remove(fi.Name.Length - 4);
            GXmlDocument xdoc = new GXmlDocument(fi.FullName);
            GDocument gdoc = new GDocument();
            gdoc.Save(xdoc.RootStruct, tempDirName + "/" + sname);
        }

        private void SetToClose() {
            Console.WriteLine("Fermeture de l'application.");
            Again = false;
        }

        private static void PushToContinue() {
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey(false);
        }
        private void CreateDirectory(string path) {
            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
            DirectoryInfo di = Directory.CreateDirectory(path);
        }
    }

    class Program {
        static void Main(string[] args) {
            QuickModem qm = new QuickModem();
            qm.Start();
        }
    }
}
