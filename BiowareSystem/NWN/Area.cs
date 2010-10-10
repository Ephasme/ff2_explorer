using Bioware.GFF;
using Bioware.Resources;
namespace Bioware.NWN {
    public class Area {
        const string CHANCE_LIGHTNING = "ChanceLightning";
        const string CHANCE_RAIN = "ChanceRain";
        const string CHANCE_SNOW = "ChanceSnow";
        const string COMMENTS = "Comments";
        const string CREATOR_ID = "Creator_ID";
        const string HEIGHT = "Height";
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
        public int ChangeRain {
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
        /// Deprecated; unused. Always -1.
        /// </summary>
        public int Creator_ID {
            get {
                return -1;
            }
            set {
                gd_are.RootStruct.SelectField(CREATOR_ID).Value = "-1";
            }
        }
        /// <summary>
        /// 1 if day/night transitions occur, 0 otherwise.
        /// </summary>
        public bool DayNightCycle {
            get {
                return (gd_are.RootStruct.SelectField("DayNightCycle").Value == "1");
            }
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
        /* Flags à implémenter
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
            }
            set {
            }
        }
        public bool IsUnderground {
            get {
            }
            set {
            }
        }
        public bool IsNatural {
            get {
            }
            set {
            }
        }
        #endregion
        */
        /// <summary>
        /// Area size in the y-direction (north-south direction) measured in number of tiles.
        /// </summary>
        public int Height {
            get {
                return int.Parse(gd_are.RootStruct.SelectField(HEIGHT).Value);
            }
        }
        /* Propriétés à implémenter
        /// <summary>
        /// BYTE 1 if the area is always night, 0 if area is always day.
        /// Meaningful only if DayNightCycle is 0.
        /// </summary>
        public bool IsNight {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// BYTE Index into environment.2da
        /// </summary>
        public byte LightingScheme {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// WORD Index into loadscreens.2da. Default loading screen to use when loading this area.
        /// Note that a Door or Trigger that has an area transition can override the loading screen of the destination area.
        /// </summary>
        public ushort LoadScreenID {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// INT Modifier to Listen akill checks made in area
        /// </summary>
        public int ModListenCheck {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// INT Modifier to Spot skill checks made in area
        /// </summary>
        public int ModSpotCheck {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// DWORD Nighttime ambient light color (BGR format)
        /// </summary>
        public uint MoonAmbientColor {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// DWORD Nighttime diffuse light color (BGR format)
        /// </summary>
        public uint MoodDiffuseColor {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// BYTE Nighttime fog amount (0-15)
        /// </summary>
        public byte MoonFogAmount {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// DWORD Nighttime fog color (BGR format)
        /// </summary>
        public uint MoonFogColor {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// BYTE 1 if shadows appear at night, 0 otherwise
        /// </summary>
        public bool MoonShadows {
            get {
            }
            set {
            }
        }

        /// <summary>
        /// CExoLocString Name of area as seen in game and in left-hand module
        /// contents treeview in toolset. If there is a colon (:) in the
        /// name, then the game does not show any of the text up
        /// to and including the first colon.
        /// </summary>
        /// <param name="langue">Langue dans laquelle on veut récupérer le nom de la map.</param>
        /// <returns>Nom de la map dans la langue demandée.</returns>
        public string Name(Lang langue) {

        }
        /// <summary>
        /// CExoLocString Name of area as seen in game and in left-hand module
        /// contents treeview in toolset. If there is a colon (:) in the
        /// name, then the game does not show any of the text up
        /// to and including the first colon.
        /// </summary>
        /// <param name="langue">Langue dans laquelle on veut définir le nom de l'aire.</param>
        public void SetName(Lang langue) {
        }
        /// <summary>
        /// BYTE 1 if resting is not allowed, 0 otherwise
        /// </summary>
        public bool NoRest {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// CResRef OnEnter event
        /// </summary>
        public string OnEnter {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// CResRef OnExit event
        /// </summary>
        public string OnExit {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// CResRef OnHeartbeat event
        /// </summary>
        public string OnHeartbeat {
            get {
            }
            set {
            }
        }
        /// <summary>
        /// CResRef OnUserDefined event
        /// </summary>
        public string OnUserDefined {
            get {
            }
            set {
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
            }
            set {
            }
        }
        /// <summary>
        /// CResRef Should be identical to the filename of the area
        /// </summary>
        public ResRef ResRef {
            get {
            }
            set {
            }
        }
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
