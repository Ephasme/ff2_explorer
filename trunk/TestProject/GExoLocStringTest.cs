using Microsoft.VisualStudio.TestTools.UnitTesting;
using Bioware.GFF.Field;
using Bioware.GFF;
using Bioware.Tools;
namespace TestProject
{
    
    
    /// <summary>
    ///Classe de test pour GExoLocStringTest, destinée à contenir tous
    ///les tests unitaires GExoLocStringTest
    ///</summary>
    [TestClass]
    public class GExoLocStringTest {
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


        /// <summary>
        ///Test pour ExoLocString.
        ///</summary>
        [TestMethod]
        [DeploymentItem("Bioware.dll")]
        public void GExoLocStringParseTest() {
            const string byteString = "FFFFFFFF0100000000000000110000004C796F6E202D20506F727465204E6F7264";
            const string valueString = "-1||0=Lyon - Porte Nord";
            var fld = new GField("test", GType.CExoLocString, HexaManip.StringToByteArray(byteString));
            var aExlocstr = new GExoLocStringReader_Accessor();
            aExlocstr.Parse(fld);
            var expValueString = aExlocstr.TextValue;
            aExlocstr.Parse(valueString);
            var expByteString = HexaManip.ByteArrayToString(aExlocstr.ByteArray);

            Assert.AreEqual(expValueString, valueString);
            Assert.AreEqual(expByteString, byteString);
        }
    }
}
