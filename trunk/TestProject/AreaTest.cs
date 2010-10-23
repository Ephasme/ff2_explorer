﻿using Bioware.NWN;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bioware.GFF;
using Bioware;

namespace TestProject {


    /// <summary>
    ///Classe de test pour AreaTest, destinée à contenir tous
    ///les tests unitaires AreaTest
    ///</summary>
    [TestClass]
    public class AreaTest {

        string _path;
        GDocument[] _list;
        NArea _area;

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
        [TestInitialize]
        public void MyTestInitialize()
        {
            _path = "D:/NWN/modules/ffr2_repository/";
            _list = new[] { new GDocument(_path + "ext_ar_00.git"), new GDocument(_path + "ext_ar_00.gic"), new GDocument(_path + "ext_ar_00.are") };
            _area = new NArea(_list);
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
        [TestMethod]
        public void AreaPropertiesTest() {
            Assert.AreEqual(0, _area.ChanceLightning);
            _area.ChanceLightning = 5;
            Assert.AreEqual(5, _area.ChanceLightning);
            _area.ChanceLightning = 0;

            Assert.AreEqual(20, _area.ChanceRain);
            _area.ChanceRain = 5;
            Assert.AreEqual(5, _area.ChanceRain);
            _area.ChanceRain = 20;

            Assert.AreEqual(false, _area.MoonShadows);
            _area.MoonShadows = true;
            Assert.AreEqual(true, _area.MoonShadows);
            _area.MoonShadows = false;

            Assert.AreEqual("ext_ar_00", _area.ResRef);
            _area.ResRef = (ResRef)"tagada";
            Assert.AreEqual("tagada", _area.ResRef);
            _area.ResRef = (ResRef)"ext_ar_00";

        }

        /// <summary>
        ///Test pour GetName
        ///</summary>
        [TestMethod]
        public void AreaNameTest() {
            const Lang langue = Lang.English;
            const string expected = "Lyon - Porte Nord";
            var actual = _area.GetName(langue);
            Assert.AreEqual(expected, actual);
        }
    }
}
