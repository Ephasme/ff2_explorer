using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Bioware.Virtual {
    public interface IFileReader {
        VStruct getRootStruct();
    }
}
