using System.Collections.Generic;
using System.IO;
using System;
using System.Collections;
using System.Text;
namespace Bioware.Resources {
    public class TDAAdapter {
        public class Row {
            string[] values;
            TDAAdapter ad;
            public Row(string[] values, TDAAdapter ad) {
                this.values = values;
                this.ad = ad;
            }
            public string this[string column_name] {
                get {
                    int index = Array.IndexOf(ad.Columns, column_name) + 1;
                    return values[index];
                }
            }
        }
        public const string BLANK = "****";
        public static char[] SEPARATORS = { ' ' };
        StreamReader srd;
        string FileName;
        string FileVersion;
        string DefaultReturn;
        List<Row> rows;
        public TDAAdapter(ContentObject co) {
            rows = new List<Row>();
            if (co.ResType == ResType.dda) {
                srd = new StreamReader(co.DataStream);
                FileName = co.FileName;
                FileVersion = srd.ReadLine();
                DefaultReturn = srd.ReadLine();
                Columns = srd.ReadLine().Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries);
                while (!srd.EndOfStream) {
                    rows.Add(new Row(srd.ReadLine().Split(SEPARATORS, StringSplitOptions.RemoveEmptyEntries), this));
                }
            } else {
                throw new ApplicationException("L'objet n'est pas un 2da valide.");
            }
        }
        public string[] Columns { get; private set; }
        public Row this[int row_index] {
            get {
                return rows[row_index];
            }
        }
    }
    public class ContentObject {
        uint offset, size;
        public Stream DataStream {
            get {
                BinaryReader br = new BinaryReader(File.OpenRead(FilePath));
                br.Stream.Position = offset;
                return new MemoryStream(br.ReadBytes((int)size));
            }
        }
        public ResRef ResRef { get; private set; }
        public ResType ResType { get; private set; }
        public string FilePath { get; set; }
        public string FileName {
            get {
                return ResRef.String + "." + Enum.GetName(typeof(ResType), ResType);
            }
            set {
                ResType = (ResType)Enum.Parse(typeof(ResType), Path.GetExtension(value).TrimStart('.').Trim().ToLower());
                ResRef = new ResRef(Path.GetFileNameWithoutExtension(value));
            }
        }
        public ContentObject(string path, ResRef resref, ResType restype, uint offset, uint size) {
            ResRef = resref;
            ResType = restype;
            FilePath = path;
            this.offset = offset;
            this.size = size;
        }
        public ContentObject(string path) {
            FileName = Path.GetFileName(path);
            FilePath = path;
            this.offset = 0;
            this.size = (uint)File.OpenRead(path).Length;
        }

    }
    public class Container : IEnumerable {
        private Dictionary<string, ContentObject> Contents;
        public Container() {
            Contents = new Dictionary<string, ContentObject>();
        }
        public void Add(ContentObject cobj) {
            if (Contents.ContainsKey(cobj.FileName)) {
                Contents[cobj.FileName] = cobj;
            } else {
                Contents.Add(cobj.FileName, cobj);
            }
        }
        public bool Remove(ContentObject cobj) {
            return Contents.Remove(cobj.FileName);
        }
        public ContentObject this[string file_name] {
            get {
                try {
                    return Contents[file_name];
                } catch (KeyNotFoundException) {
                    return null;
                }
            }
            set {
                Contents[file_name] = value;
            }
        }

        #region IEnumerable Membres
        public IEnumerator GetEnumerator() {
            return Contents.Values.GetEnumerator();
        }
        #endregion
    }
    public class ResourceManager : Container {
        public void Add(params string[] list) {
            foreach (string path in list) {
                Container c = null;
                if (Directory.Exists(path)) {
                    c = new DirectoryContainer(path);
                } else {
                    if (File.Exists(path)) {
                        if (KFile.IsKEY(path)) {
                            c = new KFile(path);
                        } else if (EFile.IsERF(path)) {
                            c = new EFile(path);
                        } else {
                            throw new ApplicationException("Impossible d'ajouter le chemin spécifié : " + path);
                        }
                    }
                }
                foreach (ContentObject co in c) {
                    base.Add(co);
                }
            }
        }
        public ResourceManager(params string[] list) {
            Add(list);
        }
        public ResourceManager() {
        }
    }
    public class DirectoryContainer : Container {
        public DirectoryContainer() { }
        public DirectoryContainer(string path) {
            Load(path);
        }
        public void Load(string path) {
            if (Directory.Exists(path)) {
                DirectoryInfo di = new DirectoryInfo(path);
                FileInfo[] l_fi = di.GetFiles();
                foreach (FileInfo fi in l_fi) {
                    ContentObject co = new ContentObject(fi.FullName);
                    Add(co);
                }
            }
        }
    }
    public class KFile : Container {
        public static bool IsKEY(string path) {
            return Path.GetExtension(path) == EXT;
        }
        class Biff {
            public class Resource {
                public uint ID { get; private set; }
                public uint Offset { get; private set; }
                public uint Size { get; private set; }
                public ResType ResType { get; private set; } //DWORD Resource type of this resource
                public Resource(uint id, uint offset, uint size, ResType res_type) {
                    ID = id;
                    Offset = offset;
                    Size = size;
                    ResType = res_type;
                }
            }
            class Header {
                public const int FILE_TYPE_LENGTH = 4;
                public const int VERSION_LENGTH = 4;
                public string FileType { get; set; }//4 char "BIFF"
                public string Version { get; set; }// 4 char "V1  "
                public uint VarResCount { get; set; }// DWORD Number of variable resources in this file.
                public uint FixResCount { get; set; }// DWORD Number of fixed resources in this file.
                public uint VarTableOffset { get; set; }// DWORD
                public Header(BinaryReader br) {
                    long pos = br.Stream.Position;
                    br.Stream.Position = 0;
                    FileType = new string(br.ReadChars(FILE_TYPE_LENGTH));
                    Version = new string(br.ReadChars(VERSION_LENGTH));
                    VarResCount = br.ReadUInt32();
                    FixResCount = br.ReadUInt32();
                    VarTableOffset = br.ReadUInt32();
                    br.Stream.Position = pos;
                }
            }
            Dictionary<int, Resource> l_vr;
            public Biff() { }
            public Biff(string path) {
                Load(path);
            }
            public void Load(string path) {
                FilePath = path;
                if (File.Exists(path)) {
                    BinaryReader br = new BinaryReader(File.OpenRead(path));
                    Header hd = new Header(br);
                    l_vr = new Dictionary<int, Resource>((int)hd.VarResCount);
                    br.Stream.Position = hd.VarTableOffset;
                    for (int i = 0; i < hd.VarResCount; i++) {
                        Resource bvr = new Resource(br.ReadUInt32(), br.ReadUInt32(), br.ReadUInt32(), (ResType)br.ReadUInt32());
                        l_vr.Add(i, bvr);
                    }
                } else {
                    throw new ApplicationException("Le fichier n'existe pas.");
                }
            }
            public string FilePath { get; private set; }
            public Resource this[int index] {
                get {
                    return l_vr[index];
                }
            }
        }
        class Header {
            public const int FILE_TYPE_LENGTH = 4;
            public const int FILE_VERSION_LENGTH = 4;
            public const int RESERVED_SIZE = 32;
            public string FileType { get; set; }// 4 char "KEY "
            public string FileVersion { get; set; } //4 char "V1  "
            public uint BiffCount { get; set; }//DWORD Number of BIF files that this KEY file controls
            public uint KeyCount { get; set; }//DWORD Number of Resources in all BIF files linked to this keyfile
            public uint OffsetToFileTable { get; set; }//DWORD Byte offset of File Table from beginning of this file
            public uint OffsetToKeyTable { get; set; }//DWORD Byte offset of Key Entry Table from beginning of this file
            public uint BuildYear { get; set; }// DWORD Number of years since 1900
            public uint BuildDay { get; set; }// DWORD Number of days since January 1
            public byte[] Reserved { get; set; }//32 bytes Reserved for future use
            public Header(BinaryReader br) {
                long pos = br.Stream.Position;
                FileType = new string(br.ReadChars(FILE_TYPE_LENGTH));
                FileVersion = new string(br.ReadChars(FILE_VERSION_LENGTH));
                BiffCount = br.ReadUInt32();
                KeyCount = br.ReadUInt32();
                OffsetToFileTable = br.ReadUInt32();
                OffsetToKeyTable = br.ReadUInt32();
                BuildYear = br.ReadUInt32();
                BuildDay = br.ReadUInt32();
                Reserved = br.ReadBytes(RESERVED_SIZE);
            }
        }
        const string EXT = ".key";
        const int BIT_SHIFT = 20;
        public KFile() : base() { }
        public KFile(string path) {
            Load(path);
        }
        public void Load(string path) {
            if (File.Exists(path)) {
                if (Path.GetExtension(path) == EXT) {
                    List<Biff> l_bf = new List<Biff>();
                    BinaryReader br = new BinaryReader(File.OpenRead(path));
                    Header kh = new Header(br);

                    br.Stream.Position = kh.OffsetToFileTable;
                    for (int i = 0; i < kh.BiffCount; i++) {
                        string biff_path = string.Empty;

                        br.Stream.Seek(sizeof(uint), SeekOrigin.Current);
                        uint name_offset = br.ReadUInt32();
                        ushort name_size = br.ReadUInt16();
                        br.Stream.Seek(sizeof(ushort), SeekOrigin.Current);

                        long pos = br.Stream.Position;
                        br.Stream.Position = name_offset;
                        string name = new string(br.ReadChars(name_size)).TrimEnd('\0').ToLower();
                        br.Stream.Position = pos;

                        l_bf.Add(new Biff(Path.GetDirectoryName(path) + "/" + name));
                    }

                    br.Stream.Position = kh.OffsetToKeyTable;
                    for (int i = 0; i < kh.KeyCount; i++) {
                        ResRef resref = new ResRef(br.ReadChars(ResRef.LENGTH));
                        br.Stream.Seek(sizeof(ushort), SeekOrigin.Current);
                        uint rawid = br.ReadUInt32();
                        int biff_id = (int)(rawid >> BIT_SHIFT);
                        int res_id = (int)(rawid - (biff_id << BIT_SHIFT));
                        Biff.Resource res = l_bf[biff_id][res_id];
                        Add(new ContentObject(l_bf[biff_id].FilePath, resref, res.ResType, res.Offset, res.Size));
                    }
                } else {
                    throw new ApplicationException("Le fichier n'est pas un fichier .key valide.");
                }
            } else {
                throw new ApplicationException("Le fichier n'existe pas.");
            }
        }
    }
    public class EFile : Container {
        public static bool IsERF(string path) {
            string ext = Path.GetExtension(path);
            return (ext == ".mod" || ext == ".erf" || ext == ".sav" || ext == ".hak");
        }
        struct EKeyFrame {
            public ResRef ResRef; // 16 bytes Filename
            public uint ResID; // 32 bit Resource ID, starts at 0 and increments
            public ResType ResType; //16 bit File type
            public ushort Unused;// 16 bit NULLs
            public EKeyFrame(ResRef resref, uint res_id, ResType res_type, ushort unused) {
                ResRef = resref;
                ResID = res_id;
                ResType = (ResType)res_type;
                Unused = unused;
            }
        }
        struct EResFrame {
            public uint OffsetToResource; // 32 bit offset to file data from beginning of ERF
            public uint ResourceSize;// 32 bit number of bytes
            public EResFrame(uint offset_to_res, uint res_size) {
                OffsetToResource = offset_to_res;
                ResourceSize = res_size;
            }
        }
        class EHeader {
            public const string VERSION = "V1.0";
            public const int VERSION_LENGTH = 4;
            public const int TYPE_LENGTH = 4;
            public const int RESERVED_SIZE = 116;
            public string FileType { get; set; } // 4 char "ERF ", "MOD ", "SAV ", "HAK " as appropriate
            public string Version { get; set; } //4 char "V1.0"
            public int LanguageCount { get; set; } //32 bit number of strings in the Localized String Table
            public int LocalizedStringSize { get; set; } // 32 bit total size (bytes) of Localized String Table
            public int EntryCount { get; set; } //32 bit number of files packed into the ERF
            public int OffsetToLocalizedString { get; set; } // 32 bit from beginning of file, see figure above
            public int OffsetToKeyList { get; set; } //32 bit from beginning of file, see figure above
            public int OffsetToResourceList { get; set; } //32 bit from beginning of file, see figure above
            public int BuildYear { get; set; } //4 bytes since 1900
            public int BuildDay { get; set; } //4 bytes since January 1st
            public int DescriptionStrRef { get; set; }//4 bytes strref for file description
            public byte[] Reserved { get; set; }// 116 bytes NULL
            public EHeader(BinaryReader br) {
                long pos = br.Stream.Position;
                br.Stream.Position = 0;
                FileType = new string(br.ReadChars(TYPE_LENGTH));
                if (FileType == "ERF " || FileType == "MOD " || FileType == "SAV " || FileType == "HAK ") {
                    Version = new string(br.ReadChars(VERSION_LENGTH));
                    LanguageCount = br.ReadInt32();
                    LocalizedStringSize = br.ReadInt32();
                    EntryCount = br.ReadInt32();
                    OffsetToLocalizedString = br.ReadInt32();
                    OffsetToKeyList = br.ReadInt32();
                    OffsetToResourceList = br.ReadInt32();
                    BuildYear = br.ReadInt32();
                    BuildDay = br.ReadInt32();
                    DescriptionStrRef = br.ReadInt32();
                    Reserved = br.ReadBytes(RESERVED_SIZE);
                } else {
                    throw new ApplicationException("Le fichier n'est pas un fichier ERF valide.");
                }
                br.Stream.Position = pos;
            }
        }
        Dictionary<uint, string> l_desc;
        public string GetDescription(Lang lang, Gender gdr) {
            uint lang_id = (uint)lang * 2 + (uint)gdr;
            if (l_desc.ContainsKey(lang_id)) {
                return l_desc[lang_id];
            } else {
                return string.Empty;
            }
        }
        public EFile() { }
        public EFile(string path) {
            Load(path);
        }
        public void Load(string path) {
            BinaryReader br = new BinaryReader(new MemoryStream(File.ReadAllBytes(path)));
            EHeader h = new EHeader(br);
            l_desc = new Dictionary<uint, string>();
            Dictionary<EKeyFrame, EResFrame> ri = new Dictionary<EKeyFrame, EResFrame>();

            List<EKeyFrame> keys = new List<EKeyFrame>();
            List<EResFrame> values = new List<EResFrame>();

            long pos = br.Stream.Position;

            br.Stream.Position = h.OffsetToLocalizedString;
            for (int i = 0; i < h.LanguageCount; i++) {
                uint lang_id = br.ReadUInt32();
                uint str_size = br.ReadUInt32();
                string str = new string(br.ReadChars((int)str_size)).TrimEnd('\0');
                l_desc.Add(lang_id, str);
            }

            br.Stream.Position = h.OffsetToKeyList;
            for (int i = 0; i < h.EntryCount; i++) {
                keys.Add(new EKeyFrame(new ResRef(br.ReadChars(ResRef.LENGTH)), br.ReadUInt32(), (ResType)br.ReadUInt16(), br.ReadUInt16()));
            }
            br.Stream.Position = h.OffsetToResourceList;
            for (int i = 0; i < h.EntryCount; i++) {
                values.Add(new EResFrame(br.ReadUInt32(), br.ReadUInt32()));
            }
            for (int i = 0; i < h.EntryCount; i++) {
                Add(new ContentObject(path, keys[i].ResRef, keys[i].ResType, values[i].OffsetToResource, values[i].ResourceSize));
            }
        }
    }
}
