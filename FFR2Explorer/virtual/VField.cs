using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using DWORD = System.UInt32;
using System.IO;
using Bioware.GFF;
namespace Bioware.Virtual {
    public abstract class VField {
        public String Label { get; set; }
        public VCpsitField Owner { get; set; }
        public VType Type { get; private set; }
        public static bool isComplex(VType type) {
            switch (type) {
                case VType.BYTE:
                case VType.CHAR:
                case VType.WORD:
                case VType.SHORT:
                case VType.DWORD:
                case VType.INT:
                case VType.FLOAT:
                    return false;
                default:
                    return true;
            }
        }
        public static bool isSimple(VType type) {
            return !isComplex(type);
        }
        public static bool isComposite(VType type) {
            return (type == VType.STRUCT || type == VType.LIST);
        }
        public static bool isLarge(VType type) {
            switch (type) {
                case VType.DWORD64:
                case VType.INT64:
                case VType.DOUBLE:
                case VType.CEXOSTRING:
                case VType.RESREF:
                case VType.CEXOLOCSTRING:
                case VType.VOID:
                    return true;
                default:
                    return false;
            }
        }

        public bool IsRoot {
            get {
                return (Owner == null);
            }
            private set { }
        }
        public VField(String label, VType type) {
            Owner = null;
            Label = label;
            Type = type;
        }
    }
}

