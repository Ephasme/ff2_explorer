using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using DWORD = System.UInt32;

namespace Bioware {
    public struct GConst {
        public const int STRUCT_VALUE_COUNT = 3;
        public const int STRUCT_SIZE = STRUCT_VALUE_COUNT * sizeof(DWORD);
        public const int LABEL_LENGTH = 16;
        public const int RESREF_MAX_LENGTH = 16;
        public const int ROOT_STRUCT_INDEX = 0;
    }
    public enum VType : uint {
        BYTE, CHAR, WORD, SHORT, DWORD, INT, DWORD64, INT64,
        FLOAT, DOUBLE, CEXOSTRING, RESREF, CEXOLOCSTRING,
        VOID, STRUCT, LIST, INVALID = 4294967295
    }
    public struct GFieldSTR {
        public VType Type;
        public DWORD DataOrOffset;
        public DWORD LabelIndex;
    }
    public struct GStructSTR {
        public const uint ROOT_TYPE = 4294967295; // 0xFFFFFFFF (-1)
        public const uint STANDARD_TYPE = 1;
        public const string DEFAULT_LABEL = "struct";
        public DWORD Type;
        public DWORD FieldCount;
        public DWORD DataOrOffset;
    }
    struct GHeaderSTR {
        #region Constantes du tableau des données de l'en-tête.
        public const int STRUCT_OFFSET = 0;
        public const int STRUCT_COUNT = 1;
        public const int FIELD_OFFSET = 2;
        public const int FIELD_COUNT = 3;
        public const int LABEL_OFFSET = 4;
        public const int LABEL_COUNT = 5;
        public const int FIELD_DATA_OFFSET = 6;
        public const int FIELD_DATA_COUNT = 7;
        public const int FIELD_INDICES_OFFSET = 8;
        public const int FIELD_INDICES_COUNT = 9;
        public const int LIST_INDICES_OFFSET = 10;
        public const int LIST_INDICES_COUNT = 11;
        #endregion
        #region Constantes d'information sur la structure.
        public const string VERSION = "V3.2";
        public const int FILE_TYPE_SIZE = 4;
        public const int FILE_VERSION_SIZE = 4;
        public const int DWORD_TABLE_SIZE = 12;
        public const int SIZE = DWORD_TABLE_SIZE * sizeof(DWORD) + (FILE_TYPE_SIZE + FILE_VERSION_SIZE);
        #endregion
        #region Variables contenant les données de l'en-tête.
        public UInt32[] Infos;
        public char[] Type;
        public char[] Version;
        #endregion
        public GHeaderSTR init() {
            Infos = new UInt32[DWORD_TABLE_SIZE];
            Type = new char[FILE_TYPE_SIZE];
            Version = new char[FILE_VERSION_SIZE];
            return this;
        }
    }
}