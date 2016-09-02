using System.Text;

namespace Bioware
{
    public static class LatinEncoding
    {
        public const string Name = "ISO-8859-1";
        public static Encoding Latin1 = Encoding.GetEncoding(Name);
    }
}