using System;

namespace Bioware.GFF.Field
{
    public class GffWordReader : IValueReader
    {
        private ushort _ush;

        public void Parse(string value)
        {
            _ush = ushort.Parse(value);
        }

        public void Parse(GffField field)
        {
            _ush = BitConverter.ToUInt16(field.Bytes, 0);
        }

        public string TextValue => _ush.ToString();

        public byte[] ByteArray => BitConverter.GetBytes(_ush);
    }
}