using Bioware.XML;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bioware.Virtual;
using Bioware.GFF;
namespace Test
{
    
    
    /// <summary>
    ///Classe de test pour XMLFileReaderTest, destinée à contenir tous
    ///les tests unitaires XMLFileReaderTest
    ///</summary>
    [TestClass()]
    public class XMLFileReaderTest {


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
        ///Test pour Constructeur XMLFileReader
        ///</summary>
        [TestMethod()]
        [DeploymentItem("GFFSystem.dll")]
        public void XMLFileReaderConstructorTest() {
            string path = "D:\\NWN\\modules\\ffr2_repository\\sys_ar_00.are";
            GFileReader reader = new GFileReader(path);
            VStruct root = reader.getRootStruct();
            XMLFileSaver saver = new XMLFileSaver(path+".xml", root);
            saver.save();
            XMLFileReader_Accessor target = new XMLFileReader_Accessor(path + ".xml");
        }
    }
}
