namespace Bioware.GFF.Struct
{
    public class GffRootStruct : GffInFieldStruct
    {
        public const uint RootIndex = 0;
        public const uint RootType = uint.MaxValue;

        public GffRootStruct(string ext) : base(null, RootType)
        {
            Extention = ext;
        }

        public string Extention { get; }
    }
}