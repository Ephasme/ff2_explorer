using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Bioware
{
    public class Container : IEnumerable
    {
        private readonly Dictionary<string, ContentObject> _contents;

        public Container()
        {
            _contents = new Dictionary<string, ContentObject>();
        }

        public ContentObject this[string fileName]
        {
            get
            {
                try
                {
                    return _contents[fileName];
                }
                catch (KeyNotFoundException)
                {
                    return null;
                }
            }
            set { _contents[fileName] = value; }
        }

        public IEnumerator GetEnumerator()
        {
            return _contents.Values.GetEnumerator();
        }

        public void ExtractAll(string path, Func<ContentObject, bool> extCondMeth)
        {
            if (Directory.Exists(path))
            {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            var list = _contents.Values.Where(extCondMeth);
            foreach (var co in list)
            {
                File.WriteAllBytes(Path.Combine(path, co.FileName), co.DataStream.ToArray());
            }
        }

        public void Extract(string path, string filename)
        {
            File.WriteAllBytes(Path.Combine(path, filename), _contents[filename].DataStream.ToArray());
        }

        public void ExtractAll(string path)
        {
            ExtractAll(path, item => true);
        }

        public void ExtractAll(string path, ResType type)
        {
            ExtractAll(path, item => item.ResType == type);
        }

        public void ExtractAll(string path, string part)
        {
            ExtractAll(path, item => item.FileName.Contains(part));
        }

        public void Add(ContentObject contentObject)
        {
            if (_contents.ContainsKey(contentObject.FileName))
            {
                _contents[contentObject.FileName] = contentObject;
            }
            else
            {
                _contents.Add(contentObject.FileName, contentObject);
            }
        }

        public bool Remove(ContentObject contentObject)
        {
            return _contents.Remove(contentObject.FileName);
        }
    }
}