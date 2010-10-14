using Bioware.NWN;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bioware.GFF;
using System;
using Bioware;

namespace TestProject {


    /// <summary>
    ///Classe de test pour AreaTest, destinée à contenir tous
    ///les tests unitaires AreaTest
    ///</summary>
    [TestClass()]
    public class AreaTest {

        string path;
        GDocument[] list;
        Area area;

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
        [TestInitialize()]
        public void MyTestInitialize()
        {
            path = "D:/NWN/modules/ffr2_repository/";
            list = new GDocument[] { new GDocument(path + "ext_ar_00.git"), new GDocument(path + "ext_ar_00.gic"), new GDocument(path + "ext_ar_00.are") };
            area = new Area(list);
        }
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
        public void Area_PropertiesTest() {
            Assert.AreEqual(0, area.ChanceLightning);
            area.ChanceLightning = 5;
            Assert.AreEqual(5, area.ChanceLightning);
            area.ChanceLightning = 0;

            Assert.AreEqual(20, area.ChanceRain);
            area.ChanceRain = 5;
            Assert.AreEqual(5, area.ChanceRain);
            area.ChanceRain = 20;

            Assert.AreEqual(false, area.MoonShadows);
            area.MoonShadows = true;
            Assert.AreEqual(true, area.MoonShadows);
            area.MoonShadows = false;

            Assert.AreEqual("ext_ar_00", area.ResRef);
            area.ResRef = (ResRef)"tagada";
            Assert.AreEqual("tagada", area.ResRef);
            area.ResRef = (ResRef)"ext_ar_00";

        }

        /// <summary>
        ///Test pour GetName
        ///</summary>
        [TestMethod()]
        public void Area_NameTest() {
            Lang langue = Lang.English;
            string expected = "Lyon - Porte Nord";
            string actual = area.GetName(langue);
            Assert.AreEqual(expected, actual);
        }
    }
}
