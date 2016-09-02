using System;
using Bioware;
using Bioware.Erf;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class ContainerTest
    {
        public TestContext TestContext { get; set; }

        /// <summary>
        ///     Test pour ExtractAll
        /// </summary>
        [Test]
        public void ContainerExtractAllTest()
        {
            Container cont = new ErfFile("D:/NWN/modules/FFR2_V1_0a.mod");
            const string path = "D:/NWN/modules/FFR2_V1_0a";
            Func<ContentObject, bool> condMeth = item => true;
            cont.ExtractAll(path, condMeth);
        }
    }
}