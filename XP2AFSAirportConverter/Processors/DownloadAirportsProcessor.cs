using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.SceneryGateway;
using XP2AFSAirportConverter.SceneryGateway.Models;

namespace XP2AFSAirportConverter.Processors
{
    public class DownloadAirportsProcessor
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");
        private SceneryGatewayApi sceneryGatewayApi;
        private Dictionary<string, AirportListItem> airportLookup;
        private bool overwriteDownloads;

        public DownloadAirportsProcessor()
        {
            sceneryGatewayApi = new SceneryGatewayApi();
            airportLookup = new Dictionary<string, AirportListItem>();
        }

        public void DownloadAirports(List<string> icaoCodes, bool overwriteDownloads)
        {
            AirportListProcessor airportListProcessor = new AirportListProcessor();

            this.overwriteDownloads = overwriteDownloads;

            if (icaoCodes == null || icaoCodes.Count < 1)
            {
                if (icaoCodes == null)
                {
                    icaoCodes = new List<string>();
                }

                log.Info("No airports specified so downloading all airports");

                var airportListFile = XP2AFSConverterManager.Settings.XP2AFSConverterFolder + @"xp_airports.xml";

                var airportList = airportListProcessor.DeserializeAirportListFile(airportListFile);
                this.airportLookup = new Dictionary<string, AirportListItem>();

                foreach (AirportListItem airport in airportList.Airports)
                {
                    icaoCodes.Add(airport.AirportCode);
                    this.airportLookup.Add(airport.AirportCode, airport);
                }

            }

            int i = 0;

            foreach (string icaoCode in icaoCodes)
            {
                log.Info("------------------------------------------------------------");
                log.InfoFormat("Downloading airport {0} of {1}", i + 1, icaoCodes.Count());
                this.DownloadAirport(icaoCode);
                i++;
            }
        }

        private void DownloadAirport(string icaoCode)
        {
            bool download = true;

            // If we don't want to overwrite download and the airport already exists, don't download
            if (!this.overwriteDownloads)
            {
                if (DirectoryHelper.CheckIfXPAirportIsDownloaded(icaoCode, XP2AFSConverterManager.Settings))
                {
                    download = false;
                    log.InfoFormat("Skipping airport {0}, already downloaded", icaoCode);
                }
            }

            if (download)
            {
                log.InfoFormat("Downloading airport {0}", icaoCode);

                try
                {
                    log.Info("Getting airport info");
                    var airport = this.sceneryGatewayApi.GetAirport(icaoCode);

                    if (airport != null)
                    {
                        log.Info("Getting scenery");

                        if (airport.RecommendedSceneryId.HasValue)
                        {
                            var scenery = this.sceneryGatewayApi.GetScenery(airport.RecommendedSceneryId.Value);

                            if (scenery != null && airport.AirportName.Length > 0)
                            {
                                var airportFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(airport, XP2AFSConverterManager.Settings);
                                var airportZipFilename = airportFullDirectory + @"\" + airport.Icao + ".zip";
                                var airportFilename = airportFullDirectory + @"\airport.xml";
                                var airportSceneryFilename = airportFullDirectory + @"\scenery.xml";

                                if (!Directory.Exists(airportFullDirectory))
                                {
                                    Directory.CreateDirectory(airportFullDirectory);
                                }

                                File.WriteAllBytes(airportZipFilename, Convert.FromBase64String(scenery.MasterZipBlob));

                                this.SerializeAirport(airport, airportFilename);

                                scenery.MasterZipBlob = "";
                                this.SerializeScenery(scenery, airportSceneryFilename);
                            }
                        }


                    }
                }

                catch (Exception)
                {
                    log.ErrorFormat("Failed to download airport {0}", icaoCode);
                }

                Random rnd = new Random();
                var wait = rnd.Next(500, 2000);
                System.Threading.Thread.Sleep(wait);
            }


        }

        private void SerializeAirport(Airport airport, string filename)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var writer = new StreamWriter(ms, Encoding.UTF8);
                var serializer = new XmlSerializer(typeof(Airport));
                serializer.Serialize(writer, airport);
                writer.Flush();

                //if the serialization succeed, rewrite the file.
                File.WriteAllBytes(filename, ms.ToArray());
            }
        }

        private void SerializeScenery(Scenery scenery, string filename)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                var writer = new StreamWriter(ms, Encoding.UTF8);
                var serializer = new XmlSerializer(typeof(Scenery));
                serializer.Serialize(writer, scenery);
                writer.Flush();

                //if the serialization succeed, rewrite the file.
                File.WriteAllBytes(filename, ms.ToArray());
            }
        }




    }
}
