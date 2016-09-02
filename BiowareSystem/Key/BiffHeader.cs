using System.IO;

namespace Bioware.Key
{
    public class BiffHeader
    {
        private const int FileTypeLength = 4;
        private const int VersionLength = 4;
        // Inutiles :
        //string _fileType; //4 char "BIFF"
        //string _version; // 4 char "V1  "
        //public uint FixResCount;

        public readonly uint VarResCount; // DWORD Number of variable resources in this file.
        public readonly uint VarTableOffset; // DWORD

        public BiffHeader(LatinBinaryReader br)
        {
            var pos = br.Stream.Position;
            br.Stream.Position = 0;
            // Inutile :
            //_fileType = new string(br.ReadChars(FileTypeLength));
            //_version = new string(br.ReadChars(VersionLength));
            // Remplacé par :
            br.BaseStream.Seek(FileTypeLength, SeekOrigin.Current);
            br.BaseStream.Seek(VersionLength, SeekOrigin.Current);
            // --------------
            VarResCount = br.ReadUInt32();
            // Inutile :
            // FixResCount = br.ReadUInt32();
            // Remplacé par :
            br.BaseStream.Seek(sizeof (int), SeekOrigin.Current);
            // --------------
            VarTableOffset = br.ReadUInt32();
            br.Stream.Position = pos;
        }
    }
}