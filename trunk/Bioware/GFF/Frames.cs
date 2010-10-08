namespace Bioware.GFF {
    abstract class GBasicFrame {
        public const int TYPE = 0;
        public const int VALUE_COUNT = 3;
        public const int SIZE = VALUE_COUNT * sizeof(uint);
        public uint Type {
            get {
                return Datas[TYPE];
            }
            set {
                Datas[TYPE] = value;
            }
        }
        public uint[] Datas { get; set; }
        public GBasicFrame() {
            Datas = new uint[VALUE_COUNT];
        }
    }
    class GFieldFrame : GBasicFrame {
        public const int LABEL_INDEX = 1;
        public const int DATA_OR_DATA_OFFSET = 2;
        public uint LabelIndex {
            get {
                return Datas[LABEL_INDEX];
            }
            set {
                Datas[LABEL_INDEX] = value;
            }
        }
        public uint DataOrDataOffset {
            get {
                return Datas[DATA_OR_DATA_OFFSET];
            }
            set {
                Datas[DATA_OR_DATA_OFFSET] = value;
            }
        }
        public GFieldFrame(uint Type, uint LabelIndex, uint DataOrDataOffset) {
            this.Type = Type;
            this.LabelIndex = LabelIndex;
            this.DataOrDataOffset = DataOrDataOffset;
        }
        public GFieldFrame() : this(0, 0, 0) { }
    }
    class GStructFrame : GBasicFrame {
        public const int DATA_OR_DATA_OFFSET = 1;
        public const int FIELD_COUNT = 2;
        public uint FieldCount {
            get {
                return Datas[FIELD_COUNT];
            }
            set {
                Datas[FIELD_COUNT] = value;
            }
        }
        public uint DataOrDataOffset {
            get {
                return Datas[DATA_OR_DATA_OFFSET];
            }
            set {
                Datas[DATA_OR_DATA_OFFSET] = value;
            }
        }
        public GStructFrame(uint Type, uint DataOrDataOffset, uint FieldCount) {
            this.Type = Type;
            this.FieldCount = FieldCount;
            this.DataOrDataOffset = DataOrDataOffset;
        }
        public GStructFrame() : this(0, 0, 0) { }
    }
}
