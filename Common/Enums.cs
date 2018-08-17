using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Common
{
    public enum AirportType
    {
        Airport,
        SeaPlaneBase,
        Helipad
    }

    public enum SurfaceType
    {
        Asphalt = 1,
        Concrete = 2,
        Grass = 3,
        Dirt = 4,
        Gravel = 5,
        DryLakeBed = 12,
        Water = 13,
        SnowOrIce = 14,
        Transparent = 15
    }

    public enum ShoulderType
    {
        NoShoulder = 0,
        AsphaltShoulder = 1,
        ConcreneShoulder = 2
    }

    public enum RunwayEdgeLights
    {
        NoEdgeLights = 0,
        MediumIntensityEdgeLights = 2
    }

    public enum RunwayMarkings
    {
        NoRunwayMarkings = 0,
        VisualMarkings = 1,
        NonPrecisionApproachMarkings = 2,
        PrecisionApproachMarkings = 3,
        UKStyleNonPrecisionApproachMarkings = 4,
        UKStylePrecisionApproachMarkings = 5
    }

    public enum ApproachLighting
    {
        NoApproachLighting = 0,
        ALSFI = 1,
        ALSFII = 2,
        Calvert = 3,
        CalvertILSCatIIAndCatII = 4,
        SSALR = 5,
        SSALF = 6,
        SALS = 7,
        MALSR = 8,
        MALSF = 9,
        MALS = 10,
        ODALS = 11,
        RAIL = 12
    }

    public enum REILLights
    {
        NoREIL = 0,
        OmniDirectionalREIL = 1,
        UnidirectionalREIL = 2
    }


    public enum LineType
    {
        Nothing = 0,
        SolidYellowLine = 1,
        BrokenYellowLine = 2,
        DoubleSolidYellowLines = 3,
        TwoBrokenYellowLinesAndTwoSolidYellowLines = 4,
        BrokenYellowLineWithParallelSolidYellowLine = 5,
        YellowCrossHatchedLine = 6,
        SolidYellowLineWithBrokenYellowLineOnEachSide = 7,
        WidelySeparatedBrokenYellowLine = 8,

    }

    public enum BeaconType
    {
        NoBeacon = 0,
        WhiteGreenFlashing = 1,
        WhiteYellowFlashing = 2,
        GreenYelloWhiteFlashing = 3,
        WhiteWhiteGreenFlashing = 4
    }

    public enum SignSize
    {
        SmallTaxiwaySign = 1,
        MediumTaxiwaySign = 2,
        LargeTaxiwaySign = 3,
        LargeDistanceRemainingSignOnRunwayEdge = 4,
        SmallDistanceRemainingSignOnRunwayEdge = 5,

    }

    public enum LightingObjectType
    {
        VASI = 1,
        PAPI4L = 2,
        PAPI4R = 3,
        SpaceShuttlePAPI = 4,
        TriColourVASI = 5,
        RunwayGuard = 6
    }

    public enum Direction
    {
        Left,
        Right
    }

    public enum TaxiLocationType
    {
        Gate,
        Hangar,
        Misc,

    }

    public enum AirplaneType
    {
        Heavy,
        Jets,
        Turboprops,
        Props,
        Helos,
        All

    }
}
