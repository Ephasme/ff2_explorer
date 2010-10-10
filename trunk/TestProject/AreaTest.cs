using Bioware.NWN;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bioware.GFF;

namespace TestProject
{
    
    
    /// <summary>
    ///Classe de test pour AreaTest, destinée à contenir tous
    ///les tests unitaires AreaTest
    ///</summary>
    [TestClass()]
    public class AreaTest {


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
        ///Test pour ChanceLightning
        ///</summary>
        [TestMethod()]
        public void Area_ChanceLightningTest() {
            string path = "D:/NWN/modules/ffr2_repository/";
            GDocument[] list = new GDocument[] { new GDocument(path + "ext_ar_00.git"), new GDocument(path + "ext_ar_00.gic"), new GDocument(path + "ext_ar_00.are") };
            Area area = new Area(list);
            int expected = 0;
            Assert.AreEqual(expected, area.ChanceLightning);
        }
    }
}
