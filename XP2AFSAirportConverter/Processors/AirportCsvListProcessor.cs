using CsvHelper;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XP2AFSAirportConverter.AFS;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Converters;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.SceneryGateway.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.Processors
{
    public class AirportCsvListProcessor
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        public void GenerateAirportCsvList(AirportList airportList)
        {
            var csvFile = XP2AFSConverterManager.Settings.XP2AFSConverterFolder + @"airports.csv";
            var writer = new StreamWriter(csvFile);
            var csv = new CsvWriter(writer);
            csv.WriteHeader<AirportCsvListItem>();
            csv.NextRecord();

            int i = 0;
            foreach (var airport in airportList.Airports)
            {
                // Exclude sea runways, closed runways and helipads
                if (!airport.AirportName.StartsWith("[S]") &&
                    !airport.AirportName.StartsWith("[X]") &&
                    !airport.AirportName.StartsWith("[H]"))
                {

                    var airportXPFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(airport.AirportCode, XP2AFSConverterManager.Settings);
                    var airportZipFilename = airportXPFullDirectory + @"\" + airport.AirportCode + ".zip";
                    var airportFilename = airportXPFullDirectory + @"\airport.xml";
                    var airportSceneryFilename = airportXPFullDirectory + @"\scenery.xml";

                    if (File.Exists(airportZipFilename))
                    {
                        //if (airport.AirportCode == "")

                        log.InfoFormat("Adding airport {0} of {1}", i + 1, airportList.Airports.Count);
                        // Parse the DST and DAT files
                        var datFileLoader = new DATFileLoader();
                        var datFile = datFileLoader.GetDATFileFromXPZip(airport.AirportCode, airportZipFilename);
                        //var dsfFile = this.GetDSFFileFromXPZip(airport.AirportCode, airportZipFilename);
                        var sceneryFile = DeserializeSceneryFile(airportSceneryFilename);

                        // Create empty AFS files
                        var tscFile = new TSCFile();
                        var tocFile = new TOCFile();

                        var datConverter = new DATConverter();
                        // Convert the X-Plane files
                        datConverter.Convert(datFile, tscFile, tocFile);

                        if (datFile.AirportHeader.AirportType == AirportType.Airport && datFile.LandRunways != null)
                        {
                            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                            AirportCsvListItem airportListItem = new AirportCsvListItem();
                            airportListItem.Code = airport.AirportCode;
                            airportListItem.Name = textInfo.ToTitleCase(airport.AirportName.ToLower());
                            airportListItem.IsHelipad = false;

                            if (datFile.MetadataLookup.ContainsKey("datum_lat"))
                            {
                                double lat;
                                if (Double.TryParse(datFile.MetadataLookup["datum_lat"], out lat))
                                {
                                    airportListItem.Latitude = lat;
                                }
                            }

                            if (datFile.MetadataLookup.ContainsKey("datum_lon"))
                            {
                                double lon;
                                if (Double.TryParse(datFile.MetadataLookup["datum_lon"], out lon))
                                {
                                    airportListItem.Longitude = lon;
                                }
                            }

                            if (datFile.MetadataLookup.ContainsKey("country"))
                            {
                                airportListItem.Country = datFile.MetadataLookup["country"];
                            }

                            if (datFile.MetadataLookup.ContainsKey("state"))
                            {
                                airportListItem.Region = datFile.MetadataLookup["state"];
                            }

                            if (datFile.MetadataLookup.ContainsKey("city"))
                            {
                                airportListItem.Region = datFile.MetadataLookup["city"];
                            }

                            if (datFile.MetadataLookup.ContainsKey("region_code"))
                            {
                                airportListItem.RegionCode = datFile.MetadataLookup["region_code"];
                            }

                            if (datFile.MetadataLookup.ContainsKey("icao_code"))
                            {
                                airportListItem.ICAO = datFile.MetadataLookup["icao_code"];
                            }

                            if (datFile.MetadataLookup.ContainsKey("iata_code"))
                            {
                                airportListItem.IATA = datFile.MetadataLookup["iata_code"];
                            }

                            if (datFile.MetadataLookup.ContainsKey("faa_code"))
                            {
                                airportListItem.FAACode = datFile.MetadataLookup["faa_code"];
                            }

                            if (datFile.Pavements == null || datFile.Pavements.Count == 0)
                            {
                                airportListItem.HasPavements = false;
                            }
                            else
                            {
                                airportListItem.HasPavements = true;
                            }

                            if (sceneryFile.Type == "3D")
                            {
                                airportListItem.Is3D = true;
                            }
                            else
                            {
                                airportListItem.Is3D = false;
                            }

                            foreach (var runway in datFile.LandRunways)
                            {
                                airportListItem.RunwaySurfaceType = (int)runway.SurfaceType;
                            }

                            airportListItem.RunwayCount = datFile.LandRunways.Count;

                            // If we couldn't get a value from the metadata we need to get it elsewhere
                            if (!airportListItem.Latitude.HasValue || !airportListItem.Longitude.HasValue)
                            {
                                airportListItem.Latitude = tscFile.Location.Latitude;
                                airportListItem.Longitude = tscFile.Location.Longitude;
                            }

                            csv.WriteRecord(airportListItem);
                            csv.NextRecord();

                        }

                    }
                    else
                    {
                        log.ErrorFormat("Could not find the data for airport {0} make sure it is downloaded", airport.AirportCode);
                    }


                }

                i++;

            }

            writer.Flush();
            writer.Close();
            writer = null;

        }

        private Scenery DeserializeSceneryFile(string sceneryFile)
        {
            Scenery scenery = null;

            if (File.Exists(sceneryFile))
            {
                var sceneryFileString = File.ReadAllText(sceneryFile);

                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(sceneryFileString)))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(Scenery));
                    StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);
                    scenery = (Scenery)xmlSerializer.Deserialize(reader);
                }
            }
            else
            {
                log.Info("Scenery file not found");
            }

            return scenery;
        }

    }
}
