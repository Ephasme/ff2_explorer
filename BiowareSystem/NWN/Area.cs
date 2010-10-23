using Bioware.GFF;
using Bioware.GFF.Field;
using Bioware.GFF.Struct;
using Bioware.Resources;

namespace Bioware.NWN {
    public abstract class NObject {}

    public abstract class NTrigger : NObject {
        public const string EXT = ".utw";
        protected GStruct g_str;
        public NTrigger(GStruct g_str) {
            this.g_str = g_str;
        }
    }
    public abstract class NTrap : NTrigger {
        public NTrap(GStruct g_str) : base(g_str) {}
    }
    public abstract class NAreaTransition : NTrigger {
        public NAreaTransition(GStruct g_str) : base(g_str) {}
    }
    public class NiAreaTransition : NAreaTransition {
        public NiAreaTransition(GStruct g_str) : base(g_str) {}
    }
    public class NtAreaTransition : NAreaTransition {
        public const string TemplateResref = "TemplateResRef";
        public const string Geometry = "Geometry";
        private GDocument _gDoc;
        private string _path;
        public NtAreaTransition(string path) : this(new GDocument(path)) {
            _path = path;
        }
        private NtAreaTransition(GDocument gDoc) : base(gDoc.RootStruct) {
            _gDoc = gDoc;
        }
        //CResRef The filename of the UTT file itself. It is an error if this
        //is different. Certain applications check the value of this
        //Field instead of the ResRef of the actual file.
        //If you manually rename a UTT file outside of the
        //toolset, then you must also update the TemplateResRef
        //Field inside it.
        public ResRef TemplateResRef {
            get { return (ResRef) g_str.SelectField(TemplateResref).Value; }
            set { g_str.SelectField(TemplateResref).Value = value; }
        }
    }

    public class NItem : NObject {}
    public class NArea : NObject {
        #region Nom des propriétés.
        #endregion

        #region Propriétés basiques de l'aire.
        /// <summary>
        /// Percent chance of lightning (0-100).
        /// </summary>
        public int ChanceLightning {
            get { return int.Parse(gd_are.RootStruct.SelectField("ChanceLightning").Value); }
            set { gd_are.RootStruct.SelectField("ChanceLightning").Value = value.ToString(); }
        }
        /// <summary>
        /// Percent chance of rain (0-100).
        /// </summary>
        public int ChanceRain {
            get { return int.Parse(gd_are.RootStruct.SelectField("ChanceRain").Value); }
            set { gd_are.RootStruct.SelectField("ChanceRain").Value = value.ToString(); }
        }
        /// <summary>
        /// Percent chance of snow (0-100).
        /// </summary>
        public int ChanceSnow {
            get { return int.Parse(gd_are.RootStruct.SelectField("ChanceSnow").Value); }
            set { gd_are.RootStruct.SelectField("ChanceSnow").Value = value.ToString(); }
        }
        /// <summary>
        /// Module designer comments.
        /// </summary>
        public string Comments {
            get { return gd_are.RootStruct.SelectField("Comments").Value; }
            set { gd_are.RootStruct.SelectField("Comments").Value = value; }
        }
        /// <summary>
        /// 1 if day/night transitions occur, 0 otherwise.
        /// </summary>
        public bool DayNightCycle {
            get { return (gd_are.RootStruct.SelectField("DayNightCycle").Value == "1"); }
            set {
                string res;
                if (value) {
                    res = "1";
                } else {
                    res = "0";
                }
                gd_are.RootStruct.SelectField("DayNightCycle").Value = res;
            }
        }

        /// <summary>
        /// Area size in the y-direction (north-south direction) measured in number of tiles.
        /// </summary>
        public int Height {
            get { return int.Parse(gd_are.RootStruct.SelectField("Height").Value); }
            private set { gd_are.RootStruct.SelectField("Height").Value = value.ToString(); }
        }
        /// <summary>
        /// BYTE 1 if the area is always night, 0 if area is always day.
        /// Meaningful only if DayNightCycle is 0.
        /// </summary>
        public bool IsNight {
            get { return (gd_are.RootStruct.SelectField("IsNight").Value == "1"); }
            set { gd_are.RootStruct.SelectField("IsNight").Value = (value) ? ("1") : ("0"); }
        }
        /// <summary>
        /// BYTE Index into environment.2da
        /// </summary>
        public byte LightingScheme {
            get { return byte.Parse(gd_are.RootStruct.SelectField("LightingScheme").Value); }
            set { gd_are.RootStruct.SelectField("LightingScheme").Value = value.ToString(); }
        }
        /// <summary>
        /// WORD Index into loadscreens.2da. Default loading screen to use when loading this area.
        /// That a Door or Trigger that has an area transition can override the loading screen of the destination area.
        /// </summary>
        public ushort LoadScreenID {
            get { return ushort.Parse(gd_are.RootStruct.SelectField("LoadScreenID").Value); }
            set { gd_are.RootStruct.SelectField("LoadScreenID").Value = value.ToString(); }
        }
        /// <summary>
        /// INT Modifier to Listen akill checks made in area
        /// </summary>
        public int ModListenCheck {
            get { return int.Parse(gd_are.RootStruct.SelectField("ModListenCheck").Value); }
            set { gd_are.RootStruct.SelectField("ModListenCheck").Value = value.ToString(); }
        }
        /// <summary>
        /// INT Modifier to Spot skill checks made in area
        /// </summary>
        public int ModSpotCheck {
            get { return int.Parse(gd_are.RootStruct.SelectField("ModSpotCheck").Value); }
            set { gd_are.RootStruct.SelectField("ModSpotCheck").Value = value.ToString(); }
        }
        /// <summary>
        /// DWORD Nighttime ambient light color (BGR format)
        /// </summary>
        public uint MoonAmbientColor {
            get { return uint.Parse(gd_are.RootStruct.SelectField("MoonAmbientColor").Value); }
            set { gd_are.RootStruct.SelectField("MoonAmbientColor").Value = value.ToString(); }
        }
        /// <summary>
        /// DWORD Nighttime diffuse light color (BGR format)
        /// </summary>
        public uint MoodDiffuseColor {
            get { return uint.Parse(gd_are.RootStruct.SelectField("MoonDiffuseColor").Value); }
            set { gd_are.RootStruct.SelectField("MoonDiffuseColor").Value = value.ToString(); }
        }
        /// <summary>
        /// BYTE Nighttime fog amount (0-15)
        /// </summary>
        public byte MoonFogAmount {
            get { return byte.Parse(gd_are.RootStruct.SelectField("MoonFogAmount").Value); }
            set { gd_are.RootStruct.SelectField("MoonFogAmount").Value = value.ToString(); }
        }
        /// <summary>
        /// DWORD Nighttime fog color (BGR format)
        /// </summary>
        public uint MoonFogColor {
            get { return uint.Parse(gd_are.RootStruct.SelectField("MoonFogColor").Value); }
            set { gd_are.RootStruct.SelectField("MoonFogColor").Value = value.ToString(); }
        }
        /// <summary>
        /// BYTE 1 if shadows appear at night, 0 otherwise
        /// </summary>
        public bool MoonShadows {
            get { return (gd_are.RootStruct.SelectField("MoonShadows").Value == "0") ? (false) : (true); }
            set { gd_are.RootStruct.SelectField("MoonShadows").Value = (value) ? ("1") : ("0"); }
        }
        /// <summary>
        /// BYTE 1 if resting is not allowed, 0 otherwise
        /// </summary>
        public bool NoRest {
            get { return (gd_are.RootStruct.SelectField("NoRest").Value == "1"); }
            set { gd_are.RootStruct.SelectField("NoRest").Value = (value) ? ("1") : ("0"); }
        }
        /// <summary>
        /// CResRef OnEnter event
        /// </summary>
        public ResRef OnEnter {
            get { return (ResRef) gd_are.RootStruct.SelectField("OnEnter").Value; }
            set { gd_are.RootStruct.SelectField("OnEnter").Value = value; }
        }
        /// <summary>
        /// CResRef OnExit event
        /// </summary>
        public ResRef OnExit {
            get { return (ResRef) gd_are.RootStruct.SelectField("OnEnter").Value; }
            set { gd_are.RootStruct.SelectField("OnEnter").Value = value; }
        }
        /// <summary>
        /// CResRef OnHeartbeat event
        /// </summary>
        public ResRef OnHeartbeat {
            get { return (ResRef) gd_are.RootStruct.SelectField("OnHeartbeat").Value; }
            set { gd_are.RootStruct.SelectField("OnHeartbeat").Value = value; }
        }
        /// <summary>
        /// CResRef OnUserDefined event
        /// </summary>
        public ResRef OnUserDefined {
            get { return (ResRef) gd_are.RootStruct.SelectField("OnUserDefined").Value; }
            set { gd_are.RootStruct.SelectField("OnUserDefined").Value = value; }
        }
        /// <summary>
        /// BYTE Index into pvpsettings.2da. Note that the settings are
        /// actually hard-coded into the game, and pvpsettings.2da
        /// serves only to provide text descriptions of the settings
        /// (ie., do not edit pvpsettings.2da).
        /// </summary>
        public byte PlayerVsPlayer {
            get { return byte.Parse(gd_are.RootStruct.SelectField("PlayerVsPlayer").Value); }
            set { gd_are.RootStruct.SelectField("PlayerVsPlayer").Value = value.ToString(); }
        }
        /// <summary>
        /// CResRef Should be identical to the filename of the area
        /// </summary>
        public ResRef ResRef {
            get { return (ResRef) gd_are.RootStruct.SelectField("ResRef").Value; }
            set { gd_are.RootStruct.SelectField("ResRef").Value = value; }
        }
        /// <summary>
        /// BYTE Index into skyboxes.2da (0-255). 0 means no skybox.
        /// </summary>
        public byte SkyBox {
            get { return byte.Parse(gd_are.RootStruct.SelectField("SkyBox").Value); }
            set { gd_are.RootStruct.SelectField("SkyBox").Value = value.ToString(); }
        }
        /// <summary>
        /// BYTE Opacity of shadows (0-100)
        /// </summary>
        public byte ShadowOpacity {
            get { return byte.Parse(gd_are.RootStruct.SelectField("ShadowOpacity").Value); }
            set { gd_are.RootStruct.SelectField("ShadowOpacity").Value = value.ToString(); }
        }
        /// <summary>
        /// DWORD Daytime ambient light color (BGR format)
        /// </summary>
        public uint SunAmbientColor {
            get { return uint.Parse(gd_are.RootStruct.SelectField("SunAmbientColor").Value); }
            set { gd_are.RootStruct.SelectField("SunAmbientColor").Value = value.ToString(); }
        }
        /// <summary>
        /// DWORD Daytime diffuse light color (BGR format)
        /// </summary>
        public uint SunDiffuseColor {
            get { return uint.Parse(gd_are.RootStruct.SelectField("SunDiffuseColor").Value); }
            set { gd_are.RootStruct.SelectField("SunDiffuseColor").Value = value.ToString(); }
        }
        /// <summary>
        /// BYTE Daytime fog amount (0-15)
        /// </summary>
        public byte SunFogAmount {
            get { return byte.Parse(gd_are.RootStruct.SelectField("SunFogAmount").Value); }
            set { gd_are.RootStruct.SelectField("SunFogAmount").Value = value.ToString(); }
        }
        /// <summary>
        /// DWORD Daytime fog color (BGR format)
        /// </summary>
        public uint SunFogColor {
            get { return uint.Parse(gd_are.RootStruct.SelectField("SunFogColor").Value); }
            set { gd_are.RootStruct.SelectField("SunFogColor").Value = value.ToString(); }
        }
        /// <summary>
        /// BYTE 1 if shadows appear during the day, 0 otherwise
        /// </summary>
        public bool SunShadows {
            get { return (gd_are.RootStruct.SelectField("SunShadows").Value == "0") ? (false) : (true); }
            set { gd_are.RootStruct.SelectField("SunShadows").Value = (value) ? ("1") : ("0"); }
        }
        /// <summary>
        /// CExoString Tag of the area, used for scripting
        /// </summary>
        public string Tag {
            get { return gd_are.RootStruct.SelectField("Tag").Value; }
            set { gd_are.RootStruct.SelectField("Tag").Value = value; }
        }
        /// <summary>
        /// CResRef ResRef of the tileset (.SET) file used by the area Version DWORD Revision number of the area. Initially 1 when area is
        /// first saved to disk, and increments every time the ARE file is saved. Equals 2 on second save, and so on.
        /// </summary>
        public ResRef TileSet {
            get { return (ResRef) gd_are.RootStruct.SelectField("Tileset").Value; }
            private set { gd_are.RootStruct.SelectField("Tileset").Value = value; }
        }
        /// <summary>
        /// INT Area size in the x-direction (west-east direction) measured in number of tiles
        /// </summary>
        public int Width {
            get { return int.Parse(gd_are.RootStruct.SelectField("Width").Value); }
            private set { gd_are.RootStruct.SelectField("Width").Value = value.ToString(); }
        }
        /// <summary>
        /// INT Strength of the wind in the area. None, weak, or strong (0-2).
        /// </summary>
        public int WindPower {
            get { return int.Parse(gd_are.RootStruct.SelectField("WindPower").Value); }
            private set { gd_are.RootStruct.SelectField("WindPower").Value = value.ToString(); }
        }

        #region Flags.
        //Flags DWORD Set of bit flags specifying area terrain type:
        //0x0001 = interior (exterior if unset)
        //0x0002 = underground (aboveground if unset)
        //0x0004 = natural (urban if unset)
        //These flags affect game behaviour with respect to
        //ability to hear things behind walls, map exploration
        //visibility, and whether certain feats are active, though
        //not necessarily in that order. They do not affect how the
        //toolset presents the area to the user.
        public bool IsInterior {
            get { return (int.Parse(gd_are.RootStruct.SelectField("Flags").Value) == 0x0001); }
            set { gd_are.RootStruct.SelectField("Flags").Value = (0x0001).ToString(); }
        }
        public bool IsUnderground {
            get { return (int.Parse(gd_are.RootStruct.SelectField("Flags").Value) == 0x0002); }
            set { gd_are.RootStruct.SelectField("Flags").Value = (0x0002).ToString(); }
        }
        public bool IsNatural {
            get { return (int.Parse(gd_are.RootStruct.SelectField("Flags").Value) == 0x0004); }
            set { gd_are.RootStruct.SelectField("Flags").Value = (0x0004).ToString(); }
        }
        #endregion

        /// <param name="langue">Langue dans laquelle on veut récupérer le nom de la map.</param>
        /// <returns>Nom de la map dans la langue demandée.</returns>
        public string GetName(Lang langue) {
            var efld = new GExoLocField(gd_are.RootStruct.SelectField("Name"));
            return efld.GetString(langue);
        }
        /// <param name="langue">Langue dans laquelle on veut définir le nom de l'aire.</param>
        public void SetName(Lang langue, string name) {
            var efld = new GExoLocField(gd_are.RootStruct.SelectField("Name"));
            efld.SetString(langue, name);
        }
        #endregion

        // TODO : Gestion des tiles.
        // Tile_List List List of AreaTiles used in the area. StructID 1.

        private readonly GDocument gd_are;
        private GDocument gd_gic;
        private GDocument gd_git;
        /// <summary>
        /// Permet de créer une aire.
        /// </summary>
        /// <param name="list">L'ordre des paramètres est {.git, .gic, .are}.</param>
        public NArea(params GDocument[] list) {
            gd_git = list[0];
            gd_gic = list[1];
            gd_are = list[2];
        }
        /// <summary>
        /// Permet de créer une aire.
        /// </summary>
        /// <param name="list">L'ordre des paramètres est {.git, .gic, .are}.</param>
        public NArea(params ContentObject[] list)
            : this(
                new GDocument(list[0].DataStream), new GDocument(list[1].DataStream), new GDocument(list[2].DataStream)) {}
    }
}