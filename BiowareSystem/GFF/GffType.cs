namespace Bioware.GFF
{
    public enum GffType : uint
    {
        Byte,
        Char,
        Word,
        Short,
        Dword,
        Int,
        Dword64,
        Int64,
        Float,
        Double,
        CExoString,
        ResRef,
        CExoLocString,
        Void,
        Struct,
        List,
        Invalid = 4294967295
    }
}