using System;

namespace Bioware.GFF.Field
{
    public class GffShortReader : IValueReader
    {
        private short _sh;

        public void Parse(string value)
        {
            _sh = short.Parse(value);
        }

        public void Parse(GffField field)
        {
            _sh = BitConverter.ToInt16(field.Bytes, 0);
        }

        public string TextValue => _sh.ToString();

        public byte[] ByteArray => BitConverter.GetBytes(_sh);
    }
}