using System;

using Bioware.NWN;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace TestProject
{
    
    
    /// <summary>
    ///Classe de test pour ModuleTest, destinée à contenir tous
    ///les tests unitaires ModuleTest
    ///</summary>
    [TestClass]
    public class ModuleTest {
        /// <summary>
        ///Obtient ou définit le contexte de test qui fournit
        ///des informations sur la série de tests active ainsi que ses fonctionnalités.
        ///</summary>
        public TestContext TestContext { get; set; }

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

        Module _mod;
        const string RootPath = "D:/NWN";
        const string ModuleName = "FFR2_V1_0a.mod";

        /// <summary>
        ///Test pour Load
        ///</summary>
        [TestMethod]
        public void ModuleLoadTest() {
            _mod = new Module(RootPath, ModuleName); // TODO : initialisez à une valeur appropriée
        }

        /// <summary>
        ///Test pour AreaList
        ///</summary>
        [TestMethod]
        public void ModuleAreaListTest() {
            ModuleLoadTest();
            var nAreas = _mod.AreaList;
            Assert.IsNotNull(nAreas);
        }
    }
}
