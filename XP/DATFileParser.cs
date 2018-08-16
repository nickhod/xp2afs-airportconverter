using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;

namespace XP2AFSAirportConverter.XP
{
    //https://developer.x-plane.com/article/airport-data-apt-dat-file-format-specification
    //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT850-Spec.pdf
    //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT1000-Spec.pdf

    public enum RowCode
    {
        LandAirportHeader = 1,
        SeaplaneBaseHeader = 16,
        HeliportHeader = 17,
        LandRunway = 100,
        WaterRunway = 101,
        Helipad = 102,
        Pavement = 110,
        LinearFeature = 120,
        AirportBoundary = 130,
        Node = 111,
        Node1 = 112,
        Node2 = 113,
        Node3 = 114,
        Node4 = 115,
        Node5 = 116,
        Viewpoint = 14,
        StartupLocation = 15,
        LightBeacon = 18,
        Windsock = 19,
        Signs = 20,
        LightingObjects = 21,
        TrafficFlow = 1000,
        TrafficFlowWindRule = 1001,
        TrafficFlowCeilingRule = 1002,
        TrafficFlowVisibilityRule = 1003,
        TrafficTimeRule = 1004,
        RunwayInUseRule = 1100,
        VFRPatternRule = 1101,
        TaxiRoutingNetwork = 1200,
        TaxiRoutingEdge = 1202,
        EdgeActiveZone = 1204,
        TaxiLocation = 1300,
        Metadata= 1302,
        TruckParkingLocation = 1400,
        TruckDestinationLocation = 1401,
        ATCRecorded = 50,
        ATCUnicom = 51,
        ATCClearance = 52,
        ATCGround = 53,
        ATCTower = 54,
        ATCApproach = 55,
        ATCDeparture = 56
    }

    /*
https://developer.x-plane.com/article/airport-data-apt-dat-file-format-specification
http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT850-Spec.pdf
http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT1000-Spec.pdf
*/

    public class DATFileParser
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        // Some types set a header then data of other row types will follow
        private RowCode ParseMode { get; set; }

        private DATFile datFile;

        public DATFile ParseFromString(string data)
        {
            this.datFile = new DATFile();

            StringReader strReader = new StringReader(data);

            while (true)
            {
                var line = strReader.ReadLine();
                if (line != null)
                {
                    this.ParseLine(line);
                    int i = 0;
                }
                else
                {
                    break;

                }
            }

            return datFile;
        }

        private void ParseLine(string line)
        {
            if (!string.IsNullOrEmpty(line))
            {
                line = this.RemomveDuplicatSpaces(line);

                string[] data = line.Split(' ');
                if (data.Length > 0)
                {
                    if (this.IsNumber(data[0]))
                    {
                        var rowCode = (RowCode)int.Parse(data[0]);

                        switch (rowCode)
                        {
                            case RowCode.LandAirportHeader:
                                this.ParseAirportHeader(data);
                                break;
                            case RowCode.SeaplaneBaseHeader:
                                this.ParseAirportHeader(data);
                                break;
                            case RowCode.HeliportHeader:
                                this.ParseAirportHeader(data);
                                break;
                            case RowCode.LandRunway:
                                this.ParseLandRunway(data);
                                break;
                            case RowCode.WaterRunway:
                                break;
                            case RowCode.Helipad:
                                break;
                            case RowCode.Pavement:
                                break;
                            case RowCode.LinearFeature:
                                break;
                            case RowCode.AirportBoundary:
                                break;
                            case RowCode.Node:
                                break;
                            case RowCode.Node1:
                                break;
                            case RowCode.Node2:
                                break;
                            case RowCode.Node3:
                                break;
                            case RowCode.Node4:
                                break;
                            case RowCode.Node5:
                                break;
                            case RowCode.Viewpoint:
                                break;
                            case RowCode.StartupLocation:
                                break;
                            case RowCode.LightBeacon:
                                break;
                            case RowCode.Windsock:
                                break;
                            case RowCode.Signs:
                                break;
                            case RowCode.LightingObjects:
                                break;
                            case RowCode.TrafficFlow:
                                break;
                            case RowCode.TrafficFlowWindRule:
                                break;
                            case RowCode.TrafficFlowCeilingRule:
                                break;
                            case RowCode.TrafficFlowVisibilityRule:
                                break;
                            case RowCode.TrafficTimeRule:
                                break;
                            case RowCode.RunwayInUseRule:
                                break;
                            case RowCode.VFRPatternRule:
                                break;
                            case RowCode.TaxiRoutingNetwork:
                                break;
                            case RowCode.TaxiRoutingEdge:
                                break;
                            case RowCode.EdgeActiveZone:
                                break;
                            case RowCode.TaxiLocation:
                                break;
                            case RowCode.ATCRecorded:
                                break;
                            case RowCode.ATCUnicom:
                                break;
                            case RowCode.ATCClearance:
                                break;
                            case RowCode.ATCGround:
                                break;
                            case RowCode.ATCTower:
                                break;
                            case RowCode.ATCApproach:
                                break;
                            case RowCode.ATCDeparture:
                                break;
                            case RowCode.Metadata:
                                break;
                            default:
                                break;
                        }
                    }
                }
            }
        }

        private void ParseAirportHeader(string[] data)
        {
            if (data.Length >= 6)
            {
                var airportHeader = new AirportHeader();

                double elevation;
                if (!double.TryParse(data[1], out elevation))
                {
                    log.Error("Error parsing elevation");
                }
                airportHeader.ElevationFeet = elevation;

                airportHeader.ICAOCode = data[4];
                airportHeader.Name = data[5];

                switch (data[0])
                {
                    case "1":
                        airportHeader.AirportType = AirportType.Airport;
                        break;
                    case "16":
                        airportHeader.AirportType = AirportType.SeaPlaneBase;
                        break;
                    case "17":
                        airportHeader.AirportType = AirportType.Helipad;
                        break;
                }

                this.datFile.AirportHeader = airportHeader;
            }
            else
            {
                log.ErrorFormat("Airport header has {0} data elements, which is too few.", data.Length);
            }

        }

        private void ParseLandRunway(string[] data)
        {
            if (data.Length >= 6)
            {
                var landRunway = new LandRunway();

                // 1 - Runway width
                double runwayWidth;
                if (double.TryParse(data[1], out runwayWidth))
                {
                    landRunway.Width = runwayWidth;
                }
                else
                {
                    log.Error("Error parsing runway width");
                }

                // 2 - Surface type
                int surfaceTypeId;
                if (int.TryParse(data[2], out surfaceTypeId))
                {
                    SurfaceType surfaceType = (SurfaceType)surfaceTypeId;
                    landRunway.SurfaceType = surfaceType;
                }
                else
                {
                    log.Error("Error parsing surface type");
                }

                // 3 - Runway shoulder surface type
                int shoulderTypeId;
                if (int.TryParse(data[3], out shoulderTypeId))
                {
                    ShoulderType shoulderType = (ShoulderType)shoulderTypeId;
                    landRunway.ShoulderType = shoulderType;
                }
                else
                {
                    log.Error("Error parsing shoulder type");
                }

                // 4 - Runway smoothness
                double runwaySmoothness;
                if (double.TryParse(data[4], out runwaySmoothness))
                {
                    landRunway.Smoothness = runwaySmoothness;
                }
                else
                {
                    log.Error("Error parsing smoothness");
                }

                // 5 - Runway centre-line lights
                landRunway.HasCenterLineLights = false;
                if (data[5] == "1")
                {
                    landRunway.HasCenterLineLights = true;
                }

                // 6 - Runway edge lighting (also implies threshold lights)
                int edgeLightsInt;
                if (int.TryParse(data[5], out edgeLightsInt))
                {
                    RunwayEdgeLights edgeLights = (RunwayEdgeLights)edgeLightsInt;
                    landRunway.EdgeLights = edgeLights;
                }
                else
                {
                    log.Error("Error parsing runway edge lights");
                }

                // 7 - Auto-generate distance-remaining signs (turn off if created manually)
                landRunway.AutoGenerateDistanceRemainingSigns = false;
                if (data[7] == "1")
                {
                    landRunway.AutoGenerateDistanceRemainingSigns = true;
                }

                // The next two chunks of data are the runway ends
                // Each end is 9 values
                string[] end1Data = data.SubArray(8, 9);
                string[] end2Data = data.SubArray(17, 9);

                landRunway.End1 = ParseLandRunwayEnd(end1Data);
                landRunway.End2 = ParseLandRunwayEnd(end2Data);

                if (this.datFile.LandRunways == null)
                {
                    this.datFile.LandRunways = new List<LandRunway>();
                }

                this.datFile.LandRunways.Add(landRunway);
            }
            {
                log.ErrorFormat("Landrunway has {0} data elements, which is too few.", data.Length);
            }
        }

        private LandRunwayEnd ParseLandRunwayEnd(string[] data)
        {
            var landRunwayEnd = new LandRunwayEnd();

            // 0 - Runway number
            landRunwayEnd.Number = data[0];

            // 1 - Lat
            double latitude;
            if (double.TryParse(data[1], out latitude))
            {
                landRunwayEnd.Latitude = latitude;
            }
            else
            {
                log.Error("Error parsing runway end latitude");
            }

            // 2 - Lon
            double longitude;
            if (double.TryParse(data[2], out longitude))
            {
                landRunwayEnd.Longitude = longitude;
            }
            else
            {
                log.Error("Error parsing runway end longitude");
            }

            // 3 - Length of displaced threshold
            double displacedThreshold;
            if (double.TryParse(data[3], out displacedThreshold))
            {
                landRunwayEnd.LengthOfDisplacedThreshold = displacedThreshold;
            }
            else
            {
                log.Error("Error parsing runway end displaced threshold");
            }

            // 4 - Length of overrun
            double overrun;
            if (double.TryParse(data[4], out overrun))
            {
                landRunwayEnd.LengthOfOverrun = overrun;
            }
            else
            {
                log.Error("Error parsing runway end overrun");
            }

            // 5 - Markings
            int markingsInt;
            if (int.TryParse(data[5], out markingsInt))
            {
                landRunwayEnd.RunwayMarkings = (RunwayMarkings)markingsInt;
            }
            else
            {
                log.Error("Error parsing runway end markings");
            }

            // 6 - Approach lighting
            int approachLightingInt;
            if (int.TryParse(data[6], out approachLightingInt))
            {
                landRunwayEnd.ApproachLighting = (ApproachLighting)approachLightingInt;
            }
            else
            {
                log.Error("Error parsing runway end approach lighting");
            }

            // 7 - Flag for touchdown zone
            landRunwayEnd.FlagForRunwayTouchdownZoneLighting = false;
            if (data[7] == "1")
            {
                landRunwayEnd.FlagForRunwayTouchdownZoneLighting = true;
            }


            // 8 - Code for runway identifier lights
            int identifierLights;
            if (int.TryParse(data[8], out identifierLights))
            {
                landRunwayEnd.REILLights = (REILLights)identifierLights;
            }
            else
            {
                log.Error("Error parsing runway end identifier lights");
            }

            return landRunwayEnd;
        }

        //private void ParseTemplate(string[] data)
        //{
        //    if (data.Length >= 6)
        //    {

        //    }
        //    {
        //        log.ErrorFormat("Airport header has {0} data elements, which is too few.", data.Length);
        //    }
        //}

        private void ParseMetadata(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Metadata has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseSign(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Sign has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseLightingObject(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Lighting object has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseLightBeacon(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Light beacon has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseWindSock(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Windsock has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseViewpoint(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Viewpoint has {0} data elements, which is too few.", data.Length);
            }
        }



        private void ParsePavement(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Pavement has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseNode(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Node has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseStartupLocation(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Startup location has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseTaxiLocation(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("Taxi location has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseVFRPatternRule(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            {
                log.ErrorFormat("VRF Pattern rule has {0} data elements, which is too few.", data.Length);
            }
        }

        private Boolean IsNumber(String s)
        {
            Boolean value = true;
            foreach (Char c in s.ToCharArray())
            {
                value = value && Char.IsDigit(c);
            }

            return value;
        }

        private string RemomveDuplicatSpaces(string line)
        {
            line = Regex.Replace(line, @"\s+", " ");
            return line;
        }



    }
}
