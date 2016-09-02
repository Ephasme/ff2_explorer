using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Bioware.Erf;
using Bioware.GFF;
using Bioware.GFF.Field;
using Bioware.GFF.Struct;
using Bioware.Key;

namespace Bioware.NWN
{
    public class Module
    {
        public const string AreaName = "Area_Name";
        public const string ModuleDir = "/modules";
        public const string OverrideDir = "/override";
        public const string HakDir = "/hak";
        private static readonly string FileInfoName = "module." + Enum.GetName(typeof (ResType), ResType.Ifo);
        private List<NArea> _areaList;
        private DirectoryInfo _diHak;
        private DirectoryInfo _diModules;
        private DirectoryInfo _diOverride;
        private DirectoryInfo _diRoot;
        private string[] _hakList;
        private ErfFile _mod;
        private GffDocument _modInfo;
        private ResourceManager _rm;

        public Module(string rootPath, string moduleName)
        {
            Load(rootPath, moduleName);
        }

        public string[] HakList
        {
            get
            {
                if (_hakList == null)
                {
                    var modHakstructList = _modInfo.RootStruct.SelectList("Mod_HakList");
                    _hakList =
                        modHakstructList.Select(modHakstruct => (GffField) ((GffStruct) modHakstruct)[0])
                            .Select(fld => _diHak.GetFiles(fld.Value + ErfFile.HakExt)[0].FullName)
                            .ToArray();
                }
                return _hakList;
            }
        }

        public List<NArea> AreaList
        {
            get
            {
                if (_areaList == null)
                {
                    _areaList = new List<NArea>();
                    var glist = _modInfo.RootStruct.SelectList("Mod_Area_list");
                    foreach (var fld in from GffInListStruct ils in glist select ils.SelectField(AreaName))
                    {
                        _areaList.Add(CreateArea(fld.Value));
                    }
                }
                return _areaList;
            }
        }

        public void Load(string rootPath, string moduleName)
        {
            if (!Directory.Exists(rootPath)) return;
            _rm = new ResourceManager();
            moduleName = Path.GetExtension(moduleName) != ErfFile.ModExt
                ? moduleName + ErfFile.ModExt
                : moduleName;

            _diRoot = new DirectoryInfo(rootPath);
            _diModules = new DirectoryInfo(_diRoot.FullName + ModuleDir);
            _diOverride = new DirectoryInfo(_diRoot.FullName + OverrideDir);
            _diHak = new DirectoryInfo(_diRoot.FullName + HakDir);

            _rm.Add(Directory.GetFiles(_diRoot.FullName, "*" + KeyFile.Ext));
            _rm.Add(_diOverride.FullName);

            _mod = new ErfFile(_diModules.FullName + "/" + moduleName);
            _modInfo = new GffDocument(_mod[FileInfoName].DataStream);
            _rm.Add(HakList);

            _rm.Add(_diModules.FullName + "/" + moduleName);
        }

        private NArea CreateArea(string resref)
        {
            var t = typeof (ResType);
            var e =
                new
                {
                    t = "." + Enum.GetName(t, ResType.Git),
                    c = "." + Enum.GetName(t, ResType.Gic),
                    a = "." + Enum.GetName(t, ResType.Are)
                };
            return new NArea(_rm[resref + e.t], _rm[resref + e.c], _rm[resref + e.a]);
        }
    }
}