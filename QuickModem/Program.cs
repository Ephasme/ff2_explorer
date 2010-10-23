using System.IO;
using System;
using Bioware.GFF;
using Bioware.GFF.XML;
using System.Text.RegularExpressions;
using System.Configuration;
using Bioware;
using System.Text;
using Bioware.Resources;
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
        }

        string baseDirName, tempDirName, xmlDirName, modDirName;

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

        private void GetConfig() {
            if (File.Exists("./qm.ini")) {
                StreamReader str = new StreamReader(File.OpenRead("./qm.ini"));
                Regex rg = new Regex("^(?<item>.*)=(?<value>.*)");
                while (!str.EndOfStream) {
                    string line = str.ReadLine();
                    Match m = rg.Match(line);
                    switch (m.Groups["item"].Value) {
                        case "BASE":
                            baseDirName = m.Groups["value"].Value;
                            break;
                        case "MODULE":
                            modDirName = m.Groups["value"].Value;
                            break;
                    }
                }
                str.Close();
            } else {
                baseDirName = Path.GetFullPath("./");
                modDirName = baseDirName + "/modules/FFR.mod";
            }
        }

        private void SetConfig() {
            StreamWriter strw = new StreamWriter(File.Create("./qm.ini"), Encoding.ASCII);
            strw.WriteLine("BASE=" + baseDirName);
            strw.WriteLine("MODULE=" + modDirName);
            strw.Close();
        }

        public void Start() {
            GetConfig();
            GetBaseDirectory();
            tempDirName = baseDirName + "/modules/temp0";
            xmlDirName = baseDirName + "/modules/xml";
            if (!Directory.Exists(tempDirName)) {
                CreateDirectory(tempDirName);
                GetModuleDirectory();
                var mod = new EFile(modDirName);
                Console.WriteLine("Extraction du module... Veuillez patienter.");
                var dt = DateTime.Now;
                mod.ExtractAll(tempDirName);
                Console.WriteLine("Extraction exécutée en "+(DateTime.Now.Subtract(dt).TotalSeconds+" secondes."));
                PushToContinue();
            } else {
                Console.WriteLine("Utilisation du dossier temp0 déjà présent.");
                PushToContinue();
            }
            SetConfig();
            CreateDirectory(xmlDirName);
            Again = true;

            while (Again) {
                WriteHeader();
                Console.WriteLine("Votre choix : ");
                int result = Convert.ToChar(Console.Read());
                switch (result) {
                    case MODELISATION:
                        DoOnFiles(tempDirName, xmlDirName, IsGFF, ModelFile);
                        break;
                    case DEMODELISATION:
                        DoOnFiles(xmlDirName, tempDirName, IsXML, DemodFile);
                        break;
                    case QUITTER:
                        SetToClose();
                        break;
                }
            }
        }

        private void GetModuleDirectory() {
            while (!File.Exists(modDirName)) {
                var di_basedir = new DirectoryInfo(baseDirName + "/modules");
                var l_fi_modules = di_basedir.GetFiles("*" + EFile.ModExt);
                int choice = -1;
                bool choice_ok = false;
                while (choice_ok == false) {
                    Console.Clear();
                    Console.WriteLine("Vous utilisez le répertoire de base : " + baseDirName);
                    Console.WriteLine("Quel module voulez-vous charger ?");
                    foreach (var fi_module in l_fi_modules) {
                        Console.WriteLine("\t" + Array.IndexOf(l_fi_modules, fi_module) + ") " + fi_module.Name);
                    }
                    choice_ok = int.TryParse(Console.ReadLine(), out choice);
                    choice_ok &= (choice >= 0 && choice < l_fi_modules.Length);
                    if (choice_ok == false) {
                        Console.WriteLine("Impossible d'interpréter la demande " + choice + ".\nVeuillez recommencer.\n");
                    } else {
                        modDirName = l_fi_modules[choice].FullName;
                    }
                }
            }
        }

        private void GetBaseDirectory() {
            while (!Directory.Exists(baseDirName + "/modules")) {
                Console.Clear();
                Console.WriteLine("Veuillez entrer le répertoire de votre installation de NWN :");
                baseDirName = Console.ReadLine();
            }
        }
        public void DoOnFiles(string path, string copy_path, DoActionMethod doAction, ActionMethod actionMethod) {
            if (Directory.Exists(path)) {
                string[] l_sfi = Directory.GetFiles(path);
                int cent = 0;
                var dt = DateTime.Now;
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
                var ts = DateTime.Now.Subtract(dt);
                Console.WriteLine("\nProcessus exécuté en " + ts.Minutes + " min " + ts.Seconds + " sec " + ts.Milliseconds + " msec.");
                PushToContinue();
            } else {
                Console.WriteLine("Le dossier " + Path.GetFullPath(path) + " n'existe pas.");
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
