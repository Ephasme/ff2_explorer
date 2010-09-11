using Microsoft.VisualStudio.TestTools.UnitTesting;
using FFR2Explorer;
namespace TestFFR2Explorer
{
    
    
    /// <summary>
    ///Classe de test pour XMLFileSaverTest, destinée à contenir tous
    ///les tests unitaires XMLFileSaverTest
    ///</summary>
    [TestClass()]
    public class XMLFileSaverTest {


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


        /// <summary>
        ///Test pour save
        ///</summary>
        [TestMethod()]
        public void saveTest() {
            GFileLoader gFileLd = new GFileLoader("D:/NWN/modules/ffr2_repository/ext_ar_00.are");
            string saving_path = "D:/NWN/modules/ffr2_repository/ext_ar_00.are.xml";
            Struct root = gFileLd.RootStruct;
            XMLFileSaver target = new XMLFileSaver(saving_path, root);
            target.save();
        }
    }
}
