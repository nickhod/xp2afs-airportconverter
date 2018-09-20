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


    //https://forums.x-plane.org/index.php?/forums/topic/66713-understanding-the-logic-of-bezier-control-points-in-aptdat/

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
        ATCDeparture = 56,
        Unknown1 = 1050
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

        private IList<Node> temporaryNodeCollection { get; set; }

        /// <summary>
        /// The last "header" line processeed. Does not include nodes
        /// </summary>
        private RowCode LastHeaderRowCode { get; set; }

        public DATFileParser()
        {
            this.temporaryNodeCollection = new List<Node>();
        }

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
                                this.SaveTemporaryNodeCollection();
                                this.ParseAirportHeader(data);
                                break;
                            case RowCode.SeaplaneBaseHeader:
                                this.SaveTemporaryNodeCollection();
                                this.ParseAirportHeader(data);
                                break;
                            case RowCode.HeliportHeader:
                                this.SaveTemporaryNodeCollection();
                                this.ParseAirportHeader(data);
                                break;
                            case RowCode.LandRunway:
                                this.SaveTemporaryNodeCollection();
                                this.ParseLandRunway(data);
                                break;
                            case RowCode.WaterRunway:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.Helipad:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.Pavement:
                                this.SaveTemporaryNodeCollection();
                                this.ParsePavement(data);
                                break;
                            case RowCode.LinearFeature:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.AirportBoundary:
                                this.SaveTemporaryNodeCollection();
                                this.ParseAirportBoundary(data);
                                break;
                            case RowCode.Node:
                                this.ParseNode(data);
                                break;
                            case RowCode.Node1:
                                this.ParseNode(data);
                                break;
                            case RowCode.Node2:
                                this.ParseNode(data);
                                break;
                            case RowCode.Node3:
                                this.ParseNode(data);
                                break;
                            case RowCode.Node4:
                                this.ParseNode(data);
                                break;
                            case RowCode.Node5:
                                this.ParseNode(data);
                                break;
                            case RowCode.Viewpoint:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.StartupLocation:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.LightBeacon:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.Windsock:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.Signs:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.LightingObjects:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TrafficFlow:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TrafficFlowWindRule:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TrafficFlowCeilingRule:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TrafficFlowVisibilityRule:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TrafficTimeRule:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.RunwayInUseRule:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.VFRPatternRule:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TaxiRoutingNetwork:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TaxiRoutingEdge:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.EdgeActiveZone:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.TaxiLocation:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.ATCRecorded:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.ATCUnicom:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.ATCClearance:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.ATCGround:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.ATCTower:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.ATCApproach:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.ATCDeparture:
                                this.SaveTemporaryNodeCollection();
                                break;
                            case RowCode.Metadata:
                                this.ParseMetadata(data);
                                this.SaveTemporaryNodeCollection();
                                break;
                            default:
                                this.SaveTemporaryNodeCollection();
                                break;
                        }


                        // Store the last "header" row processed.
                        // This exclude node rows that "belong" to a specific header
                        if (rowCode != RowCode.Node &&
                            rowCode != RowCode.Node1 &&
                            rowCode != RowCode.Node2 &&
                            rowCode != RowCode.Node3 &&
                            rowCode != RowCode.Node4 &&
                            rowCode != RowCode.Node5)
                        {
                            this.LastHeaderRowCode = rowCode;
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
            if (data.Length >= 26)
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
            else
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
            if (data.Length > 1)
            {
                if (this.datFile.Metadata == null)
                {
                    this.datFile.Metadata = new List<Metadata>();
                }

                string key = data[1];
                string value = data[2];

                if (data.Length > 3)
                {
                    for(int i =3; i < data.Length; i++)
                    {
                        value = value + " " + data[i];
                    }
                }

                var metadata = new Metadata();
                metadata.Key = key;
                metadata.Value = value;
                this.datFile.Metadata.Add(metadata);
            }
            else

            {
                log.ErrorFormat("Metadata has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseSign(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else

            {
                log.ErrorFormat("Sign has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseLightingObject(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else

            {
                log.ErrorFormat("Lighting object has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseLightBeacon(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else

            {
                log.ErrorFormat("Light beacon has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseWindSock(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else

            {
                log.ErrorFormat("Windsock has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseViewpoint(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else

            {
                log.ErrorFormat("Viewpoint has {0} data elements, which is too few.", data.Length);
            }
        }



        private void ParsePavement(string[] data)
        {
            if (data.Length >= 5)
            {
                var pavement = new Pavement();

                // Surface type
                int surfaceTypeId;
                if (int.TryParse(data[1], out surfaceTypeId))
                {
                    SurfaceType surfaceType = (SurfaceType)surfaceTypeId;
                    pavement.SurfaceType = surfaceType;
                }
                else
                {
                    log.Error("Error parsing surface type");
                }

                // Smoothness
                double smoothness;
                if (double.TryParse(data[2], out smoothness))
                {
                    pavement.Smoothness = smoothness;
                }
                else
                {
                    log.Error("Error parsing smoothness");
                }

                // Orientation of texture
                double orientation;
                if (double.TryParse(data[3], out orientation))
                {
                    pavement.Orientation = orientation;
                }
                else
                {
                    log.Error("Error parsing orientation");
                }

                // Description
                pavement.Description = data[4];

                if (this.datFile.Pavements == null)
                {
                    this.datFile.Pavements = new List<Pavement>();
                }
                this.datFile.Pavements.Add(pavement);
            }
            else

            {
                log.ErrorFormat("Pavement has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseNode(string[] data)
        {
            Node node = new Node();

            int nodeRowCode;
            if (!int.TryParse(data[0], out nodeRowCode))
            {
                log.Error("Error parsing node code");
            }

            // Every node type has a lat / lon
            double latitude;
            if (double.TryParse(data[1], out latitude))
            {
                node.Latitude = latitude;
            }
            else
            {
                log.Error("Error parsing node latitude");
            }

            // Every node type has a lat / lon
            double longitude;
            if (double.TryParse(data[2], out longitude))
            {
                node.Longitude = longitude;
            }
            else
            {
                log.Error("Error parsing node longitude");
            }

            int lineTypeIndex = 3;
            int lightingLineTypeIndex = 4;

            // The line type index of these nodes types is shifted due ot the bezier control point
            if (nodeRowCode == 112|| nodeRowCode == 114)
            {
                lineTypeIndex= 5;
                lightingLineTypeIndex = 6;
            }

            // All node types apart from these may have line types
            if (nodeRowCode != 115 && nodeRowCode != 116)
            {
                int expectedDataLengthWithLineTypes = 5;

                if (nodeRowCode == 112 || nodeRowCode == 114)
                {
                    expectedDataLengthWithLineTypes = 7;
                }

                if (data.Length == expectedDataLengthWithLineTypes)
                {
                    int lineTypeInt;
                    if (int.TryParse(data[lineTypeIndex], out lineTypeInt))
                    {
                        node.LineType = (LineType)lineTypeInt;
                    }
                    else
                    {
                        log.Error("Error parsing node line type");
                    }

                    int lightingLineTypeInt;
                    if (int.TryParse(data[lightingLineTypeIndex], out lightingLineTypeInt))
                    {
                        node.LightingLineType = (LineType)lightingLineTypeInt;
                    }
                    else
                    {
                        log.Error("Error parsing node lighting line type");
                    }
                }


            }

            // These node types have a bezier control point
            if (nodeRowCode == 112 || nodeRowCode == 114 || nodeRowCode == 116)
            {
                double bezierLatitude;
                if (double.TryParse(data[3], out bezierLatitude))
                {
                    node.BezierControlPoint1Latitude = bezierLatitude;
                }
                else
                {
                    log.Error("Error parsing node bezier latitude");
                }

                double bezierLongitude;
                if (double.TryParse(data[4], out bezierLongitude))
                {
                    node.BezierControlPoint1Longitude = bezierLongitude;
                }
                else
                {
                    log.Error("Error parsing node bezier longitude");
                }
            }

            // These node types close a loop
            if (nodeRowCode == 113 || nodeRowCode == 114)
            {
                node.CloseLoop = true;
            }

            if (nodeRowCode == 115 || nodeRowCode == 116)
            {
                node.End = true;
            }

            this.temporaryNodeCollection.Add(node);
        }

        private void ParseStartupLocation(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else

            {
                log.ErrorFormat("Startup location has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseTaxiLocation(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else

            {
                log.ErrorFormat("Taxi location has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseVFRPatternRule(string[] data)
        {
            if (data.Length >= 6)
            {

            }
            else
            {
                log.ErrorFormat("VRF Pattern rule has {0} data elements, which is too few.", data.Length);
            }
        }

        private void ParseAirportBoundary(string[] data)
        {
            if (data.Length >= 2)
            {
                this.datFile.AirportBoundary = new AirportBoundary();
                this.datFile.AirportBoundary.Description = data[1];
            }
            else
            {
                log.ErrorFormat("Airport boundary  has {0} data elements, which is too few.", data.Length);
            }
        }

        private void SaveTemporaryNodeCollection()
        {
            if (this.temporaryNodeCollection.Count > 0)
            {
                switch (this.LastHeaderRowCode)
                {
                    case RowCode.AirportBoundary:

                        if (this.datFile.AirportBoundary.Nodes == null)
                        {
                            this.datFile.AirportBoundary.Nodes = new List<Node>();
                        }

                        ((List<Node>)this.datFile.AirportBoundary.Nodes).AddRange(this.temporaryNodeCollection);
                        this.NodeCollectionSecondPass(this.datFile.AirportBoundary.Nodes);

                        break;
                    case RowCode.Pavement:

                        // These nodes must be applicable to the last Pavement in the list
                        if (this.datFile.Pavements != null && this.datFile.Pavements.Count > 0)
                        {
                            var pavement = this.datFile.Pavements[this.datFile.Pavements.Count - 1];

                            if (pavement.Nodes == null)
                            {
                                pavement.Nodes = new List<Node>();
                            }

                            ((List<Node>)pavement.Nodes).AddRange(this.temporaryNodeCollection);
                            this.NodeCollectionSecondPass(pavement.Nodes);
                        }
                        else
                        {
                            log.Error("Have a Node collection but no Pavement to attach to");
                        }

                        break;
                    case RowCode.LinearFeature:
                        //this.NodeCollectionSecondPass(linearFeature.Nodes);
                        break;
                }

            }

            this.temporaryNodeCollection.Clear();
        }

        private void NodeCollectionSecondPass(IList<Node> nodes)
        {
            Node lastNode = null;

            foreach (Node node in nodes)
            {

                lastNode = node;
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
