namespace Bioware.GFF.Field
{
    public class GffByteReader : IValueReader
    {
        private byte _value;

        public void Parse(string value)
        {
            _value = byte.Parse(value);
        }

        public void Parse(GffField field)
        {
            _value = field.Bytes[0];
        }

        public string TextValue => _value.ToString();

        public byte[] ByteArray
        {
            get
            {
                var res = new[] {_value};
                return res;
            }
        }
    }
}