using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using Bioware.GFF;
using Bioware.Virtual;
using Bioware.XML;

namespace FFR2QuickModem {
    class Program {
        public static string[] VALID_EXTENTION_LIST = { ".ifo", ".are", ".git", ".gic", ".utc", ".utd",
                                                        ".ute", ".uti", ".utp", ".uts", ".utm", ".utt",
                                                        ".utw", ".dlg", ".jrl", ".fac", ".itp", ".ptm",
                                                        ".ptt", ".bic" };
        public const string DEFAULT_PATH = "D:/NWN/modules/ffr2_repository";
        static void Main(string[] args) {

            string path = DEFAULT_PATH;
            bool again = true;

            while (again) {
                Console.Clear();
                Console.WriteLine("Bienvenue sur FFR2QuickModel par Loup-Stéphane PELUSO (c) 2010...");
                Console.WriteLine("-----------------------------------------------------------------");

                Console.WriteLine("     Fonction de modélisation/démodélisation XML :");
                Console.WriteLine("M : Modélisation GFF->XML");
                Console.WriteLine("D : Démodélisation XML->GFF");
                Console.WriteLine("G : Modem GFF->GFF");
                Console.WriteLine("Q : Quitter");
                Console.WriteLine("-----------------------------------------------------------------");
                Console.WriteLine("c : Changer le chemin par défaut des fichiers du module.");
                Console.WriteLine("Votre choix :");
                int result = Convert.ToChar(Console.Read());

                switch (result) {
                    case 'G':
                        if (Directory.Exists(path)) {
                            Directory.CreateDirectory(path + "/new");
                            DirectoryInfo di = new DirectoryInfo(path);
                            FileInfo[] l_fi = di.GetFiles();
                            foreach (FileInfo fi in l_fi) {
                                if (Array.IndexOf(VALID_EXTENTION_LIST, fi.Extension) != -1) {
                                    GffFileReader gff_rd = new GffFileReader(fi.FullName);
                                    VStruct root = gff_rd.getRootStruct();
                                    GffFileSaver gff_sv = new GffFileSaver(root, path + "/new/" + fi.Name, Path.GetExtension(fi.FullName));
                                    gff_sv.save();
                                }
                            }
                        }
                        break;
                    case 'M':
                        if (Directory.Exists(path)) {
                            DirectoryInfo di = new DirectoryInfo(path);
                            FileInfo[] l_fi = di.GetFiles();
                            foreach (FileInfo fi in l_fi) {
                                if (Array.IndexOf(VALID_EXTENTION_LIST, fi.Extension) != -1) {
                                    GffFileReader gff_rd = new GffFileReader(fi.FullName);
                                    VStruct root = gff_rd.getRootStruct();
                                    XmlFileSaver xml_sv = new XmlFileSaver(fi.FullName + ".xml", root);
                                    xml_sv.save();
                                }
                            }
                        }
                        break;
                    case 'D':
                        if (Directory.Exists(path)) {
                            DirectoryInfo di = new DirectoryInfo(path);
                            FileInfo[] l_fi = di.GetFiles();
                            FileInfo[] xml_l_fi = di.GetFiles("*.xml");
                            foreach (FileInfo fi in xml_l_fi) {
                                string spath = fi.FullName.Remove(fi.FullName.Length-4);
                                string sext = Path.GetExtension(spath);
                                XmlFileReader xml_rd = new XmlFileReader(fi.FullName);
                                VStruct root = xml_rd.getRootStruct();
                                GffFileSaver gff_sv = new GffFileSaver(root, spath+".new.gff", sext);
                                gff_sv.save();
                            }
                        }
                        break;
                    case 'Q':
                        Console.WriteLine("Fermeture de l'application.");
                        again = false;
                        break;
                    case 'c':
                        Console.Clear();
                        Console.WriteLine("Changement du chemin par défaut.");
                        if (Directory.Exists(path)) {
                            path = Console.ReadLine();
                            Console.WriteLine("Changement effectué avec succès");
                        } else {
                            Console.WriteLine("Impossible de trouver le chemin spécifié.");
                        }
                        break;
                }
            }
        }
    }
}
