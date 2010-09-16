using Bioware.GFF;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bioware.Virtual;
using System.IO;
using System;

namespace Test {


    /// <summary>
    ///Classe de test pour GffFileSaverTest, destinée à contenir tous
    ///les tests unitaires GffFileSaverTest
    ///</summary>
    [TestClass()]
    public class GffFileSaverTest {


        private TestContext testContextInstance;

        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext {
            get {
                return testContextInstance;
            }
            set {
                testContextInstance = value;
            }
        }

        #region Attributs de tests supplémentaires
        // 
        //Vous pouvez utiliser les attributs supplémentaires suivants lors de l'écriture de vos tests :
        //
        //Utilisez ClassInitialize pour exécuter du code avant d'exécuter le premier test dans la classe
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Utilisez ClassCleanup pour exécuter du code après que tous les tests ont été exécutés dans une classe
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Utilisez TestInitialize pour exécuter du code avant d'exécuter chaque test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Utilisez TestCleanup pour exécuter du code après que chaque test a été exécuté
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion

        public static string[] VALID_EXTENTION_LIST = { ".ifo", ".are", ".git", ".gic", ".utc", ".utd",
                                                        ".ute", ".uti", ".utp", ".uts", ".utm", ".utt",
                                                        ".utw", ".dlg", ".jrl", ".fac", ".itp", ".ptm",
                                                        ".ptt", ".bic" };

        public const string BASE_PATH = "D:/NWN/modules/ffr2_repository/";
        public const string FILE = "module.ifo";

        /// <summary>
        ///Test pour save
        ///</summary>
        [TestMethod()]
        public void saveTest() {
            string path = BASE_PATH + FILE;
            string ext = Path.GetExtension(path);
            string salt = ".g.gff";

            GffFileReader reader = new GffFileReader(path);
            VStruct root = reader.getRootStruct();
            GffFileSaver target = new GffFileSaver(root, path + salt, ext);

            target.save();
        }

        [TestMethod()]
        public void saveAllFilesTest() {
            if (Directory.Exists(BASE_PATH)) {
                Directory.CreateDirectory(BASE_PATH + "/new");
                DirectoryInfo di = new DirectoryInfo(BASE_PATH);
                FileInfo[] l_fi = di.GetFiles();
                foreach (FileInfo fi in l_fi) {
                    if (Array.IndexOf(VALID_EXTENTION_LIST, fi.Extension) != -1) {
                        GffFileReader gff_rd = new GffFileReader(fi.FullName);
                        VStruct root = gff_rd.getRootStruct();
                        GffFileSaver gff_sv = new GffFileSaver(root, BASE_PATH + "/new/" + fi.Name, Path.GetExtension(fi.FullName));
                        gff_sv.save();
                    }
                }
            }
        }
    }
}
