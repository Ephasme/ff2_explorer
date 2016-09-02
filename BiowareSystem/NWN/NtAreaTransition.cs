using Bioware.GFF;

namespace Bioware.NWN
{
    public class NtAreaTransition : NAreaTransition
    {
        public const string TemplateResref = "TemplateResRef";
        public const string Geometry = "Geometry";

        public NtAreaTransition(string path) : this(new GffDocument(path))
        {
            Path = path;
        }

        private NtAreaTransition(GffDocument gffDoc) : base(gffDoc.RootStruct)
        {
            GffDoc = gffDoc;
        }

        public GffDocument GffDoc { get; }
        public string Path { get; }
        //CResRef The filename of the UTT file itself. It is an error if this
        //is different. Certain applications check the value of this
        //Field instead of the ResRef of the actual file.
        //If you manually rename a UTT file outside of the
        //toolset, then you must also update the TemplateResRef
        //Field inside it.
        public ResRef TemplateResRef
        {
            get { return (ResRef) GffStr.SelectField(TemplateResref).Value; }
            set { GffStr.SelectField(TemplateResref).Value = value; }
        }
    }
}