using FFR2Explorer.gff_specific;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using FFR2Explorer;
using System.Collections.Generic;

namespace TestProject
{
    
    
    /// <summary>
    ///Classe de test pour GFileReaderTest, destinée à contenir tous
    ///les tests unitaires GFileReaderTest
    ///</summary>
    [TestClass()]
    public class GFileReaderTest {


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
        ///Test pour Constructeur GFileLinearReader
        ///</summary>
        [TestMethod()]
        [DeploymentItem("FFR2Explorer.exe")]
        public void GFileReader_constructorTest() {
            string path = "D:/NWN/modules/ffr2_repository/ext_ar_00.are.newGFF";
            GFileReader_Accessor target = new GFileReader_Accessor(path);
        }

        /// <summary>
        ///Test pour getRootStruct
        ///</summary>
        [TestMethod()]
        public void GFileReader_getRootStructTest() {
            string path = "D:/NWN/localvault/aluviandarkstar.bic";
            GFileReader target = new GFileReader(path);
            GStruct actual = target.getRootStruct();
        }

        /// <summary>
        ///Test pour getOrphelins
        ///</summary>
        [TestMethod()]
        public void GFileReader_getOrphelinsTest() {
            string path = "D:/NWN/localvault/aluviandarkstar.bic";
            GFileReader target = new GFileReader(path);
            PrivateObject pv = new PrivateObject(target, new PrivateType(target.GetType()));
            GFileReader_Accessor a_target = new GFileReader_Accessor(pv);
            List<GField> orph = GFileReader.getOrphelins(a_target.l_fld, a_target.l_str);
        }
    }
}
