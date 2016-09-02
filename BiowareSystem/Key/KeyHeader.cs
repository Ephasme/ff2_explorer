using System.IO;

namespace Bioware.Key
{
    public class KeyHeader
    {
        private const int FileTypeLength = 4;
        private const int FileVersionLength = 4;
        private const int ReservedSize = 32;
        public readonly uint BiffCount; //DWORD Number of BIF files that this KEY file controls
        public readonly uint KeyCount; //DWORD Number of Resources in all BIF files linked to this keyfile
        public readonly uint OffsetToFileTable; //DWORD Byte offset of File Table from beginning of this file
        public readonly uint OffsetToKeyTable;

        public KeyHeader(LatinBinaryReader br)
        {
            // On passe le type et la version de fichier.
            br.BaseStream.Seek(FileTypeLength + FileVersionLength, SeekOrigin.Current);
            BiffCount = br.ReadUInt32();
            KeyCount = br.ReadUInt32();
            OffsetToFileTable = br.ReadUInt32();
            OffsetToKeyTable = br.ReadUInt32();
            br.BaseStream.Seek(sizeof (int)*2 + ReservedSize, SeekOrigin.Current);
            //BuildYear = br.ReadUInt32();
            //BuildDay = br.ReadUInt32();
            //Reserved = br.ReadBytes(ReservedSize);
        }

        //DWORD Byte offset of Key Entry Table from beginning of this file
        //uint BuildYear { get; set; } // DWORD Number of years since 1900
        //uint BuildDay { get; set; } // DWORD Number of days since January 1

        //byte[] Reserved { get; set; } //32 bytes Reserved for future use
    }
}