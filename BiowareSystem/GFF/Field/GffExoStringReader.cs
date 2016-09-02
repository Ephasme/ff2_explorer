namespace Bioware.GFF.Field
{
    public class GffExoStringReader : IValueReader
    {
        public void Parse(string value)
        {
            TextValue = value;
        }

        public void Parse(GffField field)
        {
            TextValue = LatinEncoding.Latin1.GetString(field.Bytes);
        }

        public string TextValue { get; private set; }

        public byte[] ByteArray => LatinEncoding.Latin1.GetBytes(TextValue);
    }
}