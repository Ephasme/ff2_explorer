using System.IO;
using System.Linq;

namespace Bioware
{
    public class DirectoryContainer : Container
    {
        public DirectoryContainer()
        {
        }

        public DirectoryContainer(string path)
        {
            Load(path);
        }

        public void Load(string path)
        {
            if (!Directory.Exists(path))
            {
                return;
            }
            var di = new DirectoryInfo(path);
            var lFi = di.GetFiles();
            foreach (var co in lFi.Select(fi => new ContentObject(fi.FullName)))
            {
                Add(co);
            }
        }
    }
}