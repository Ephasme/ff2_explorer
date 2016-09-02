using System;

namespace Bioware.GFF.Field
{
    public class GffDwordReader : IValueReader
    {
        private uint _dword;

        public void Parse(string value)
        {
            _dword = uint.Parse(value);
        }

        public void Parse(GffField field)
        {
            _dword = BitConverter.ToUInt32(field.Bytes, 0);
        }

        public string TextValue => _dword.ToString();

        public byte[] ByteArray => BitConverter.GetBytes(_dword);
    }
}