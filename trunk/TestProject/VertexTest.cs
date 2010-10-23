using Bioware.Geometry;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using System;
namespace TestProject {


    /// <summary>
    ///Classe de test pour VertexTest, destinée à contenir tous
    ///les tests unitaires VertexTest
    ///</summary>
    [TestClass]
    public class VertexTest {
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
        ///Test pour op_Addition
        ///</summary>
        [TestMethod]
        public void VertexAdditionTest() {
            var vA = new Vertex(5.2, 8, 4);
            var vB = new Vertex(4, 1.254, 875.12);
            var expected = new Vertex(9.2, 9.254, 879.12);
            var actual = vA + vB;
            Assert.AreEqual(expected, actual);
        }

        /// <summary>
        ///Test pour Equals
        ///</summary>
        [TestMethod]
        public void VertexEqualsTest() {
            double[] cA = { 5.245122345, 4.11548667542135, 2.456778 };
            double[] cB = { 5.2451223456, 4.11548667542135, 2.456778 };
            var vA = new Vertex(cA);
            var vB = new Vertex(cB);
            Assert.IsFalse(vA.Equals(vB));

            cA = new[] { 5.245122345, 4.11548667542135, 2.456778 };
            cB = new[] { 5.245122345, 4.11548667542135, 2.456778 };
            vA = new Vertex(cA);
            vB = new Vertex(cB);
            Assert.IsTrue(vA.Equals(vB));
        }


        private static double[] GenDoubleList(int not) {
            var lD = new List<double>();
            var rdm = new Random((DateTime.Now.Millisecond ^ 2) / 2);
            var imax = rdm.Next(50, 60);
            if (imax == not) { imax++; }
            for (var i = 0; i < imax; i++) {
                lD.Add(rdm.NextDouble() * rdm.Next(100));
            }
            return lD.ToArray();
        }
        /// <summary>
        ///Test pour GetHashCode
        ///</summary>
        [TestMethod]
        public void VertexGetHashCodeTest() {
            Vertex vA, vB;
            for (var i = 0; i < 500; i++) {
                vA = new Vertex(GenDoubleList(0));
                vB = new Vertex(GenDoubleList(vA.Dimension));
                Assert.IsFalse(vA.GetHashCode() == vB.GetHashCode());
            }
        }
    }
}
