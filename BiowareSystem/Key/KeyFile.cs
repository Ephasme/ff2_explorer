using System;
using System.Collections.Generic;
using System.IO;

namespace Bioware.Key
{
    public class KeyFile : Container
    {
        public const string Ext = ".key";
        private const int BitShift = 20;

        public KeyFile()
        {
        }

        public KeyFile(string path)
        {
            Load(path);
        }

        public static bool IsKey(string path)
        {
            return Path.GetExtension(path) == Ext;
        }

        public void Load(string path)
        {
            if (File.Exists(path))
            {
                if (Path.GetExtension(path) == Ext)
                {
                    var lBf = new List<BiffFile>();
                    var br = new LatinBinaryReader(File.OpenRead(path));
                    var kh = new KeyHeader(br);

                    br.Stream.Position = kh.OffsetToFileTable;
                    for (var i = 0; i < kh.BiffCount; i++)
                    {
                        br.Stream.Seek(sizeof (uint), SeekOrigin.Current);
                        var nameOffset = br.ReadUInt32();
                        var nameSize = br.ReadUInt16();
                        br.Stream.Seek(sizeof (ushort), SeekOrigin.Current);

                        var pos = br.Stream.Position;
                        br.Stream.Position = nameOffset;
                        var name = new string(br.ReadChars(nameSize)).TrimEnd('\0').ToLower();
                        br.Stream.Position = pos;

                        lBf.Add(new BiffFile(Path.GetDirectoryName(path) + "/" + name));
                    }

                    br.Stream.Position = kh.OffsetToKeyTable;
                    for (var i = 0; i < kh.KeyCount; i++)
                    {
                        var resref = new ResRef(br.ReadChars(ResRef.Length));
                        br.Stream.Seek(sizeof (ushort), SeekOrigin.Current);
                        var rawid = br.ReadUInt32();
                        var biffId = (int) (rawid >> BitShift);
                        var resId = (int) (rawid - (biffId << BitShift));
                        var res = lBf[biffId][resId];
                        Add(new ContentObject(lBf[biffId].FilePath, resref, res.ResType, res.Offset, res.Size));
                    }
                }
                else
                {
                    throw new ApplicationException("Le fichier n'est pas un fichier .key valide.");
                }
            }
            else
            {
                throw new ApplicationException("Le fichier n'existe pas.");
            }
        }
    }
}