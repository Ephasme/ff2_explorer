using System;
using System.Text;

namespace Bioware
{
    public class ResRef
    {
        public const int Length = 16;

        private string _value;

        public ResRef(char[] resRef)
        {
            CharTable = resRef;
        }

        public ResRef(string resRef)
        {
            _value = TestLength(resRef);
        }

        public char[] CharTable
        {
            get { return _value.PadRight(Length, '\0').ToCharArray(); }
            set { _value = TestLength(value).TrimEnd('\0').Trim().ToLower(); }
        }

        public static implicit operator string(ResRef resref) => resref.ToString();

        public static explicit operator ResRef(string value) => new ResRef(value);

        public override bool Equals(object obj)
        {
            var rr = obj as ResRef;
            if (rr != null)
            {
                return rr._value == _value;
            }
            return false;
        }

        public override int GetHashCode()
        {
            return BitConverter.ToInt32(Encoding.ASCII.GetBytes(_value), 0);
        }

        public override string ToString()
        {
            return _value;
        }

        private static string TestLength(string resref)
        {
            return TestLength(resref.ToCharArray());
        }

        private static string TestLength(char[] resref)
        {
            if (resref.Length > Length)
            {
                throw new ApplicationException("ResRef trop long.");
            }
            return new string(resref);
        }
    }
}