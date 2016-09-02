using Bioware.GFF;
using Bioware.GFF.Field;

namespace Bioware.NWN
{
    public class NArea : NObject
    {
        // TODO : Gestion des tiles.
        // Tile_List List List of AreaTiles used in the area. StructID 1.

        private readonly GffDocument _gdAre;

        /// <summary>
        ///     Permet de créer une aire.
        /// </summary>
        /// <param name="list">L'ordre des paramètres est {.git, .gic, .are}.</param>
        public NArea(params GffDocument[] list)
        {
            GdGit = list[0];
            GdGic = list[1];
            _gdAre = list[2];
        }

        /// <summary>
        ///     Permet de créer une aire.
        /// </summary>
        /// <param name="list">L'ordre des paramètres est {.git, .gic, .are}.</param>
        public NArea(params ContentObject[] list)
            : this(
                new GffDocument(list[0].DataStream), new GffDocument(list[1].DataStream),
                new GffDocument(list[2].DataStream))
        {
        }

        /// <summary>
        ///     Percent chance of lightning (0-100).
        /// </summary>
        public int ChanceLightning
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("ChanceLightning").Value); }
            set { _gdAre.RootStruct.SelectField("ChanceLightning").Value = value.ToString(); }
        }

        /// <summary>
        ///     Percent chance of rain (0-100).
        /// </summary>
        public int ChanceRain
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("ChanceRain").Value); }
            set { _gdAre.RootStruct.SelectField("ChanceRain").Value = value.ToString(); }
        }

        /// <summary>
        ///     Percent chance of snow (0-100).
        /// </summary>
        public int ChanceSnow
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("ChanceSnow").Value); }
            set { _gdAre.RootStruct.SelectField("ChanceSnow").Value = value.ToString(); }
        }

        /// <summary>
        ///     Module designer comments.
        /// </summary>
        public string Comments
        {
            get { return _gdAre.RootStruct.SelectField("Comments").Value; }
            set { _gdAre.RootStruct.SelectField("Comments").Value = value; }
        }

        /// <summary>
        ///     1 if day/night transitions occur, 0 otherwise.
        /// </summary>
        public bool DayNightCycle
        {
            get { return _gdAre.RootStruct.SelectField("DayNightCycle").Value == "1"; }
            set
            {
                string res;
                if (value)
                {
                    res = "1";
                }
                else
                {
                    res = "0";
                }
                _gdAre.RootStruct.SelectField("DayNightCycle").Value = res;
            }
        }

        /// <summary>
        ///     Area size in the y-direction (north-south direction) measured in number of tiles.
        /// </summary>
        public int Height
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("Height").Value); }
            private set { _gdAre.RootStruct.SelectField("Height").Value = value.ToString(); }
        }

        /// <summary>
        ///     BYTE 1 if the area is always night, 0 if area is always day.
        ///     Meaningful only if DayNightCycle is 0.
        /// </summary>
        public bool IsNight
        {
            get { return _gdAre.RootStruct.SelectField("IsNight").Value == "1"; }
            set { _gdAre.RootStruct.SelectField("IsNight").Value = value ? "1" : "0"; }
        }

        /// <summary>
        ///     BYTE Index into environment.2da
        /// </summary>
        public byte LightingScheme
        {
            get { return byte.Parse(_gdAre.RootStruct.SelectField("LightingScheme").Value); }
            set { _gdAre.RootStruct.SelectField("LightingScheme").Value = value.ToString(); }
        }

        /// <summary>
        ///     WORD Index into loadscreens.2da. Default loading screen to use when loading this area.
        ///     That a Door or Trigger that has an area transition can override the loading screen of the destination area.
        /// </summary>
        public ushort LoadScreenId
        {
            get { return ushort.Parse(_gdAre.RootStruct.SelectField("LoadScreenID").Value); }
            set { _gdAre.RootStruct.SelectField("LoadScreenID").Value = value.ToString(); }
        }

        /// <summary>
        ///     INT Modifier to Listen akill checks made in area
        /// </summary>
        public int ModListenCheck
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("ModListenCheck").Value); }
            set { _gdAre.RootStruct.SelectField("ModListenCheck").Value = value.ToString(); }
        }

        /// <summary>
        ///     INT Modifier to Spot skill checks made in area
        /// </summary>
        public int ModSpotCheck
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("ModSpotCheck").Value); }
            set { _gdAre.RootStruct.SelectField("ModSpotCheck").Value = value.ToString(); }
        }

        /// <summary>
        ///     DWORD Nighttime ambient light color (BGR format)
        /// </summary>
        public uint MoonAmbientColor
        {
            get { return uint.Parse(_gdAre.RootStruct.SelectField("MoonAmbientColor").Value); }
            set { _gdAre.RootStruct.SelectField("MoonAmbientColor").Value = value.ToString(); }
        }

        /// <summary>
        ///     DWORD Nighttime diffuse light color (BGR format)
        /// </summary>
        public uint MoodDiffuseColor
        {
            get { return uint.Parse(_gdAre.RootStruct.SelectField("MoonDiffuseColor").Value); }
            set { _gdAre.RootStruct.SelectField("MoonDiffuseColor").Value = value.ToString(); }
        }

        /// <summary>
        ///     BYTE Nighttime fog amount (0-15)
        /// </summary>
        public byte MoonFogAmount
        {
            get { return byte.Parse(_gdAre.RootStruct.SelectField("MoonFogAmount").Value); }
            set { _gdAre.RootStruct.SelectField("MoonFogAmount").Value = value.ToString(); }
        }

        /// <summary>
        ///     DWORD Nighttime fog color (BGR format)
        /// </summary>
        public uint MoonFogColor
        {
            get { return uint.Parse(_gdAre.RootStruct.SelectField("MoonFogColor").Value); }
            set { _gdAre.RootStruct.SelectField("MoonFogColor").Value = value.ToString(); }
        }

        /// <summary>
        ///     BYTE 1 if shadows appear at night, 0 otherwise
        /// </summary>
        public bool MoonShadows
        {
            get { return _gdAre.RootStruct.SelectField("MoonShadows").Value != "0"; }
            set { _gdAre.RootStruct.SelectField("MoonShadows").Value = value ? "1" : "0"; }
        }

        /// <summary>
        ///     BYTE 1 if resting is not allowed, 0 otherwise
        /// </summary>
        public bool NoRest
        {
            get { return _gdAre.RootStruct.SelectField("NoRest").Value == "1"; }
            set { _gdAre.RootStruct.SelectField("NoRest").Value = value ? "1" : "0"; }
        }

        /// <summary>
        ///     CResRef OnEnter event
        /// </summary>
        public ResRef OnEnter
        {
            get { return (ResRef) _gdAre.RootStruct.SelectField("OnEnter").Value; }
            set { _gdAre.RootStruct.SelectField("OnEnter").Value = value; }
        }

        /// <summary>
        ///     CResRef OnExit event
        /// </summary>
        public ResRef OnExit
        {
            get { return (ResRef) _gdAre.RootStruct.SelectField("OnEnter").Value; }
            set { _gdAre.RootStruct.SelectField("OnEnter").Value = value; }
        }

        /// <summary>
        ///     CResRef OnHeartbeat event
        /// </summary>
        public ResRef OnHeartbeat
        {
            get { return (ResRef) _gdAre.RootStruct.SelectField("OnHeartbeat").Value; }
            set { _gdAre.RootStruct.SelectField("OnHeartbeat").Value = value; }
        }

        /// <summary>
        ///     CResRef OnUserDefined event
        /// </summary>
        public ResRef OnUserDefined
        {
            get { return (ResRef) _gdAre.RootStruct.SelectField("OnUserDefined").Value; }
            set { _gdAre.RootStruct.SelectField("OnUserDefined").Value = value; }
        }

        /// <summary>
        ///     BYTE Index into pvpsettings.2da. Note that the settings are
        ///     actually hard-coded into the game, and pvpsettings.2da
        ///     serves only to provide text descriptions of the settings
        ///     (ie., do not edit pvpsettings.2da).
        /// </summary>
        public byte PlayerVsPlayer
        {
            get { return byte.Parse(_gdAre.RootStruct.SelectField("PlayerVsPlayer").Value); }
            set { _gdAre.RootStruct.SelectField("PlayerVsPlayer").Value = value.ToString(); }
        }

        /// <summary>
        ///     CResRef Should be identical to the filename of the area
        /// </summary>
        public ResRef ResRef
        {
            get { return (ResRef) _gdAre.RootStruct.SelectField("ResRef").Value; }
            set { _gdAre.RootStruct.SelectField("ResRef").Value = value; }
        }

        /// <summary>
        ///     BYTE Index into skyboxes.2da (0-255). 0 means no skybox.
        /// </summary>
        public byte SkyBox
        {
            get { return byte.Parse(_gdAre.RootStruct.SelectField("SkyBox").Value); }
            set { _gdAre.RootStruct.SelectField("SkyBox").Value = value.ToString(); }
        }

        /// <summary>
        ///     BYTE Opacity of shadows (0-100)
        /// </summary>
        public byte ShadowOpacity
        {
            get { return byte.Parse(_gdAre.RootStruct.SelectField("ShadowOpacity").Value); }
            set { _gdAre.RootStruct.SelectField("ShadowOpacity").Value = value.ToString(); }
        }

        /// <summary>
        ///     DWORD Daytime ambient light color (BGR format)
        /// </summary>
        public uint SunAmbientColor
        {
            get { return uint.Parse(_gdAre.RootStruct.SelectField("SunAmbientColor").Value); }
            set { _gdAre.RootStruct.SelectField("SunAmbientColor").Value = value.ToString(); }
        }

        /// <summary>
        ///     DWORD Daytime diffuse light color (BGR format)
        /// </summary>
        public uint SunDiffuseColor
        {
            get { return uint.Parse(_gdAre.RootStruct.SelectField("SunDiffuseColor").Value); }
            set { _gdAre.RootStruct.SelectField("SunDiffuseColor").Value = value.ToString(); }
        }

        /// <summary>
        ///     BYTE Daytime fog amount (0-15)
        /// </summary>
        public byte SunFogAmount
        {
            get { return byte.Parse(_gdAre.RootStruct.SelectField("SunFogAmount").Value); }
            set { _gdAre.RootStruct.SelectField("SunFogAmount").Value = value.ToString(); }
        }

        /// <summary>
        ///     DWORD Daytime fog color (BGR format)
        /// </summary>
        public uint SunFogColor
        {
            get { return uint.Parse(_gdAre.RootStruct.SelectField("SunFogColor").Value); }
            set { _gdAre.RootStruct.SelectField("SunFogColor").Value = value.ToString(); }
        }

        /// <summary>
        ///     BYTE 1 if shadows appear during the day, 0 otherwise
        /// </summary>
        public bool SunShadows
        {
            get { return _gdAre.RootStruct.SelectField("SunShadows").Value != "0"; }
            set { _gdAre.RootStruct.SelectField("SunShadows").Value = value ? "1" : "0"; }
        }

        /// <summary>
        ///     CExoString Tag of the area, used for scripting
        /// </summary>
        public string Tag
        {
            get { return _gdAre.RootStruct.SelectField("Tag").Value; }
            set { _gdAre.RootStruct.SelectField("Tag").Value = value; }
        }

        /// <summary>
        ///     CResRef ResRef of the tileset (.SET) file used by the area Version DWORD Revision number of the area. Initially 1
        ///     when area is
        ///     first saved to disk, and increments every time the ARE file is saved. Equals 2 on second save, and so on.
        /// </summary>
        public ResRef TileSet
        {
            get { return (ResRef) _gdAre.RootStruct.SelectField("Tileset").Value; }
            private set { _gdAre.RootStruct.SelectField("Tileset").Value = value; }
        }

        /// <summary>
        ///     INT Area size in the x-direction (west-east direction) measured in number of tiles
        /// </summary>
        public int Width
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("Width").Value); }
            private set { _gdAre.RootStruct.SelectField("Width").Value = value.ToString(); }
        }

        /// <summary>
        ///     INT Strength of the wind in the area. None, weak, or strong (0-2).
        /// </summary>
        public int WindPower
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("WindPower").Value); }
            private set { _gdAre.RootStruct.SelectField("WindPower").Value = value.ToString(); }
        }

        //Flags DWORD Set of bit flags specifying area terrain type:
        //0x0001 = interior (exterior if unset)
        //0x0002 = underground (aboveground if unset)
        //0x0004 = natural (urban if unset)
        //These flags affect game behaviour with respect to
        //ability to hear things behind walls, map exploration
        //visibility, and whether certain feats are active, though
        //not necessarily in that order. They do not affect how the
        //toolset presents the area to the user.
        public bool IsInterior
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("Flags").Value) == 0x0001; }
            set { _gdAre.RootStruct.SelectField("Flags").Value = 0x0001.ToString(); }
        }

        public bool IsUnderground
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("Flags").Value) == 0x0002; }
            set { _gdAre.RootStruct.SelectField("Flags").Value = 0x0002.ToString(); }
        }

        public bool IsNatural
        {
            get { return int.Parse(_gdAre.RootStruct.SelectField("Flags").Value) == 0x0004; }
            set { _gdAre.RootStruct.SelectField("Flags").Value = 0x0004.ToString(); }
        }

        public GffDocument GdGic { get; }
        public GffDocument GdGit { get; }

        /// <param name="langue">Langue dans laquelle on veut récupérer le nom de la map.</param>
        /// <returns>Nom de la map dans la langue demandée.</returns>
        public string GetName(Lang langue)
        {
            var efld = new GffExoLocField(_gdAre.RootStruct.SelectField("Name"));
            return efld.GetString(langue);
        }

        /// <param name="langue">Langue dans laquelle on veut définir le nom de l'aire.</param>
        /// <param name="name"></param>
        public void SetName(Lang langue, string name)
        {
            var efld = new GffExoLocField(_gdAre.RootStruct.SelectField("Name"));
            efld.SetString(langue, name);
        }
    }
}