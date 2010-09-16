using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bioware.Virtual {
    public enum VType : uint {
        BYTE, CHAR, WORD, SHORT, DWORD, INT, DWORD64, INT64,
        FLOAT, DOUBLE, CEXOSTRING, RESREF, CEXOLOCSTRING,
        VOID, STRUCT, LIST, INVALID = 4294967295
    }
}
