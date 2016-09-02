namespace Bioware.GFF.Field
{
    public interface IValueReader
    {
        string TextValue { get; }

        byte[] ByteArray { get; }

        void Parse(string value);

        void Parse(GffField field);
    }
}