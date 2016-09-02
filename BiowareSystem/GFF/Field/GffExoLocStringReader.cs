using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Bioware.GFF.Field
{
    public class GffExoLocStringReader : IValueReader
    {
        private Dictionary<int, string> _dic;

        private int _strref;

        public void Parse(string value)
        {
            _dic = new Dictionary<int, string>();
            var locstrList = Regex.Split(value, "\\|\\|");
            _strref = int.Parse(locstrList[0]);
            for (var i = 1; i < locstrList.Length; i++)
            {
                var rgx = new Regex("(?<id>[0-9]+)=(?<value>.*)");
                var m = rgx.Match(locstrList[i]);
                _dic.Add(int.Parse(m.Groups["id"].Value), Decode(m.Groups["value"].Value));
            }
        }

        public void Parse(GffField fld)
        {
            if (fld.Type != GffType.CExoLocString)
            {
                throw new ApplicationException("Impossible de parser ce type " +
                                               Enum.GetName(typeof (GffType), fld.Type) +
                                               " en ExoLocString.");
            }
            var br = new LatinBinaryReader(new MemoryStream(fld.Bytes));
            _strref = (int) br.ReadUInt32();
            var strcount = br.ReadUInt32();
            _dic = new Dictionary<int, string>((int) strcount);
            for (var i = 0; i < strcount; i++)
            {
                var id = br.ReadInt32();
                var size = br.ReadInt32();
                _dic.Add(id, new string(br.ReadChars(size)));
            }
        }

        public string TextValue
        {
            get
            {
                var res = string.Empty;
                res += _strref;
                return _dic.Aggregate(res, (current, kvp) => current + ("||" + kvp.Key) + "=" + Encode(kvp.Value));
            }
        }

        public byte[] ByteArray
        {
            get
            {
                var br = new LatinBinaryWriter(new MemoryStream());
                br.Write((uint) _strref);
                br.Write((uint) _dic.Count);
                foreach (var kvp in _dic)
                {
                    br.Write(kvp.Key);
                    br.Write(kvp.Value.Length);
                    br.Write(kvp.Value.ToCharArray());
                }
                return ((MemoryStream) br.BaseStream).ToArray();
            }
        }

        public string GetString(Lang lang)
        {
            return _dic.ContainsKey((int) lang) ? _dic[(int) lang] : string.Empty;
        }

        public void SetString(Lang lang, string name)
        {
            if (_dic.ContainsKey((int) lang))
            {
                _dic[(int) lang] = name;
            }
        }

        private static string Encode(string p)
        {
            return p.Replace("||", "&DBLBAR&").Replace("=", "&EGAL&");
        }

        private static string Decode(string p)
        {
            return p.Replace("&DBLBAR&", "||").Replace("&EGAL&", "=");
        }
    }
}