using Bioware.NWN;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class ModuleTest
    {
        public TestContext TestContext { get; set; }

        private Module _mod;
        private const string RootPath = "D:/NWN";
        private const string ModuleName = "FFR2_V1_0a.mod";

        [Test]
        public void ModuleAreaListTest()
        {
            ModuleLoadTest();
            var nAreas = _mod.AreaList;
            Assert.IsNotNull(nAreas);
        }

        [Test]
        public void ModuleLoadTest()
        {
            _mod = new Module(RootPath, ModuleName); // TODO : initialisez à une valeur appropriée
        }
    }
}