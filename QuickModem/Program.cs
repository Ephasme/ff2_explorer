using System;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using Bioware.Erf;
using Bioware.GFF;

namespace QuickModem
{
    public class QuickModem
    {
        public delegate void ActionMethod(FileInfo file);

        public delegate bool DoActionMethod(FileInfo file);

        public const int SeparatorSize = 70;
        public const string Creator = "Peluso Loup-Stéphane";
        public const string Version = "1.0";
        public const string Name = "QuickModem";
        public const string LibraryName = "GFFLibrary.dll";

        public const string XmlExt = ".xml";

        public const char Modelisation = 'M';
        public const char Demodelisation = 'D';
        public const char Quitter = 'Q';

        public static readonly string[] ValidExtentionList =
        {
            ".ifo", ".are", ".git", ".gic", ".utc", ".utd",
            ".ute", ".uti", ".utp", ".uts", ".utm", ".utt",
            ".utw", ".dlg", ".jrl", ".fac", ".itp", ".ptm",
            ".ptt", ".bic"
        };

        private string _baseDirName;
        private string _tempDirName;
        private string _xmlDirName;
        private string _modDirName;

        public bool Again { get; set; }

        public void WriteHeader()
        {
            Console.Clear();
            Console.WriteLine("Bienvenue sur " + Name + " par " + Creator + " (c) 2010 (Version " + Version + ")");
            WriteSeparator();
            Console.WriteLine("Système de modélisation/démodélisation XML :\n" + Modelisation +
                              " : Modélisation GFF->XML\n" + Demodelisation + " : Démodélisation XML->GFF\n" + Quitter +
                              " : Quitter");
            WriteSeparator();
            Console.WriteLine();
        }

        public void WriteSeparator()
        {
            var i = 0;
            while (i++ < SeparatorSize)
            {
                Console.Write("-");
            }
            Console.WriteLine();
        }

        private void GetConfig()
        {
            if (File.Exists("./qm.ini"))
            {
                var str = new StreamReader(File.OpenRead("./qm.ini"));
                var rg = new Regex("^(?<item>.*)=(?<value>.*)");
                while (!str.EndOfStream)
                {
                    var line = str.ReadLine();
                    if (line == null) continue;
                    var m = rg.Match(line);
                    switch (m.Groups["item"].Value)
                    {
                        case "BASE":
                            _baseDirName = m.Groups["value"].Value;
                            break;
                        case "MODULE":
                            _modDirName = m.Groups["value"].Value;
                            break;
                    }
                }
                str.Close();
            }
            else
            {
                _baseDirName = Path.GetFullPath("./");
                _modDirName = _baseDirName + "/modules/FFR.mod";
            }
        }

        private void SetConfig()
        {
            var strw = new StreamWriter(File.Create("./qm.ini"), Encoding.ASCII);
            strw.WriteLine("BASE=" + _baseDirName);
            strw.WriteLine("MODULE=" + _modDirName);
            strw.Close();
        }

        public void Start()
        {
            GetConfig();
            GetBaseDirectory();
            _tempDirName = _baseDirName + "/modules/temp0";
            _xmlDirName = _baseDirName + "/modules/xml";
            if (!Directory.Exists(_tempDirName))
            {
                CreateDirectory(_tempDirName);
                GetModuleDirectory();
                var mod = new ErfFile(_modDirName);
                Console.WriteLine("Extraction du module... Veuillez patienter.");
                var dt = DateTime.Now;
                mod.ExtractAll(_tempDirName);
                Console.WriteLine("Extraction exécutée en " + (DateTime.Now.Subtract(dt).TotalSeconds + " secondes."));
                PushToContinue();
            }
            else
            {
                Console.WriteLine("Utilisation du dossier temp0 déjà présent.");
                PushToContinue();
            }
            SetConfig();
            CreateDirectory(_xmlDirName);
            Again = true;

            while (Again)
            {
                WriteHeader();
                Console.WriteLine("Votre choix : ");
                int result = Convert.ToChar(Console.Read());
                switch (result)
                {
                    case Modelisation:
                        DoOnFiles(_tempDirName, _xmlDirName, IsGff, ModelFile);
                        break;
                    case Demodelisation:
                        DoOnFiles(_xmlDirName, _tempDirName, IsXml, DemodFile);
                        break;
                    case Quitter:
                        SetToClose();
                        break;
                }
            }
        }

        private void GetModuleDirectory()
        {
            while (!File.Exists(_modDirName))
            {
                var diBasedir = new DirectoryInfo(_baseDirName + "/modules");
                var lFiModules = diBasedir.GetFiles("*" + ErfFile.ModExt);
                var choiceOk = false;
                while (choiceOk == false)
                {
                    Console.Clear();
                    Console.WriteLine("Vous utilisez le répertoire de base : " + _baseDirName);
                    Console.WriteLine("Quel module voulez-vous charger ?");
                    foreach (var fiModule in lFiModules)
                    {
                        Console.WriteLine("\t" + Array.IndexOf(lFiModules, fiModule) + ") " + fiModule.Name);
                    }
                    int choice;
                    choiceOk = int.TryParse(Console.ReadLine(), out choice);
                    choiceOk &= choice >= 0 && choice < lFiModules.Length;
                    if (choiceOk == false)
                    {
                        Console.WriteLine("Impossible d'interpréter la demande " + choice + ".\nVeuillez recommencer.\n");
                    }
                    else
                    {
                        _modDirName = lFiModules[choice].FullName;
                    }
                }
            }
        }

        private void GetBaseDirectory()
        {
            while (!Directory.Exists(_baseDirName + "/modules"))
            {
                Console.Clear();
                Console.WriteLine("Veuillez entrer le répertoire de votre installation de NWN :");
                _baseDirName = Console.ReadLine();
            }
        }

        public static void DoOnFiles(string path, string copyPath, DoActionMethod doAction, ActionMethod actionMethod)
        {
            if (Directory.Exists(path))
            {
                var lSfi = Directory.GetFiles(path);
                var cent = 0;
                var dt = DateTime.Now;
                for (var i = 0; i < lSfi.Length; i++)
                {
                    if (cent <= SeparatorSize*i/lSfi.Length)
                    {
                        cent++;
                        Console.Write(".");
                    }
                    var fi = new FileInfo(lSfi[i]);
                    if (doAction(fi))
                    {
                        actionMethod(fi);
                    }
                    else
                    {
                        File.Copy(fi.FullName, copyPath + "/" + fi.Name, true);
                    }
                }
                var ts = DateTime.Now.Subtract(dt);
                Console.WriteLine("\nProcessus exécuté en " + ts.Minutes + " min " + ts.Seconds + " sec " +
                                  ts.Milliseconds + " msec.");
                PushToContinue();
            }
            else
            {
                Console.WriteLine("Le dossier " + Path.GetFullPath(path) + " n'existe pas.");
            }
        }

        private static bool IsXml(FileInfo fi)
        {
            return string.Equals(fi.Extension, XmlExt, StringComparison.CurrentCultureIgnoreCase);
        }

        private static bool IsGff(FileInfo fi)
        {
            return Array.IndexOf(ValidExtentionList, fi.Extension.ToLower()) != -1;
        }

        private void ModelFile(FileInfo fi)
        {
            var path = _xmlDirName + "/" + fi.Name + XmlExt;
            var gdoc = new GffDocument(fi.FullName);
            var xdoc = new GffXmlDocument();
            xdoc.Save(gdoc.RootStruct, path);
        }

        private void DemodFile(FileInfo fi)
        {
            var sname = fi.Name.Remove(fi.Name.Length - 4);
            var xdoc = new GffXmlDocument(fi.FullName);
            var gdoc = new GffDocument();
            gdoc.Save(xdoc.RootStruct, Path.Combine(_tempDirName, sname));
        }

        private void SetToClose()
        {
            Console.WriteLine("Fermeture de l'application.");
            Again = false;
        }

        private static void PushToContinue()
        {
            Console.WriteLine("Appuyez sur une touche pour continuer...");
            Console.ReadKey(false);
        }

        private void CreateDirectory(string path)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
        }
    }

    internal class Program
    {
        private static void Main(string[] args)
        {
            if (args == null) throw new ArgumentNullException(nameof(args));
            var qm = new QuickModem();
            qm.Start();
        }
    }
}