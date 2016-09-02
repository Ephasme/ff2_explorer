using System.Collections.Generic;
using System.IO;

namespace Bioware.GFF
{
    public class GffBase
    {
        public GffBase()
        {
            Header = new GffHeader();
            StructArray = new List<GffStructFrame>();
            FieldArray = new List<GffFieldFrame>();
            LabelArray = new List<string>();
            FieldDataBlock = new MemoryStream();
            FieldIndicesArray = new MemoryStream();
            ListIndicesArray = new MemoryStream();
        }

        public GffHeader Header { get; }
        public List<GffStructFrame> StructArray { get; }
        public List<GffFieldFrame> FieldArray { get; }
        public List<string> LabelArray { get; }
        public MemoryStream FieldDataBlock { get; set; }
        public MemoryStream FieldIndicesArray { get; set; }
        public MemoryStream ListIndicesArray { get; set; }
    }
}