using System;

namespace Bioware.Erf
{
    public class ErfHeader
    {
        private const int VersionLength = 4;

        private const int TypeLength = 4;

        //int LocalizedStringSize { get; set; } // 32 bit total size (bytes) of Localized String Table

        public readonly int EntryCount; //32 bit number of files packed into the ERF

        //string Version { get; set; } //4 char "V1.0"

        public readonly int LanguageCount; //32 bit number of strings in the Localized String Table

        public readonly int OffsetToKeyList; //32 bit from beginning of file, see figure above

        public readonly int OffsetToLocalizedString; // 32 bit from beginning of file, see figure above

        public readonly int OffsetToResourceList; //32 bit from beginning of file, see figure above

        //const int ReservedSize = 116;

        public ErfHeader(LatinBinaryReader br)
        {
            var pos = br.Stream.Position;
            br.Stream.Position = 0;
            var fileType = new string(br.ReadChars(TypeLength));
            if (fileType == "ERF " || fileType == "MOD " || fileType == "SAV " || fileType == "HAK ")
            {
                //Version = new string(br.ReadChars(VersionLength));
                br.ReadChars(VersionLength);
                LanguageCount = br.ReadInt32();
                //LocalizedStringSize = br.ReadInt32();
                br.ReadInt32();
                EntryCount = br.ReadInt32();
                OffsetToLocalizedString = br.ReadInt32();
                OffsetToKeyList = br.ReadInt32();
                OffsetToResourceList = br.ReadInt32();
                //BuildYear = br.ReadInt32();
                //BuildDay = br.ReadInt32();
                //DescriptionStrRef = br.ReadInt32();
                //Reserved = br.ReadBytes(ReservedSize);
            }
            else
            {
                throw new ApplicationException("Le fichier n'est pas un fichier ERF valide.");
            }
            br.Stream.Position = pos;
        }

        //int BuildYear { get; set; } //4 bytes since 1900

        //int BuildDay { get; set; } //4 bytes since January 1st

        //int DescriptionStrRef { get; set; } //4 bytes strref for file description

        //byte[] Reserved { get; set; } // 116 bytes NULL
    }
}