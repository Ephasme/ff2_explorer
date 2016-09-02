namespace Bioware.GFF.Field
{
    public class GffCharReader : IValueReader
    {
        private char _c;

        public void Parse(string value)
        {
            _c = value.ToCharArray()[0];
        }

        public void Parse(GffField field)
        {
            _c = LatinEncoding.Latin1.GetChars(field.Bytes)[0];
        }

        public string TextValue
        {
            get
            {
                char[] l = {_c};
                return new string(l);
            }
        }

        public byte[] ByteArray => LatinEncoding.Latin1.GetBytes(TextValue);
    }
}