using System.Collections.Generic;
using System.IO;

namespace Bioware.Erf
{
    public class ErfFile : Container
    {
        public const string ModExt = ".mod";

        public const string ErfExt = ".erf";

        public const string SavExt = ".sav";

        public const string HakExt = ".hak";

        private Dictionary<uint, string> _lDesc;

        public ErfFile()
        {
        }

        public ErfFile(string path)
        {
            Load(path);
        }

        public static bool IsErf(string path)
        {
            var ext = Path.GetExtension(path);
            return ext == ModExt || ext == ErfExt || ext == SavExt || ext == HakExt;
        }

        public string GetDescription(Lang lang, Gender gdr)
        {
            var langId = (uint) lang*2 + (uint) gdr;
            return _lDesc.ContainsKey(langId) ? _lDesc[langId] : string.Empty;
        }

        public void Load(string path)
        {
            var br = new LatinBinaryReader(File.OpenRead(path));
            var h = new ErfHeader(br);
            _lDesc = new Dictionary<uint, string>();

            var keys = new List<ErfKeyFrame>();
            var values = new List<ErfResFrame>();

            br.Stream.Position = h.OffsetToLocalizedString;
            for (var i = 0; i < h.LanguageCount; i++)
            {
                var langId = br.ReadUInt32();
                var strSize = br.ReadUInt32();
                var str = new string(br.ReadChars((int) strSize)).TrimEnd('\0');
                _lDesc.Add(langId, str);
            }

            br.Stream.Position = h.OffsetToKeyList;

            for (var i = 0; i < h.EntryCount; i++)
            {
                var resref = new ResRef(br.ReadChars(ResRef.Length));
                br.ReadUInt32(); // ResId
                var restype = (ResType) br.ReadUInt16();
                br.ReadUInt16(); // Unused
                keys.Add(new ErfKeyFrame(resref, restype));
            }

            br.Stream.Position = h.OffsetToResourceList;

            for (var i = 0; i < h.EntryCount; i++)
            {
                values.Add(new ErfResFrame(br.ReadUInt32(), br.ReadUInt32()));
            }

            for (var i = 0; i < h.EntryCount; i++)
            {
                Add(new ContentObject(path, keys[i].ResRef, keys[i].ResType, values[i].OffsetToResource,
                    values[i].ResourceSize));
            }
        }
    }
}