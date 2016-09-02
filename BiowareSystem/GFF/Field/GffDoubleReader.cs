using System;
using System.Globalization;

namespace Bioware.GFF.Field
{
    public class GffDoubleReader : IValueReader
    {
        private double _d;

        public void Parse(string value)
        {
            _d = double.Parse(value);
        }

        public void Parse(GffField field)
        {
            _d = BitConverter.ToDouble(field.Bytes, 0);
        }

        public string TextValue => _d.ToString(CultureInfo.InvariantCulture);

        public byte[] ByteArray => BitConverter.GetBytes(_d);
    }
}