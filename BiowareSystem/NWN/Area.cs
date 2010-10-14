using Bioware.GFF;
using Bioware.Resources;
using Bioware.GFF.Field;
namespace Bioware.NWN {
    public class Area {
        #region Nom des propriétés.
        const string CHANCE_LIGHTNING = "ChanceLightning";
        const string CHANCE_RAIN = "ChanceRain";
        const string CHANCE_SNOW = "ChanceSnow";
        const string COMMENTS = "Comments";
        const string DAY_NIGHT_CYCLE = "DayNightCycle";
        const string FLAGS = "Flags";
        const string HEIGHT = "Height";
        const string IS_NIGHT = "IsNight";
        const string LIGHTING_SCHEME = "LightingScheme";
        const string LOAD_SCREEN_ID = "LoadScreenID";
        const string MOD_LISTEN_CHECK = "ModListenCheck";
        const string MOD_SPOT_CHECK = "ModSpotCheck";
        const string MOON_AMBIENT_COLOR = "MoonAmbientColor";
        const string MOON_DIFFUSE_COLOR = "MoonDiffuseColor";
        const string MOON_FOG_AMOUNT = "MoonFogAmount";
        const string MOON_FOG_COLOR = "MoonFogColor";
        const string MOON_SHADOWS = "MoonShadows";
        const string NAME = "Name";
        const string NO_REST = "NoRest";
        const string ON_ENTER = "OnEnter";
        const string ON_HEART_BEAT = "OnHeartbeat";
        const string ON_USER_DEFINED = "OnUserDefined";
        const string PLAYER_VS_PLAYER = "PlayerVsPlayer";
        const string RES_REF = "ResRef";

        #endregion
        #region Propriétés de l'aire.
        /// <summary>
        /// Percent chance of lightning (0-100).
        /// </summary>
        public int ChanceLightning {
            get {
                return int.Parse(gd_are.RootStruct.SelectField(CHANCE_LIGHTNING).Value);
            }
            set {
                gd_are.RootStruct.SelectField(CHANCE_LIGHTNING).Value = value.ToString();
            }
        }
        /// <summary>
        /// Percent chance of rain (0-100).
        /// </summary>
        public int ChanceRain {
            get {
                return int.Parse(gd_are.RootStruct.SelectField(CHANCE_RAIN).Value);
            }
            set {
                gd_are.RootStruct.SelectField(CHANCE_RAIN).Value = value.ToString();
            }
        }
        /// <summary>
        /// Percent chance of snow (0-100).
        /// </summary>
        public int ChanceSnow {
            get {
                return int.Parse(gd_are.RootStruct.SelectField(CHANCE_SNOW).Value);
            }
            set {
                gd_are.RootStruct.SelectField(CHANCE_SNOW).Value = value.ToString();
            }
        }
        /// <summary>
        /// Module designer comments.
        /// </summary>
        public string Comments {
            get {
                return gd_are.RootStruct.SelectField(COMMENTS).Value;
            }
            set {
                gd_are.RootStruct.SelectField(COMMENTS).Value = value;
            }
        }
        /// <summary>
        /// 1 if day/night transitions occur, 0 otherwise.
        /// </summary>
        public bool DayNightCycle {
            get {
                return (gd_are.RootStruct.SelectField(DAY_NIGHT_CYCLE).Value == "1");
            }
            set {
                string res;
                if (value) {
                    res = "1";
                } else {
                    res = "0";
                }
                gd_are.RootStruct.SelectField(DAY_NIGHT_CYCLE).Value = res;
            }
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
            get {
                return (int.Parse(gd_are.RootStruct.SelectField(FLAGS).Value) == 0x0001);
            }
            set {
                gd_are.RootStruct.SelectField(FLAGS).Value = (0x0001).ToString();
            }
        }
        public bool IsUnderground {
            get {
                return (int.Parse(gd_are.RootStruct.SelectField(FLAGS).Value) == 0x0002);
            }
            set {
                gd_are.RootStruct.SelectField(FLAGS).Value = (0x0002).ToString();
            }
        }
        public bool IsNatural {
            get {
                return (int.Parse(gd_are.RootStruct.SelectField(FLAGS).Value) == 0x0004);
            }
            set {
                gd_are.RootStruct.SelectField(FLAGS).Value = (0x0004).ToString();
            }
        }
        #endregion
        /// <summary>
        /// Area size in the y-direction (north-south direction) measured in number of tiles.
        /// </summary>
        public int Height {
            get {
                return int.Parse(gd_are.RootStruct.SelectField(HEIGHT).Value);
            }
        }
        /// <summary>
        /// BYTE 1 if the area is always night, 0 if area is always day.
        /// Meaningful only if DayNightCycle is 0.
        /// </summary>
        public bool IsNight {
            get {
                return (gd_are.RootStruct.SelectField(IS_NIGHT).Value == "1");
            }
            set {
                gd_are.RootStruct.SelectField(IS_NIGHT).Value = (value) ? ("1") : ("0");
            }
        }
        /// <summary>
        /// BYTE Index into environment.2da
        /// </summary>
        public byte LightingScheme {
            get {
                return byte.Parse(gd_are.RootStruct.SelectField(LIGHTING_SCHEME).Value);
            }
            set {
                gd_are.RootStruct.SelectField(LIGHTING_SCHEME).Value = value.ToString();
            }
        }
        /// <summary>
        /// WORD Index into loadscreens.2da. Default loading screen to use when loading this area.
        /// Note that a Door or Trigger that has an area transition can override the loading screen of the destination area.
        /// </summary>
        public ushort LoadScreenID {
            get {
                return ushort.Parse(gd_are.RootStruct.SelectField(LOAD_SCREEN_ID).Value);
            }
            set {
                gd_are.RootStruct.SelectField(LOAD_SCREEN_ID).Value = value.ToString();
            }
        }
        /// <summary>
        /// INT Modifier to Listen akill checks made in area
        /// </summary>
        public int ModListenCheck {
            get {
                return int.Parse(gd_are.RootStruct.SelectField(MOD_LISTEN_CHECK).Value);
            }
            set {
                gd_are.RootStruct.SelectField(MOD_LISTEN_CHECK).Value = value.ToString();
            }
        }
        /// <summary>
        /// INT Modifier to Spot skill checks made in area
        /// </summary>
        public int ModSpotCheck {
            get {
                return int.Parse(gd_are.RootStruct.SelectField(MOD_SPOT_CHECK).Value);
            }
            set {
                gd_are.RootStruct.SelectField(MOD_SPOT_CHECK).Value = value.ToString();
            }
        }
        /// <summary>
        /// DWORD Nighttime ambient light color (BGR format)
        /// </summary>
        public uint MoonAmbientColor {
            get {
                return uint.Parse(gd_are.RootStruct.SelectField(MOON_AMBIENT_COLOR).Value);
            }
            set {
                gd_are.RootStruct.SelectField(MOON_AMBIENT_COLOR).Value = value.ToString();
            }
        }
        /// <summary>
        /// DWORD Nighttime diffuse light color (BGR format)
        /// </summary>
        public uint MoodDiffuseColor {
            get {
                return uint.Parse(gd_are.RootStruct.SelectField(MOON_DIFFUSE_COLOR).Value);
            }
            set {
                gd_are.RootStruct.SelectField(MOON_DIFFUSE_COLOR).Value = value.ToString();
            }
        }
        /// <summary>
        /// BYTE Nighttime fog amount (0-15)
        /// </summary>
        public byte MoonFogAmount {
            get {
                return byte.Parse(gd_are.RootStruct.SelectField(MOON_FOG_AMOUNT).Value);
            }
            set {
                gd_are.RootStruct.SelectField(MOON_FOG_AMOUNT).Value = value.ToString();
            }
        }
        /// <summary>
        /// DWORD Nighttime fog color (BGR format)
        /// </summary>
        public uint MoonFogColor {
            get {
                return uint.Parse(gd_are.RootStruct.SelectField(MOON_FOG_COLOR).Value);
            }
            set {
                gd_are.RootStruct.SelectField(MOON_FOG_COLOR).Value = value.ToString();
            }
        }
        /// <summary>
        /// BYTE 1 if shadows appear at night, 0 otherwise
        /// </summary>
        public bool MoonShadows {
            get {
                return (gd_are.RootStruct.SelectField(MOON_SHADOWS).Value == "0") ? (false) : (true);
            }
            set {
                gd_are.RootStruct.SelectField(MOON_SHADOWS).Value = (value) ? ("1") : ("0");
            }
        }
        /// <param name="langue">Langue dans laquelle on veut récupérer le nom de la map.</param>
        /// <returns>Nom de la map dans la langue demandée.</returns>
        public string GetName(Lang langue) {
            var efld = new GExoLocField(gd_are.RootStruct.SelectField(NAME));
            return efld.GetString(langue);
        }
        /// <param name="langue">Langue dans laquelle on veut définir le nom de l'aire.</param>
        public void SetName(Lang langue, string name) {
            var efld = new GExoLocField(gd_are.RootStruct.SelectField(NAME));
            efld.SetString(langue, name);
        }
        /// <summary>
        /// BYTE 1 if resting is not allowed, 0 otherwise
        /// </summary>
        public bool NoRest {
            get {
                return (gd_are.RootStruct.SelectField(NO_REST).Value == "1");
            }
            set {
                gd_are.RootStruct.SelectField(NO_REST).Value = (value) ? ("1") : ("0");
            }
        }
        /// <summary>
        /// CResRef OnEnter event
        /// </summary>
        public ResRef OnEnter {
            get {
                return (ResRef)gd_are.RootStruct.SelectField(ON_ENTER).Value;
            }
            set {
                gd_are.RootStruct.SelectField(ON_ENTER).Value = value;
            }
        }
        /// <summary>
        /// CResRef OnExit event
        /// </summary>
        public ResRef OnExit {
            get {
                return (ResRef)gd_are.RootStruct.SelectField(ON_ENTER).Value;
            }
            set {
                gd_are.RootStruct.SelectField(ON_ENTER).Value = value;
            }
        }
        /// <summary>
        /// CResRef OnHeartbeat event
        /// </summary>
        public ResRef OnHeartbeat {
            get {
                return (ResRef)gd_are.RootStruct.SelectField(ON_HEART_BEAT).Value;
            }
            set {
                gd_are.RootStruct.SelectField(ON_HEART_BEAT).Value = value;
            }
        }
        /// <summary>
        /// CResRef OnUserDefined event
        /// </summary>
        public ResRef OnUserDefined {
            get {
                return (ResRef)gd_are.RootStruct.SelectField(ON_USER_DEFINED).Value;
            }
            set {
                gd_are.RootStruct.SelectField(ON_USER_DEFINED).Value = value;
            }
        }
        /// <summary>
        /// BYTE Index into pvpsettings.2da. Note that the settings are
        /// actually hard-coded into the game, and pvpsettings.2da
        /// serves only to provide text descriptions of the settings
        /// (ie., do not edit pvpsettings.2da).
        /// </summary>
        public byte PlayerVsPlayer {
            get {
                return byte.Parse(gd_are.RootStruct.SelectField(PLAYER_VS_PLAYER).Value);
            }
            set {
                gd_are.RootStruct.SelectField(PLAYER_VS_PLAYER).Value = value.ToString();
            }
        }
        /// <summary>
        /// CResRef Should be identical to the filename of the area
        /// </summary>
        public ResRef ResRef {
            get {
                return (ResRef)gd_are.RootStruct.SelectField(RES_REF).Value;
            }
            set {
                gd_are.RootStruct.SelectField(RES_REF).Value = value;
            }
        }
        /* Propriétés à implémenter

        /// <summary>
        /// BYTE Index into skyboxes.2da (0-255). 0 means no skybox.
        /// </summary>
        public byte SkyBox {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// BYTE Opacity of shadows (0-100)
        /// </summary>
        public byte ShadowOpacity {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// DWORD Daytime ambient light color (BGR format)
        /// </summary>
        public uint SunAmbientColor {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// DWORD Daytime diffuse light color (BGR format)
        /// </summary>
        public uint SunDiffuseColor {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// DWORD Daytime fog amount (0-15)
        /// </summary>
        public byte SunFogAmount {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// DWORD Daytime fog color (BGR format)
        /// </summary>
        public uint SunFogColor {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// BYTE 1 if shadows appear during the day, 0 otherwise
        /// </summary>
        public bool SunShadows {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// CExoString Tag of the area, used for scripting
        /// </summary>
        public string Tag {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// CResRef ResRef of the tileset (.SET) file used by the area Version DWORD Revision number of the area. Initially 1 when area is
        /// first saved to disk, and increments every time the ARE file is saved. Equals 2 on second save, and so on.
        /// </summary>
        public ResRef TileSet {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// INT Area size in the x-direction (west-east direction) measured in number of tiles
        /// </summary>
        public int Width {
            get {
            }
        }
        /// <summary>
        /// INT Strength of the wind in the area. None, weak, or strong (0-2).
        /// </summary>
        public int WindPower {
            get {
            }
            set {
            }
        }
        */
        #endregion
        // TODO : Gestion des tiles.
        // Tile_List List List of AreaTiles used in the area. StructID 1.

        GDocument gd_git, gd_gic, gd_are;
        /// <summary>
        /// Permet de créer une aire.
        /// </summary>
        /// <param name="list">L'ordre des paramètres est {.git, .gic, .are}.</param>
        public Area(params GDocument[] list) {
            gd_git = list[0];
            gd_gic = list[1];
            gd_are = list[2];
        }
        /// <summary>
        /// Permet de créer une aire.
        /// </summary>
        /// <param name="list">L'ordre des paramètres est {.git, .gic, .are}.</param>
        public Area(params ContentObject[] list) :
            this(new GDocument(list[0].DataStream), new GDocument(list[1].DataStream), new GDocument(list[2].DataStream)) { }
    }
}
