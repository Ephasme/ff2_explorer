namespace Bioware.GFF.Field
{
    public class GffExoLocField : GffField
    {
        public GffExoLocField(GffField field) : base(field.Label, field.Type, field.Value)
        {
            ValueReader = field.ValueReader as GffExoLocStringReader;
        }

        private new GffExoLocStringReader ValueReader { get; }

        public string GetString(Lang lang) => ValueReader.GetString(lang);

        public void SetString(Lang lang, string value)
        {
            ValueReader.SetString(lang, value);
        }
    }
}