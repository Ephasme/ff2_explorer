using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GFFSystem.Exception;

namespace GFFSystem.Virtual {
    public enum VType : uint {
        BYTE, CHAR, WORD, SHORT, DWORD, INT, DWORD64, INT64,
        FLOAT, DOUBLE, CEXOSTRING, RESREF, CEXOLOCSTRING,
        VOID, STRUCT, LIST, INVALID = 4294967295
    }
    public class VFieldData {
        private MemoryStream ms_raw_dt;
        public string ToHexaString() {
            string str = "";
            byte[] buf = DataBuffer;
            foreach (byte b in buf) {
                str += String.Format("{0:X}", b).PadLeft(2, '0').ToUpper();
            }
            return str;
        }
        public VFieldData(byte[] datas) {
            ms_raw_dt = new MemoryStream(datas);
        }
        public byte[] DataBuffer {
            get {
                return ms_raw_dt.ToArray();
            }
            set {
                ms_raw_dt = new MemoryStream(value);
            }
        }
    }
    
    public abstract class VComponent {
        public string Label { get; set; }
        public VComposite Owner { get; set; }
        public VType Type { get; private set; }
        public static bool IsSimple(VType type) {
            switch (type) {
                case VType.BYTE:
                case VType.CHAR:
                case VType.WORD:
                case VType.SHORT:
                case VType.DWORD:
                case VType.INT:
                case VType.FLOAT:
                    return true;
                default:
                    return false;
            }
        }
        public static bool IsComposite(VType type) {
            return (type == VType.STRUCT || type == VType.LIST);
        }
        public static bool IsLarge(VType type) {
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
        }
        public VComponent(string label, VType type) {
            Owner = null;
            Label = label;
            Type = type;
        }
    }
    
    public sealed class VField : VComponent {
        public VFieldData FieldData { get; set; }
        public VField(string label, VType type, VFieldData vfd)
            : base(label, type) {
            FieldData = vfd;
        }
    }
    
    public abstract class VComposite : VComponent {
        private List<VComponent> childs;

        public virtual void Add(VComponent field) {
            if (field is VRootStruct) {
                throw new CompositeException(GError.ADD_ROOT_TO_SOMETHING);
            } else {
                childs.Add(field);
                field.Owner = this;
            }
        }
        public List<VComponent> Get() {
            return childs;
        }
        public VComposite(string label, VType type)
            : base(label, type) {
            childs = new List<VComponent>();
        }
    }

    public abstract class VStruct : VComposite {
        public uint StructType { set; get; }
        public VStruct(string label, uint type)
            : base(label, VType.STRUCT) {
            StructType = type;
        }
    }

    public class VInFieldStruct : VStruct {
        public VInFieldStruct(string label, uint type) : base(label, type) { }
    }
    public sealed class VInListStruct : VInFieldStruct {
        public VInListStruct(uint type) : base(null, type) { }
    }
    public sealed class VRootStruct : VInFieldStruct {
        public const uint ROOT_INDEX = 0;
        public const uint ROOT_TYPE = uint.MaxValue;
        public VRootStruct() : base(null, ROOT_TYPE) { }
    }

    public sealed class VList : VComposite {
        public override void Add(VComponent field) {
            if (field is VInListStruct) {
                base.Add(field);
            } else {
                throw new CompositeException(GError.ADD_WRONG_STRUCTURE_CLASS_TO_LIST);
            }
        }
        public VList(string label) : base(label, VType.LIST) { }
    }
}