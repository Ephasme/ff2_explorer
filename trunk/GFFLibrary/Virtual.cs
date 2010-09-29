using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using GFFLibrary.GFF;
using Tools;

namespace GFFLibrary.Virtual {
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

    public class VLabel {
        public const string NO_TEXT = "NO_TEXT";
        public const uint NO_ID = uint.MaxValue;
        public const uint LENGTH = 16;
        public string Text { get; set; }
        public uint Index { get; set; }
        public static VLabel EMPTY_LABEL = new VLabel();
        private VLabel() : this(NO_TEXT, NO_ID) { }
        public VLabel(string text, uint index) {
            Text = text;
            Index = index;
        }
    }

    public abstract class VComponent {
        public uint Index { get; set; }
        public VLabel Label { get; set; }
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
        public VComponent(VLabel label, VType type, uint index) {
            Owner = null;
            Label = label;
            Type = type;
            Index = index;
        }
    }
    public sealed class VField : VComponent {
        public VFieldData FieldData { get; set; }
        public VField(VLabel label, VType type, VFieldData vfd, uint index)
            : base(label, type, index) {
            FieldData = vfd;
        }
    }

    public abstract class VComposite : VComponent {
        
        public class HierarchyEventArgs : EventArgs {
            public HierarchyEventArgs()
                : base() {
            }
        }
        public delegate void HierarchyEventHandler(object sender, HierarchyEventArgs ev);
        public event HierarchyEventHandler ComponentAdded;

        private List<VComponent> childs;
        public virtual void Add(VComponent field) {
            if (field is VRoot) {
                throw new ApplicationException("Tentative d'ajout de la racine à un composant composite.");
            } else {
                childs.Add(field);
                field.Owner = this;
            }
            if (ComponentAdded != null) ComponentAdded(this, new HierarchyEventArgs());
        }
        public List<VComponent> Get() {
            return childs;
        }
        public VComposite(VLabel label, VType type, uint index)
            : base(label, type, index) {
            childs = new List<VComponent>();
        }
    }

    public abstract class VStruct : VComposite {
        public uint StructType { set; get; }
        public VStruct(VLabel label, uint index, uint type)
            : base(label, VType.STRUCT, index) {
            StructType = type;
        }
    }
    public sealed class VNormalStruct : VStruct {
        public uint FieldFrameIndex { set; get; }
        public VNormalStruct(VLabel label, uint field_frame_index, uint struct_frame_index, uint struct_type)
            : base(label, struct_frame_index, struct_type) {
            FieldFrameIndex = field_frame_index;
        }
    }
    public sealed class VListedStruct : VStruct {
        public VListedStruct(uint index, uint type)
            : base(VLabel.EMPTY_LABEL, index, type) {
        }
    }
    public sealed class VRoot : VStruct {
        public const int ROOT_INDEX = 0;
        public const uint ROOT_TYPE = uint.MaxValue;
        public VRoot()
            : base(VLabel.EMPTY_LABEL, ROOT_INDEX, ROOT_TYPE) {
        }
    }

    public sealed class VList : VComposite {
        public override void Add(VComponent field) {
            if (field is VStruct) {
                base.Add(field);
            } else {
                throw new ApplicationException("Tentative d'ajout d'autre chose qu'une structure à une liste.");
            }
        }
        public VList(VLabel label, uint index) : base(label, VType.LIST, index) { }
    }
}
