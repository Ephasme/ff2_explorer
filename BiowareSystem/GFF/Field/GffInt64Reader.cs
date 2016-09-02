using System;

namespace Bioware.GFF.Field
{
    public class GffInt64Reader : IValueReader
    {
        private long _i64;

        public void Parse(string value)
        {
            _i64 = long.Parse(value);
        }

        public void Parse(GffField field)
        {
            _i64 = BitConverter.ToInt64(field.Bytes, 0);
        }

        public string TextValue => _i64.ToString();

        public byte[] ByteArray => BitConverter.GetBytes(_i64);
    }
}