using System;

namespace Bioware.GFF.Field
{
    public class GffDword64Reader : IValueReader
    {
        private ulong _d64;

        public void Parse(string value)
        {
            _d64 = ulong.Parse(value);
        }

        public void Parse(GffField field)
        {
            _d64 = BitConverter.ToUInt64(field.Bytes, 0);
        }

        public string TextValue => _d64.ToString();

        public byte[] ByteArray => BitConverter.GetBytes(_d64);
    }
}