namespace Bioware.GFF.Composite
{
    public abstract class GffComponent
    {
        protected GffComponent(string label, GffType type)
        {
            Owner = null;
            Label = label;
            Type = type;
        }

        public string Label { get; }
        public GffComposite Owner { get; set; }
        public GffType Type { get; }
        public bool IsRoot => Owner == null;

        public static bool IsSimple(GffType type)
        {
            switch (type)
            {
                case GffType.Byte:
                case GffType.Char:
                case GffType.Word:
                case GffType.Short:
                case GffType.Dword:
                case GffType.Int:
                case GffType.Float:
                    return true;
                default:
                    return false;
            }
        }

        public static bool IsComposite(GffType type)
        {
            return type == GffType.Struct || type == GffType.List;
        }

        public static bool IsLarge(GffType type)
        {
            switch (type)
            {
                case GffType.Dword64:
                case GffType.Int64:
                case GffType.Double:
                case GffType.CExoString:
                case GffType.ResRef:
                case GffType.CExoLocString:
                case GffType.Void:
                    return true;
                default:
                    return false;
            }
        }
    }
}

namespace Bioware.GFF
{
}