using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DWORD = System.UInt32;

namespace FFR2Explorer {
    public struct GConst {
        #region Constantes de type GFF.
        public const int STRUCT_VALUE_COUNT = 3;
        public const int STRUCT_SIZE = STRUCT_VALUE_COUNT * sizeof(DWORD);
        public const int LABEL_LENGTH = 16;
        public const int RESREF_MAX_LENGTH = 16;
        public const int ROOT_STRUCTURE_INDEX = 0;
        public const int BYTE = 0;
        public const int CHAR = 1;
        public const int WORD = 2;
        public const int SHORT = 3;
        public const int DWORD = 4;
        public const int INT = 5;
        public const int DWORD64 = 6;
        public const int INT64 = 7;
        public const int FLOAT = 8;
        public const int DOUBLE = 9;
        public const int CEXOSTRING = 10;
        public const int RESREF = 11;
        public const int CEXOLOCSTRING = 12;
        public const int VOID = 13;
        public const int STRUCT = 14;
        public const int LIST = 15;
        #endregion
    }
    public struct GFieldSTR {
        public DWORD Type;
        public DWORD DataOrOffset;
        public DWORD LabelIndex;
    }
    public struct GStructSTR {
        public DWORD Type;
        public DWORD FieldCount;
        public DWORD DataOrOffset;
    }
}