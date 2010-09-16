using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bioware.XML {
    public static class XmlField {
        public const string E_FIELD = "field";
        public const string E_STRUCT = "struct";
        public const string E_LIST = "list";
        public const string E_STRING = "string";

    }

    public static class XmlConst {
        public const string EMPTY_STRING = "";
    }

    public static class XmlAttrib {
        public const string A_LABEL = "label";
        public const string A_INDEX = "index";
        public const string A_TYPE = "type";
        public const string A_KEY = "key";
        public const string A_STRREF = "strref";
    }
}
