using Bioware.Resources;
using System.IO;
using Bioware.GFF;
using Bioware.GFF.Composite;
using Bioware.GFF.List;
using System.Collections.Generic;
using Bioware.GFF.Field;
using Bioware.GFF.Struct;

namespace Bioware.NWN {
    public class Module {
        ResourceManager rm;
        GDocument mod_info;
        DirectoryInfo di_root, di_modules, di_override, di_hak;
        public Module(string root_path, string module_name) {
            Load(root_path, module_name);
        }
        public void Load(string root_path, string module_name) {
            if (Directory.Exists(root_path)) {
                rm = new ResourceManager();
                module_name = (Path.GetExtension(module_name) != ".mod") ? (module_name + ".mod") : (module_name);

                di_root = new DirectoryInfo(root_path);
                di_modules = new DirectoryInfo(di_root.FullName + "/modules");
                di_override = new DirectoryInfo(di_root.FullName + "/override");
                di_hak = new DirectoryInfo(di_root.FullName + "/hak");

                rm.Add(Directory.GetFiles(di_root.FullName, "*.key"));
                rm.Add(di_override.FullName);

                EFile mod = new EFile(di_modules.FullName + "/" + module_name);

                mod_info = new GDocument(mod["module.ifo"].DataStream);

                GRootStruct root = mod_info.RootStruct;

                rm.Add(di_modules.FullName + "/" + module_name);
            }
        }
    }
}
