using Bioware.GFF.Composite;

namespace Bioware.GFF.Field
{
    public class GffField : GffComponent
    {
        public readonly IValueReader ValueReader;
        private GffFieldData _fieldData;

        private GffField(string label, GffType type, GffFieldData gfd) : base(label, type)
        {
            _fieldData = gfd;
            switch (type)
            {
                case GffType.Byte:
                    ValueReader = new GffByteReader();
                    break;
                case GffType.CExoLocString:
                    ValueReader = new GffExoLocStringReader();
                    break;
                case GffType.CExoString:
                    ValueReader = new GffExoStringReader();
                    break;
                case GffType.Char:
                    ValueReader = new GffCharReader();
                    break;
                case GffType.Double:
                    ValueReader = new GffDoubleReader();
                    break;
                case GffType.Dword:
                    ValueReader = new GffDwordReader();
                    break;
                case GffType.Dword64:
                    ValueReader = new GffDword64Reader();
                    break;
                case GffType.Float:
                    ValueReader = new GffFloatReader();
                    break;
                case GffType.Int:
                    ValueReader = new GffIntReader();
                    break;
                case GffType.Int64:
                    ValueReader = new GffInt64Reader();
                    break;
                case GffType.ResRef:
                    ValueReader = new GffResRefReader();
                    break;
                case GffType.Short:
                    ValueReader = new GffShortReader();
                    break;
                case GffType.Word:
                    ValueReader = new GffWordReader();
                    break;
                case GffType.Void:
                    break;
                case GffType.Struct:
                    break;
                case GffType.List:
                    break;
                case GffType.Invalid:
                    break;
                default:
                    ValueReader = null;
                    break;
            }
        }

        public GffField(string label, GffType type, string value) : this(label, type, new GffFieldData())
        {
            Value = value;
        }

        public GffField(string label, GffType type, byte[] data) : this(label, type, new GffFieldData(data))
        {
        }

        public byte[] Bytes => _fieldData.ToArray();

        public string Value
        {
            get
            {
                if (ValueReader == null) return _fieldData.ToString();
                ValueReader.Parse(this);
                return ValueReader.TextValue;
            }
            set
            {
                if (ValueReader != null)
                {
                    ValueReader.Parse(value);
                    _fieldData = new GffFieldData(ValueReader.ByteArray);
                }
                else
                {
                    _fieldData = new GffFieldData(value);
                }
            }
        }
    }
}