using System;
using System.Collections.Generic;
using System.IO;

namespace Bioware.Key
{
    public class BiffFile
    {
        private Dictionary<int, BiffResource> _lVr;

        public BiffFile(string path)
        {
            Load(path);
        }

        public string FilePath { get; private set; }

        public BiffResource this[int index] => _lVr[index];

        private void Load(string path)
        {
            FilePath = path;
            if (!File.Exists(path))
            {
                throw new ApplicationException("Le fichier n'existe pas.");
            }
            var br = new LatinBinaryReader(File.OpenRead(path));
            var hd = new BiffHeader(br);
            _lVr = new Dictionary<int, BiffResource>((int) hd.VarResCount);
            br.Stream.Position = hd.VarTableOffset;
            for (var i = 0; i < hd.VarResCount; i++)
            {
                // On dégage l'id qui sert à rien :
                br.BaseStream.Seek(sizeof (int), SeekOrigin.Current);
                var bvr = new BiffResource(br.ReadUInt32(), br.ReadUInt32(), (ResType) br.ReadUInt32());
                _lVr.Add(i, bvr);
            }
        }
    }
}