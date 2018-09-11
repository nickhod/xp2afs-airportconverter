using CsvHelper;
using ICSharpCode.SharpZipLib.Core;
using ICSharpCode.SharpZipLib.Zip;
using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using XP2AFSAirportConverter.AFS;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Converters;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.Processors;
using XP2AFSAirportConverter.ResourceMapping;
using XP2AFSAirportConverter.SceneryGateway;
using XP2AFSAirportConverter.SceneryGateway.Models;
using XP2AFSAirportConverter.ScriptGenerators;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter
{
    public class XP2AFSConverterManager
    {
        public static Settings Settings;

        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");
        private SceneryGatewayApi sceneryGatewayApi;

        private Dictionary<string, AirportListItem> airportLookup;

        private List<ConverterAction> actions;
        private List<string> icaoCodes;

        private DATConverter datConverter;
        private DSFConverter dsfConverter;
        private ResourceMapper resourceMapper;

        private bool overwriteDownloads;

        public XP2AFSConverterManager()
        {
            Settings = new Settings();
            this.datConverter = new DATConverter();
            this.dsfConverter = new DSFConverter();
            this.actions = new List<ConverterAction>();
            this.icaoCodes = new List<string>();
            this.overwriteDownloads = false;
        }

        public void RunActions(string[] args)
        {
            log.Info("------------------------------------------------------------");
            log.Info("Starting XP2AFSAirportConverter");
            log.Info("------------------------------------------------------------");

            this.CreateDirectories();
            this.ReadSettings();

            this.ParseArgs(args);

            this.resourceMapper = new ResourceMapper();
            this.resourceMapper.ReadResourceMapping(Settings.ResourceMapPath);

            this.sceneryGatewayApi = new SceneryGatewayApi();

            foreach (ConverterAction action in this.actions)
            {
                switch (action)
                {
                    case ConverterAction.ConvertAirports:
                        log.Info("Starting Convert Airports action");
                        this.ConvertAirports(icaoCodes);
                        break;
                    case ConverterAction.DownloadAirports:
                        log.Info("Starting Download Airports action");
                        this.DownloadAirports(icaoCodes);
                        break;
                    case ConverterAction.GetAirportList:
                        log.Info("Starting Get Airport List action");
                        this.GetAirportList();
                        break;
                    case ConverterAction.ConvertAirportsStep1:
                        break;
                    case ConverterAction.ConvertAirportsStep2:
                        break;
                    case ConverterAction.CompressAndUploadAirports:
                        //https://github.com/StevenBonePgh/SevenZipSharp
                        //https://github.com/sshnet/SSH.NET
                        break;
                    case ConverterAction.AirportCsvList:
                        this.GenerateAirportCsvList();
                        break;
                    case ConverterAction.ImportAirportCsvList:
                        this.ImportAirportCsvList();
                        break;

                }
            }

        }

        private void ImportAirportCsvList()
        {
            var airportCsvListImportProcessor = new AirportCsvListImportProcessor();
            var csvFile = XP2AFSConverterManager.Settings.XP2AFSConverterFolder + @"airports.csv";
            airportCsvListImportProcessor.ImportAirportCsvList(csvFile);
        }

        private void ParseArgs(string[] args)
        {
            if (args.Length > 0)
            {
                switch (args[0])
                {
                    case "getlist":
                        this.actions.Add(ConverterAction.GetAirportList);
                        break;
                    case "download":
                        this.actions.Add(ConverterAction.DownloadAirports);
                        break;
                    case "convert":
                        this.actions.Add(ConverterAction.ConvertAirports);
                        break;
                    case "airportcsvlist":
                        this.actions.Add(ConverterAction.AirportCsvList);
                        break;
                    case "importairportcsvlist":
                        this.actions.Add(ConverterAction.ImportAirportCsvList);
                        break;
                }

                if (args.Length > 1)
                {
                    var icaoCodesTemp = args[1].Split(',');
                    foreach (var icaoCode in icaoCodesTemp)
                    {
                        if (icaoCode.ToLower() != "all")
                        {
                            this.icaoCodes.Add(icaoCode.ToUpper());
                        }
                    }

                    if (args.Length > 2)
                    {
                        if (args[2] == "overwrite")
                        {
                            this.overwriteDownloads = true;
                        }
                    }
                }
            }
        }

        private void CreateDirectories()
        {
            var documentsFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

            var xp2AFSConverterFolder = documentsFolderPath + @"\XP2AFSConverter\";
            var xplaneXP2AFSConverterFolder = documentsFolderPath + @"\XP2AFSConverter\xp\";
            var afsXP2AFSConverterFolder = documentsFolderPath + @"\XP2AFSConverter\afs\";
            var tempFolder = documentsFolderPath + @"\XP2AFSConverter\temp\";

            if (!Directory.Exists(xp2AFSConverterFolder))
            {
                Directory.CreateDirectory(xp2AFSConverterFolder);
            }

            if (!Directory.Exists(xplaneXP2AFSConverterFolder))
            {
                Directory.CreateDirectory(xplaneXP2AFSConverterFolder);
            }

            if (!Directory.Exists(afsXP2AFSConverterFolder))
            {
                Directory.CreateDirectory(afsXP2AFSConverterFolder);
            }

            if (!Directory.Exists(tempFolder))
            {
                Directory.CreateDirectory(tempFolder);
            }

            Settings.XP2AFSConverterFolder = xp2AFSConverterFolder;
            Settings.XPlaneXP2AFSConverterFolder = xplaneXP2AFSConverterFolder;
            Settings.AFSXP2AFSConverterFolder = afsXP2AFSConverterFolder;
            Settings.TempFolder = tempFolder;

        }

        private void ReadSettings()
        {
            string xpToolsPathRaw = System.Configuration.ConfigurationManager.AppSettings["XPToolsPath"];
            string xpToolsPath = xpToolsPathRaw;

            if (xpToolsPathRaw.Contains("{MyDocuments}"))
            {
                string documentsFolder = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
                xpToolsPath = xpToolsPathRaw.Replace("{MyDocuments}", documentsFolder);
            }

            Settings.XPToolsPath = xpToolsPath;

            // Currently doesn't support absolute paths
            string resourceMapPath = System.Configuration.ConfigurationManager.AppSettings["ResourceMap"];
            resourceMapPath = AppDomain.CurrentDomain.BaseDirectory + resourceMapPath;
            Settings.ResourceMapPath = resourceMapPath;

        }

        private void GetAirportList()
        {
            log.Info("Getting list of all airports");
            var airportList = this.sceneryGatewayApi.GetAllAirports();

            // Clean up the airport name so XML deserialization doesn't fail
            foreach (AirportListItem airport in airportList.Airports)
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                airport.AirportName = HttpUtility.HtmlDecode(airport.AirportName);
                airport.AirportName = airport.AirportName.Replace(@"&#x13; ", "");
                airport.AirportName = airport.AirportName.Replace(@"&#xC;", "");
                airport.AirportName = Regex.Replace(airport.AirportName, @"&#x?[^;]{1,6};", string.Empty);
                airport.AirportName = textInfo.ToTitleCase(airport.AirportName);
            }

            log.Info("Writing list of all airports to file");
            using (MemoryStream ms = new MemoryStream())
            {
                var writer = new StreamWriter(ms, Encoding.UTF8);
                var serializer = new XmlSerializer(typeof(AirportList));
                serializer.Serialize(writer, airportList);
                writer.Flush();

                //if the serialization succeed, rewrite the file.
                File.WriteAllBytes(Settings.XP2AFSConverterFolder + @"xp_airports.xml", ms.ToArray());
            }
        }

        private void DownloadAirports(List<string> icaoCodes)
        {
            if (icaoCodes == null || icaoCodes.Count < 1)
            {
                if (icaoCodes == null)
                {
                    icaoCodes = new List<string>();
                }

                log.Info("No airports specified so downloading all airports");

                var airportsFile = Settings.XP2AFSConverterFolder + @"xp_airports.xml";

                var airportList = this.DeserializeAirportsFile(airportsFile);
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

        private bool CheckIfXPAirportIsDownloaded(string icaoCode)
        {
            var airportFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(icaoCode, Settings);
            var airportZipFilename = airportFullDirectory + @"\" + icaoCode + ".zip";
            var airportFilename = airportFullDirectory + @"\airport.xml";
            var airportSceneryFilename = airportFullDirectory + @"\scenery.xml";

            bool airportExists = true;

            if (!File.Exists(airportZipFilename))
            {
                airportExists = false;
            }

            if (!File.Exists(airportFilename))
            {
                airportExists = false;
            }

            if (!File.Exists(airportSceneryFilename))
            {
                airportExists = false;
            }

            return airportExists;
        }

        private void DownloadAirport(string icaoCode)
        {
            bool download = true;

            // If we don't want to overwrite download and the airport already exists, don't download
            if (!this.overwriteDownloads)
            {
                if (this.CheckIfXPAirportIsDownloaded(icaoCode))
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
                                var airportFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(airport, Settings);
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

        private void ConvertAirports(List<string> icaoCodes)
        {
            int i = 0;
            foreach (string icaoCode in icaoCodes)
            {
                log.Info("------------------------------------------------------------");
                log.InfoFormat("Converting airport {0} of {1}", i + 1, icaoCodes.Count());

                this.ConvertAirport(icaoCode);
            }
        }

        private void ConvertAirport(string icaoCode)
        {
            log.InfoFormat("Converting {0}", icaoCode);

            var airportXPFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(icaoCode, Settings);
            var airportZipFilename = airportXPFullDirectory + @"\" + icaoCode + ".zip";
            var airportFilename = airportXPFullDirectory + @"\airport.xml";
            var airportSceneryFilename = airportXPFullDirectory + @"\scenery.xml";

            var airportAFSFullDirectory = DirectoryHelper.GetAirportAFSFullDirectory(icaoCode, Settings);

            var tscFilename = airportAFSFullDirectory + @"\" + icaoCode + ".tsc";
            var tocFilename = airportAFSFullDirectory + @"\" + icaoCode + ".toc";
            var tmcFilename = airportAFSFullDirectory + @"\" + icaoCode + ".tmc";

            if (File.Exists(airportZipFilename))
            {
                // Parse the DST and DAT files
                var datFileLoader = new DATFileLoader();
                var dsfFileLoader = new DSFFileLoader();
                var datFile = datFileLoader.GetDATFileFromXPZip(icaoCode, airportZipFilename);
                var dsfFile = dsfFileLoader.GetDSFFileFromXPZip(icaoCode, airportZipFilename);

                // Create empty AFS files
                var tscFile = new TSCFile();
                var tocFile = new TOCFile();
                var tmcFile = new TMCFile();

                // Convert the X-Plane files
                this.datConverter.Convert(datFile, tscFile, tocFile);
                this.dsfConverter.Convert(dsfFile, tscFile, tocFile);

                // Get strings of the AFS files
                var tscFileString = tscFile.ToString();
                var tocFileString = tocFile.ToString();
                var tmcFileString = tmcFile.ToString();

                if (!Directory.Exists(airportAFSFullDirectory))
                {
                    Directory.CreateDirectory(airportAFSFullDirectory);
                }

                // Write the AFS files
                File.WriteAllText(tscFilename, tscFileString);
                File.WriteAllText(tocFilename, tocFileString);
                File.WriteAllText(tmcFilename, tmcFileString);

                var maxScriptGenerator = new MaxScriptGenerator();
                maxScriptGenerator.GenerateScripts(icaoCode, datFile, dsfFile, tscFile, airportAFSFullDirectory);

                log.Info("Airport conversion done");
            }
            else
            {
                log.ErrorFormat("Could not find the data for airport {0} make sure it is downloaded", icaoCode);
            }
        }

        private void GenerateAirportCsvList()
        {
            log.Info("Generating Airport CSV File");

            AirportCsvListProcessor csvListProcessor = new AirportCsvListProcessor();

            var airportsFile = Settings.XP2AFSConverterFolder + @"xp_airports.xml";
            var airportList = this.DeserializeAirportsFile(airportsFile);

            csvListProcessor.GenerateAirportCsvList(airportList);
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

        private AirportList DeserializeAirportsFile(string airportsFile)
        {
            AirportList airportList = null;

            if (File.Exists(airportsFile))
            {
                log.Info("Loading airports from file");

                // Annoying special characters issue.
                // Not sure why these don't work given that we're using UTF8 to encode and decode
                var airportFileString = File.ReadAllText(airportsFile);
                airportFileString = Regex.Replace(airportFileString, @"&#x?[^;]{1,6};", string.Empty);

                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(airportFileString)))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(AirportList));
                    StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);
                    airportList = (AirportList)xmlSerializer.Deserialize(reader);
                }
            }
            else
            {
                log.Info("Airport file not found");
            }

            return airportList;
        }



    }
}