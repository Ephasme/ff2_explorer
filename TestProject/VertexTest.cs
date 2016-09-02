using System;
using System.Collections.Generic;
using Bioware.Geometry;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class VertexTest
    {
        public TestContext TestContext { get; set; }


        private static double[] GenDoubleList(int not)
        {
            var lD = new List<double>();
            var rdm = new Random((DateTime.Now.Millisecond ^ 2)/2);
            var imax = rdm.Next(50, 60);
            if (imax == not)
            {
                imax++;
            }
            for (var i = 0; i < imax; i++)
            {
                lD.Add(rdm.NextDouble()*rdm.Next(100));
            }
            return lD.ToArray();
        }

        [Test]
        public void VertexAdditionTest()
        {
            var vA = new Vertex(5.2, 8, 4);
            var vB = new Vertex(4, 1.254, 875.12);
            var expected = new Vertex(9.2, 9.254, 879.12);
            var actual = vA + vB;
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void VertexEqualsTest()
        {
            double[] cA = {5.245122345, 4.11548667542135, 2.456778};
            double[] cB = {5.2451223456, 4.11548667542135, 2.456778};
            var vA = new Vertex(cA);
            var vB = new Vertex(cB);
            Assert.IsFalse(vA.Equals(vB));

            cA = new[] {5.245122345, 4.11548667542135, 2.456778};
            cB = new[] {5.245122345, 4.11548667542135, 2.456778};
            vA = new Vertex(cA);
            vB = new Vertex(cB);
            Assert.IsTrue(vA.Equals(vB));
        }

        [Test]
        public void VertexGetHashCodeTest()
        {
            for (var i = 0; i < 500; i++)
            {
                var vA = new Vertex(GenDoubleList(0));
                var vB = new Vertex(GenDoubleList(vA.Dimension));
                Assert.IsFalse(vA.GetHashCode() == vB.GetHashCode());
            }
        }
    }
}