using Bioware;
using Bioware.GFF;
using Bioware.NWN;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class AreaTest
    {
        [SetUp]
        public void SetUp()
        {
            _path = "D:/NWN/modules/ffr2_repository/";
            _list = new[]
            {
                new GffDocument(_path + "ext_ar_00.git"), new GffDocument(_path + "ext_ar_00.gic"),
                new GffDocument(_path + "ext_ar_00.are")
            };
            _area = new NArea(_list);
        }

        private string _path;
        private GffDocument[] _list;
        private NArea _area;

        public TestContext TestContext { get; set; }

        [Test]
        public void AreaNameTest()
        {
            const Lang langue = Lang.English;
            const string expected = "Lyon - Porte Nord";
            var actual = _area.GetName(langue);
            Assert.AreEqual(expected, actual);
        }

        [Test]
        public void AreaPropertiesTest()
        {
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
            _area.ResRef = (ResRef) "tagada";
            Assert.AreEqual("tagada", _area.ResRef);
            _area.ResRef = (ResRef) "ext_ar_00";
        }
    }
}