using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;

namespace Bioware.Resources {
    public class TdaAdapter {
        public const string Blank = "****";
        public static char[] Separators = {' '};
        private readonly List<Row> _rows;
        private readonly StreamReader _srd;
        public TdaAdapter(ContentObject co) {
            _rows = new List<Row>();
            if (co.ResType != ResType.Dda) {
                throw new ApplicationException("L'objet n'est pas un 2da valide.");
            }
            _srd = new StreamReader(co.DataStream);
            _srd.ReadLine(); // Ligne inutile.
            _srd.ReadLine(); // Ligne inutile.
            string line;
            while (!_srd.EndOfStream) {
                line = _srd.ReadLine();
                if (line == null) {
                    break;
                }
                Columns = line.Split(Separators, StringSplitOptions.RemoveEmptyEntries);
                _rows.Add(new Row(Columns, this));
            }
        }
        public string[] Columns { get; private set; }
        public Row this[int rowIndex] {
            get { return _rows[rowIndex]; }
        }

        #region Nested type: Row
        public class Row {
            private readonly TdaAdapter _ad;
            private readonly string[] _values;
            public Row(string[] values, TdaAdapter ad) {
                _values = values;
                _ad = ad;
            }
            public string this[string columnName] {
                get {
                    int index = Array.IndexOf(_ad.Columns, columnName) + 1;
                    return _values[index];
                }
            }
        }
        #endregion
    }
    public class ContentObject {
        private readonly uint _offset;
        private readonly uint _size;
        public ContentObject(string path, ResRef resref, ResType restype, uint offset, uint size) {
            ResRef = resref;
            ResType = restype;
            FilePath = path;
            _offset = offset;
            _size = size;
        }
        public ContentObject(string path) {
            FileName = Path.GetFileName(path);
            FilePath = path;
            _offset = 0;
            _size = (uint) File.OpenRead(path).Length;
        }
        public MemoryStream DataStream {
            get {
                var br = new BinaryReader(File.OpenRead(FilePath)) {Stream = {Position = _offset}};
                return new MemoryStream(br.ReadBytes((int) _size));
            }
        }
        public ResRef ResRef { get; private set; }
        public ResType ResType { get; private set; }
        public string FilePath { get; set; }
        public string FileName {
            get { return ResRef + "." + Enum.GetName(typeof (ResType), ResType); }
            set {
                var ext = Path.GetExtension(value);
                if (ext == null) {
                    throw new NullReferenceException();
                }
                ext = (ext.Substring(1, 1).ToUpper() + ext.Substring(2));
                ResType = (ResType)Enum.Parse(typeof(ResType), ext.TrimStart('.').Trim());
                ResRef = new ResRef(Path.GetFileNameWithoutExtension(value));
            }
        }
    }
    public class Container : IEnumerable {
        #region Delegates
        public delegate bool ExtractCondition(ContentObject co);
        #endregion

        private readonly Dictionary<string, ContentObject> _contents;
        public Container() {
            _contents = new Dictionary<string, ContentObject>();
        }
        public ContentObject this[string fileName] {
            get {
                try {
                    return _contents[fileName];
                } catch (KeyNotFoundException) {
                    return null;
                }
            }
            set { _contents[fileName] = value; }
        }

        #region IEnumerable Membres
        public IEnumerator GetEnumerator() {
            return _contents.Values.GetEnumerator();
        }
        #endregion

        public void ExtractAll(string path, ExtractCondition extCondMeth) {
            if (Directory.Exists(path)) {
                Directory.Delete(path, true);
            }
            Directory.CreateDirectory(path);
            var list = _contents.Values.Where(co => extCondMeth(co));
            foreach (var co in list) {
                File.WriteAllBytes(path + "/" + co.FileName, co.DataStream.ToArray());
            }
        }
        public void Extract(string path, string filename) {
            File.WriteAllBytes(path + "/" + filename, _contents[filename].DataStream.ToArray());
        }
        public void ExtractAll(string path) {
            ExtractAll(path, item => true);
        }
        public void ExtractAll(string path, ResType type) {
            ExtractAll(path, item => item.ResType == type);
        }
        public void ExtractAll(string path, string part) {
            ExtractAll(path, item => item.FileName.Contains(part));
        }
        public void Add(ContentObject cobj) {
            if (_contents.ContainsKey(cobj.FileName)) {
                _contents[cobj.FileName] = cobj;
            } else {
                _contents.Add(cobj.FileName, cobj);
            }
        }
        public bool Remove(ContentObject cobj) {
            return _contents.Remove(cobj.FileName);
        }
    }
    public class ResourceManager : Container {
        public ResourceManager(params string[] list) {
            Add(list);
        }
        public ResourceManager() {}
        public void Add(params string[] list) {
            foreach (string path in list) {
                Container c = null;
                if (Directory.Exists(path)) {
                    c = new DirectoryContainer(path);
                } else {
                    if (File.Exists(path)) {
                        if (KFile.IsKey(path)) {
                            c = new KFile(path);
                        } else if (EFile.IsErf(path)) {
                            c = new EFile(path);
                        } else {
                            throw new ApplicationException("Impossible d'ajouter le chemin spécifié : " + path);
                        }
                    }
                }
                if (c == null) {
                    throw new ApplicationException("Le conteneur de ressource n'est pas initialisé correctement.");
                }
                foreach (ContentObject co in c) {
                    Add(co);
                }
            }
        }
    }
    public class DirectoryContainer : Container {
        public DirectoryContainer() {}
        public DirectoryContainer(string path) {
            Load(path);
        }
        public void Load(string path) {
            if (!Directory.Exists(path)) {
                return;
            }
            var di = new DirectoryInfo(path);
            var lFi = di.GetFiles();
            foreach (var co in lFi.Select(fi => new ContentObject(fi.FullName))) {
                Add(co);
            }
        }
    }
    public class KFile : Container {
        public const string Ext = ".key";
        private const int BitShift = 20;
        public KFile() {}
        public KFile(string path) {
            Load(path);
        }
        public static bool IsKey(string path) {
            return Path.GetExtension(path) == Ext;
        }
        public void Load(string path) {
            if (File.Exists(path)) {
                if (Path.GetExtension(path) == Ext) {
                    var lBf = new List<Biff>();
                    var br = new BinaryReader(File.OpenRead(path));
                    var kh = new Header(br);

                    br.Stream.Position = kh.OffsetToFileTable;
                    for (var i = 0; i < kh.BiffCount; i++) {
                        br.Stream.Seek(sizeof (uint), SeekOrigin.Current);
                        var nameOffset = br.ReadUInt32();
                        var nameSize = br.ReadUInt16();
                        br.Stream.Seek(sizeof (ushort), SeekOrigin.Current);

                        var pos = br.Stream.Position;
                        br.Stream.Position = nameOffset;
                        var name = new string(br.ReadChars(nameSize)).TrimEnd('\0').ToLower();
                        br.Stream.Position = pos;

                        lBf.Add(new Biff(Path.GetDirectoryName(path) + "/" + name));
                    }

                    br.Stream.Position = kh.OffsetToKeyTable;
                    for (var i = 0; i < kh.KeyCount; i++) {
                        var resref = new ResRef(br.ReadChars(ResRef.LENGTH));
                        br.Stream.Seek(sizeof (ushort), SeekOrigin.Current);
                        var rawid = br.ReadUInt32();
                        var biffId = (int) (rawid >> BitShift);
                        var resId = (int) (rawid - (biffId << BitShift));
                        var res = lBf[biffId][resId];
                        Add(new ContentObject(lBf[biffId].FilePath, resref, res.ResType, res.Offset, res.Size));
                    }
                } else {
                    throw new ApplicationException("Le fichier n'est pas un fichier .key valide.");
                }
            } else {
                throw new ApplicationException("Le fichier n'existe pas.");
            }
        }

        #region Nested type: Biff
        private class Biff {
            private Dictionary<int, Resource> _lVr;
            public Biff(string path) {
                Load(path);
            }
            public string FilePath { get; private set; }
            public Resource this[int index] {
                get { return _lVr[index]; }
            }
            private void Load(string path) {
                FilePath = path;
                if (!File.Exists(path)) {
                    throw new ApplicationException("Le fichier n'existe pas.");
                }
                var br = new BinaryReader(File.OpenRead(path));
                var hd = new BiffHeader(br);
                _lVr = new Dictionary<int, Resource>((int) hd.VarResCount);
                br.Stream.Position = hd.VarTableOffset;
                for (var i = 0; i < hd.VarResCount; i++) {
                    // On dégage l'id qui sert à rien :
                    br.BaseStream.Seek(sizeof (int), SeekOrigin.Current);
                    var bvr = new Resource(br.ReadUInt32(), br.ReadUInt32(), (ResType) br.ReadUInt32());
                    _lVr.Add(i, bvr);
                }
            }

            #region Nested type: Header
            private class BiffHeader {
                private const int FileTypeLength = 4;
                private const int VersionLength = 4;
                public BiffHeader(BinaryReader br) {
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
                // Inutiles :
                //string _fileType; //4 char "BIFF"
                //string _version; // 4 char "V1  "
                //public uint FixResCount;

                public readonly uint VarResCount ; // DWORD Number of variable resources in this file.
                public readonly uint VarTableOffset; // DWORD
            }
            #endregion

            #region Nested type: Resource
            public class Resource {
                public Resource(uint offset, uint size, ResType resType) {
                    Offset = offset;
                    Size = size;
                    ResType = resType;
                }
                public uint Offset { get; private set; }
                public uint Size { get; private set; }
                public ResType ResType { get; private set; } //DWORD Resource type of this resource
            }
            #endregion
        }
        #endregion

        #region Nested type: Header
        private class Header {
             const int FileTypeLength = 4;
             const int FileVersionLength = 4;
             const int ReservedSize = 32;
            public Header(BinaryReader br) {
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
            public readonly uint BiffCount; //DWORD Number of BIF files that this KEY file controls
            public readonly uint KeyCount; //DWORD Number of Resources in all BIF files linked to this keyfile
            public readonly uint OffsetToFileTable; //DWORD Byte offset of File Table from beginning of this file
            public readonly uint OffsetToKeyTable;
            //DWORD Byte offset of Key Entry Table from beginning of this file
            //uint BuildYear { get; set; } // DWORD Number of years since 1900
            //uint BuildDay { get; set; } // DWORD Number of days since January 1
            //byte[] Reserved { get; set; } //32 bytes Reserved for future use
        }
        #endregion
    }
    public class EFile : Container {
        public const string ModExt = ".mod";
        public const string ErfExt = ".erf";
        public const string SavExt = ".sav";
        public const string HakExt = ".hak";

        private Dictionary<uint, string> _lDesc;
        public EFile() {}
        public EFile(string path) {
            Load(path);
        }
        public static bool IsErf(string path) {
            var ext = Path.GetExtension(path);
            return (ext == ModExt || ext == ErfExt || ext == SavExt || ext == HakExt);
        }
        public string GetDescription(Lang lang, Gender gdr) {
            var langId = (uint) lang*2 + (uint) gdr;
            return _lDesc.ContainsKey(langId) ? _lDesc[langId] : string.Empty;
        }
        public void Load(string path) {
            var br = new BinaryReader(File.OpenRead(path));
            var h = new EHeader(br);
            _lDesc = new Dictionary<uint, string>();

            var keys = new List<EKeyFrame>();
            var values = new List<EResFrame>();

            br.Stream.Position = h.OffsetToLocalizedString;
            for (var i = 0; i < h.LanguageCount; i++) {
                var langId = br.ReadUInt32();
                var strSize = br.ReadUInt32();
                var str = new string(br.ReadChars((int) strSize)).TrimEnd('\0');
                _lDesc.Add(langId, str);
            }

            br.Stream.Position = h.OffsetToKeyList;
            for (var i = 0; i < h.EntryCount; i++) {
                var resref = new ResRef(br.ReadChars(ResRef.LENGTH));
                br.ReadUInt32(); // ResId
                var restype = (ResType) br.ReadUInt16();
                br.ReadUInt16(); // Unused
                keys.Add(new EKeyFrame(resref,restype));
            }
            br.Stream.Position = h.OffsetToResourceList;
            for (var i = 0; i < h.EntryCount; i++) {
                values.Add(new EResFrame(br.ReadUInt32(), br.ReadUInt32()));
            }
            for (var i = 0; i < h.EntryCount; i++) {
                Add(new ContentObject(path, keys[i].ResRef, keys[i].ResType, values[i].OffsetToResource,
                                      values[i].ResourceSize));
            }
        }

        #region Nested type: EHeader
        private class EHeader {
             const int VersionLength = 4;
             const int TypeLength = 4;
             //const int ReservedSize = 116;
            public EHeader(BinaryReader br) {
                var pos = br.Stream.Position;
                br.Stream.Position = 0;
                _fileType = new string(br.ReadChars(TypeLength));
                if (_fileType == "ERF " || _fileType == "MOD " || _fileType == "SAV " || _fileType == "HAK ") {
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
                } else {
                    throw new ApplicationException("Le fichier n'est pas un fichier ERF valide.");
                }
                br.Stream.Position = pos;
            }
            readonly string _fileType ;// 4 char "ERF ", "MOD ", "SAV ", "HAK " as appropriate
             //string Version { get; set; } //4 char "V1.0"
            public readonly int LanguageCount ; //32 bit number of strings in the Localized String Table
             //int LocalizedStringSize { get; set; } // 32 bit total size (bytes) of Localized String Table
            public readonly int EntryCount ; //32 bit number of files packed into the ERF
            public readonly int OffsetToLocalizedString ; // 32 bit from beginning of file, see figure above
            public readonly int OffsetToKeyList ; //32 bit from beginning of file, see figure above
            public readonly int OffsetToResourceList ; //32 bit from beginning of file, see figure above
             //int BuildYear { get; set; } //4 bytes since 1900
             //int BuildDay { get; set; } //4 bytes since January 1st
             //int DescriptionStrRef { get; set; } //4 bytes strref for file description
             //byte[] Reserved { get; set; } // 116 bytes NULL
        }
        #endregion

        #region Nested type: EKeyFrame
        private struct EKeyFrame {
            public readonly ResRef ResRef; // 16 bytes Filename
            public readonly ResType ResType; //16 bit File type
             //uint ResId; // 32 bit Resource ID, starts at 0 and increments
             //ushort Unused; // 16 bit NULLs
            public EKeyFrame(ResRef resref, ResType resType) {
                ResRef = resref;
                //ResId = res_id;
                ResType = resType;
                //Unused = unused;
            }
        }
        #endregion

        #region Nested type: EResFrame
        private struct EResFrame {
            public readonly uint OffsetToResource; // 32 bit offset to file data from beginning of ERF
            public readonly uint ResourceSize; // 32 bit number of bytes
            public EResFrame(uint offsetToRes, uint resSize) {
                OffsetToResource = offsetToRes;
                ResourceSize = resSize;
            }
        }
        #endregion
    }
}