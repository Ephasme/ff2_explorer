using FFR2Explorer.gff_specific;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FFR2Explorer;
using System.IO;

namespace TestProject {


    /// <summary>
    ///Classe de test pour GFileSaverTest, destinée à contenir tous
    ///les tests unitaires GFileSaverTest
    ///</summary>
    [TestClass()]
    public class GFileSaverTest {

        public const string PATH = "D:/NWN/modules/ffr2_repository/ext_ar_00.are";
        public string ext = Path.GetExtension(PATH);
        
        public GFieldFactory loader;
        public GFileReader linread;
        public GFileSaver saver;

        public PrivateObject pv_loader;
        public PrivateObject pv_linread;
        public PrivateObject pv_saver;

        public GFieldFactory_Accessor a_loader;
        public GFileReader_Accessor a_linread;
        public GFileSaver_Accessor a_saver;

        public GStruct root;

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

        public GFileSaverTest() {
            linread = new GFileReader(PATH);
            pv_linread = new PrivateObject(linread);
            a_linread = new GFileReader_Accessor(pv_linread);

            saver = new GFileSaver(linread.getRootStruct(), PATH+".newGFF", ext);
            pv_saver = new PrivateObject(saver);
            a_saver = new GFileSaver_Accessor(pv_saver);
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

        /// <summary>
        ///Test pour populate
        ///</summary>
        [TestMethod()]
        [DeploymentItem("FFR2Explorer.exe")]
        public void GFileSaver_saveTest() {
            a_saver.save();
        }
    }
}
