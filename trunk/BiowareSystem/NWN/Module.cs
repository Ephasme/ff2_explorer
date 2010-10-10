using Bioware.Resources;
using System.IO;
using Bioware.GFF;
using Bioware.GFF.Composite;
using Bioware.GFF.List;
using System.Collections.Generic;
using Bioware.GFF.Field;
using Bioware.GFF.Struct;
using System;

namespace Bioware.NWN {
    public class Module {
        public const string HAK_LIST = "Mod_HakList";
        public const string AREA_NAME = "Area_Name";
        public const string AREA_LIST = "Mod_Area_list";
        public const string MODULE_DIR = "/modules";
        public const string OVERRIDE_DIR = "/override";
        public const string HAK_DIR = "/hak";
        private static string FILE_INFO_NAME = "module." + Enum.GetName(typeof(ResType), ResType.ifo);
        ResourceManager rm;
        GDocument mod_info;
        List<Area> area_list;
        string[] hak_list;
        DirectoryInfo di_root, di_modules, di_override, di_hak;
        public Module(string root_path, string module_name) {
            Load(root_path, module_name);
        }
        public void Load(string root_path, string module_name) {
            if (Directory.Exists(root_path)) {
                rm = new ResourceManager();
                module_name = (Path.GetExtension(module_name) != EFile.MOD_EXT) ? (module_name + EFile.MOD_EXT) : (module_name);

                di_root = new DirectoryInfo(root_path);
                di_modules = new DirectoryInfo(di_root.FullName + MODULE_DIR);
                di_override = new DirectoryInfo(di_root.FullName + OVERRIDE_DIR);
                di_hak = new DirectoryInfo(di_root.FullName + HAK_DIR);

                rm.Add(Directory.GetFiles(di_root.FullName, "*" + KFile.EXT));
                rm.Add(di_override.FullName);

                var mod = new EFile(di_modules.FullName + "/" + module_name);
                mod_info = new GDocument(mod[FILE_INFO_NAME].DataStream);
                rm.Add(HakList);

                rm.Add(di_modules.FullName + "/" + module_name);
            }
        }

        public string[] HakList {
            get {
                if (hak_list == null) {
                    var hl = new List<string>();
                    var mod_hakstruct_list = mod_info.RootStruct.SelectList(HAK_LIST);
                    foreach (var mod_hakstruct in mod_hakstruct_list) {
                        var fld = ((GField)((GStruct)mod_hakstruct)[0]);
                        var hak_name = di_hak.GetFiles(fld.Value + EFile.HAK_EXT)[0].FullName;
                        hl.Add(hak_name);
                    }
                    hak_list = hl.ToArray();
                }
                return hak_list;
            }
        }
        public List<Area> AreaList {
            get {
                if (area_list == null) {
                    area_list = new List<Area>();
                    var glist = mod_info.RootStruct.SelectList(AREA_LIST);
                    foreach (GInListStruct ils in glist) {
                        var fld = ils.SelectField(AREA_NAME);
                        area_list.Add(CreateArea(fld.Value));
                    }
                }
                return area_list;
            }
        }
        private Area CreateArea(string resref) {
            Type t = typeof(ResType);
            var e = new { t = "." + Enum.GetName(t, ResType.git), c = "." + Enum.GetName(t, ResType.gic), a = "." + Enum.GetName(t, ResType.are) };
            return new Area(rm[resref + e.t], rm[resref + e.c], rm[resref + e.a]);
        }
    }
}
