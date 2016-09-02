using System;
using System.Globalization;

namespace Bioware.GFF.Field
{
    internal class GffFloatReader : IValueReader
    {
        private float _f;

        public void Parse(string value)
        {
            _f = float.Parse(value);
        }

        public void Parse(GffField field)
        {
            _f = BitConverter.ToSingle(field.Bytes, 0);
        }

        public string TextValue => _f.ToString(CultureInfo.InvariantCulture);

        public byte[] ByteArray => BitConverter.GetBytes(_f);
    }
}