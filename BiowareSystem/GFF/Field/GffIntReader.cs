using System;

namespace Bioware.GFF.Field
{
    public class GffIntReader : IValueReader
    {
        private int _i;

        public void Parse(string value)
        {
            _i = int.Parse(value);
        }

        public void Parse(GffField field)
        {
            _i = BitConverter.ToInt32(field.Bytes, 0);
        }

        public string TextValue => _i.ToString();

        public byte[] ByteArray => BitConverter.GetBytes(_i);
    }
}