using Bioware;
using Bioware.GFF;
using Bioware.GFF.Field;
using NUnit.Framework;

namespace TestProject
{
    [TestFixture]
    public class GExoLocStringTest
    {
        public TestContext TestContext { get; set; }

        [Test]
        public void GExoLocStringParseTest()
        {
            const string byteString = "FFFFFFFF0100000000000000110000004C796F6E202D20506F727465204E6F7264";
            const string valueString = "-1||0=Lyon - Porte Nord";
            var fld = new GffField("test", GffType.CExoLocString, HexaManip.StringToByteArray(byteString));
            var aExlocstr = new GffExoLocStringReader();
            aExlocstr.Parse(fld);
            var expValueString = aExlocstr.TextValue;
            aExlocstr.Parse(valueString);
            var expByteString = HexaManip.ByteArrayToString(aExlocstr.ByteArray);

            Assert.AreEqual(expValueString, valueString);
            Assert.AreEqual(expByteString, byteString);
        }
    }
}