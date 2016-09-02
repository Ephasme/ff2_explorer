namespace Bioware.GFF.Field
{
    public class GffResRefReader : IValueReader
    {
        private ResRef _resref;

        public void Parse(string value)
        {
            _resref = new ResRef(value);
        }

        public void Parse(GffField field)
        {
            _resref = new ResRef(LatinEncoding.Latin1.GetString(field.Bytes));
        }

        public string TextValue => _resref;

        public byte[] ByteArray => LatinEncoding.Latin1.GetBytes(_resref);
    }
}