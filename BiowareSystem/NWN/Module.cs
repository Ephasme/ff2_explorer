using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using Bioware.GFF;
using Bioware.GFF.Field;
using Bioware.GFF.Struct;
using Bioware.Resources;

namespace Bioware.NWN {
    public class Module {
        public const string AreaName = "Area_Name";
        public const string ModuleDir = "/modules";
        public const string OverrideDir = "/override";
        public const string HakDir = "/hak";
         static readonly string FileInfoName = "module." + Enum.GetName(typeof (ResType), ResType.Ifo);
         List<NArea> _areaList;
         DirectoryInfo _diHak;
         DirectoryInfo _diModules, _diOverride;
         DirectoryInfo _diRoot;
         string[] _hakList;
         EFile _mod;
         GDocument _modInfo;
         ResourceManager _rm;
        public Module(string rootPath, string moduleName) {
            Load(rootPath, moduleName);
        }
        public string[] HakList {
            get {
                if (_hakList == null) {
                    var modHakstructList = _modInfo.RootStruct.SelectList("Mod_HakList");
                    _hakList = modHakstructList.Select(modHakstruct => ((GField) ((GStruct) modHakstruct)[0])).Select(fld => _diHak.GetFiles(fld.Value + EFile.HakExt)[0].FullName).ToArray();
                }
                return _hakList;
            }
        }
        public List<NArea> AreaList {
            get {
                if (_areaList == null) {
                    _areaList = new List<NArea>();
                    var glist = _modInfo.RootStruct.SelectList("Mod_Area_list");
                    foreach (var fld in from GInListStruct ils in glist select ils.SelectField(AreaName)) {
                        _areaList.Add(CreateArea(fld.Value));
                    }
                }
                return _areaList;
            }
        }
        public void Load(string rootPath, string moduleName) {
            if (Directory.Exists(rootPath)) {
                _rm = new ResourceManager();
                moduleName = (Path.GetExtension(moduleName) != EFile.ModExt)
                                  ? (moduleName + EFile.ModExt)
                                  : (moduleName);

                _diRoot = new DirectoryInfo(rootPath);
                _diModules = new DirectoryInfo(_diRoot.FullName + ModuleDir);
                _diOverride = new DirectoryInfo(_diRoot.FullName + OverrideDir);
                _diHak = new DirectoryInfo(_diRoot.FullName + HakDir);

                _rm.Add(Directory.GetFiles(_diRoot.FullName, "*" + KFile.Ext));
                _rm.Add(_diOverride.FullName);

                _mod = new EFile(_diModules.FullName + "/" + moduleName);
                _modInfo = new GDocument(_mod[FileInfoName].DataStream);
                _rm.Add(HakList);

                _rm.Add(_diModules.FullName + "/" + moduleName);
            }
        }
        private NArea CreateArea(string resref) {
            Type t = typeof (ResType);
            var e =
                new {
                        t = "." + Enum.GetName(t, ResType.Git),
                        c = "." + Enum.GetName(t, ResType.Gic),
                        a = "." + Enum.GetName(t, ResType.Are)
                    };
            return new NArea(_rm[resref + e.t], _rm[resref + e.c], _rm[resref + e.a]);
        }
    }
}