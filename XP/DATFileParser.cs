using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

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
http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT1000-Spec.pdf
http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT850-Spec.pdf
*/

    public class DATFileParser
    {
        // Some types set a header then data of other row types will follow
        private RowCode ParseMode { get; set; }

        private DATFile datFile;

        public DATFile Parse(string data)
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
            if (data.Length == 6)
            {
                var airportHeader = new AirportHeader();

                double elevation;
                double.TryParse(data[1], out elevation);
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


        }

        public Boolean IsNumber(String s)
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
