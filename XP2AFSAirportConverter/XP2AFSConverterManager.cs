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
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Converters;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.SceneryGateway;
using XP2AFSAirportConverter.SceneryGateway.Models;
using XP2AFSAirportConverter.ScriptGenerators;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter
{
    public class XP2AFSConverterManager
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");
        private SceneryGatewayApi sceneryGatewayApi;
        private Settings settings;

        private Dictionary<string, AirportListItem> airportLookup;

        private List<ConverterAction> actions;
        private List<string> icaoCodes;

        private DATToTSCConverter datToTSCConverter;

        public XP2AFSConverterManager()
        {
            this.settings = new Settings();
            this.datToTSCConverter = new DATToTSCConverter();
            this.actions = new List<ConverterAction>();
            this.icaoCodes = new List<string>();
        }

        public void RunActions(string[] args)
        {
            log.Info("------------------------------------------------------------");
            log.Info("Starting XP2AFSAirportConverter");
            log.Info("------------------------------------------------------------");

            this.CreateDirectories();
            this.ReadSettings();

            this.ParseArgs(args);

            this.sceneryGatewayApi = new SceneryGatewayApi();

            foreach (ConverterAction action in this.actions)
            {
                switch(action)
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
                      
                }
            }

        }

        private void ParseArgs(string[] args)
        {
            if (args.Length > 0)
            {
                switch(args[0])
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

            this.settings.XP2AFSConverterFolder = xp2AFSConverterFolder;
            this.settings.XPlaneXP2AFSConverterFolder = xplaneXP2AFSConverterFolder;
            this.settings.AFSXP2AFSConverterFolder = afsXP2AFSConverterFolder;
            this.settings.TempFolder = tempFolder;

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

            settings.XPToolsPath = xpToolsPath;
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
                File.WriteAllBytes(this.settings.XP2AFSConverterFolder + @"xp_airports.xml", ms.ToArray());
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


                var airportsFile = this.settings.XP2AFSConverterFolder + @"xp_airports.xml";
                if (File.Exists(airportsFile))
                {
                    log.Info("Loading airports from file");

                    var airportList = new AirportList();
                    this.airportLookup = new Dictionary<string, AirportListItem>();

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

                    foreach (AirportListItem airport in airportList.Airports)
                    {
                        icaoCodes.Add(airport.AirportCode);
                        this.airportLookup.Add(airport.AirportCode, airport);
                    }
                }
                else
                {
                    log.Info("Airport file not found");
                }

            }

            int i = 0;

            foreach (string icaoCode in icaoCodes)
            {
                log.Info("------------------------------------------------------------");
                log.InfoFormat("Downloading airport {0} of {1}", i + 1, icaoCodes.Count());
                this.DownloadAirport(icaoCode);

                Random rnd = new Random();
                var wait = rnd.Next(500, 2000);
                System.Threading.Thread.Sleep(wait);
                i++;

                if (i == 1000)
                {
                    break;
                }
            }
        }

        private void DownloadAirport(string icaoCode)
        {
            log.InfoFormat("Downloading airport {0}", icaoCode);

            log.Info("Getting airport info");

            try
            {
                var airport = this.sceneryGatewayApi.GetAirport(icaoCode);

                if (airport != null)
                {
                    log.Info("Getting scenery");

                    if (airport.RecommendedSceneryId.HasValue)
                    {
                        var scenery = this.sceneryGatewayApi.GetScenery(airport.RecommendedSceneryId.Value);

                        if (scenery != null && airport.AirportName.Length > 0)
                        {
                            var airportFullDirectory = GetAirportXPFullDirectory(airport);
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

            var airportXPFullDirectory = this.GetAirportXPFullDirectory(icaoCode);
            var airportZipFilename = airportXPFullDirectory + @"\" + icaoCode + ".zip";
            var airportFilename = airportXPFullDirectory + @"\airport.xml";
            var airportSceneryFilename = airportXPFullDirectory + @"\scenery.xml";

            var airportAFSFullDirectory = this.GetAirportAFSFullDirectory(icaoCode);
            var tmcFilename = airportAFSFullDirectory + @"\" + icaoCode + ".tmc";

            if (File.Exists(airportZipFilename))
            {
                var datFile = this.GetDATFileFromXPZip(icaoCode, airportZipFilename);
                var dsfFile = this.GetDSFFileFromXPZip(icaoCode, airportZipFilename);

                var tscFile = this.datToTSCConverter.Convert(datFile);
                var tscFileString = tscFile.ToString();

                if (!Directory.Exists(airportAFSFullDirectory))
                {
                    Directory.CreateDirectory(airportAFSFullDirectory);
                }

                File.WriteAllText(tmcFilename, tscFileString);

                var maxScriptGenerator = new MaxScriptGenerator();
                maxScriptGenerator.GenerateScripts(icaoCode, datFile, dsfFile, tscFile, airportAFSFullDirectory);
            }
            else
            {
                log.ErrorFormat("Could not find the data for airport {0} make sure it is downloaded", icaoCode);
            }
        }

        private string GetAirportXPFullDirectory(Airport airport)
        {
            var airportFirstLetter = airport.Icao[0];
            var airportFullDirectory = this.settings.XPlaneXP2AFSConverterFolder + airportFirstLetter + @"\" + airport.Icao;
            return airportFullDirectory;
        }

        private string GetAirportXPFullDirectory(string icaoCode)
        {
            var airportFirstLetter = icaoCode[0];
            var airportFullDirectory = this.settings.XPlaneXP2AFSConverterFolder + airportFirstLetter + @"\" + icaoCode;
            return airportFullDirectory;
        }

        private string GetAirportAFSFullDirectory(string icaoCode)
        {
            var airportFirstLetter = icaoCode[0];
            var airportFullDirectory = this.settings.AFSXP2AFSConverterFolder + airportFirstLetter + @"\" + icaoCode;
            return airportFullDirectory;
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

        private DATFile GetDATFileFromXPZip(string icaoCode, string xpZipFilename)
        {
            string datFileData = "";

            using (var fileStream = new FileStream(xpZipFilename, FileMode.Open, FileAccess.Read))
            {
                using (var zipFile = new ZipFile(fileStream))
                {
                    var zipEntry = zipFile.GetEntry(icaoCode + ".dat");
                    if (zipEntry == null)
                    {
                        log.ErrorFormat("{0}.dat not found in zip", icaoCode);
                    }

                    using (var stream = zipFile.GetInputStream(zipEntry))
                    {
                        using (var reader = new StreamReader(stream))
                        {
                            datFileData = reader.ReadToEnd();
                        }
                    }
                }
            }

            var datFileParser = new DATFileParser();
            var datFile = datFileParser.ParseFromString(datFileData);
            return datFile;
        }

        private DSFFile GetDSFFileFromXPZip(string icaoCode, string xpZipFilename)
        {
            // Fun and games, the DSF file is in the hierarchy
            // ICAO.zip
            //   ICAO_Scenery_Pack.zip
            //     ICAO_Scenery_Pack (dir)
            //       Earth nav data
            //         -60-080 (this is not fixed)
            //           -52-073.dsf (this is not fixed

            byte[] dsfFileData = null;

            using (var fileStream = new FileStream(xpZipFilename, FileMode.Open, FileAccess.Read))
            {
                using (var zipFile = new ZipFile(fileStream))
                {
                    var innerZipName = String.Format("{0}_Scenery_Pack.zip", icaoCode);
                    var zipEntry = zipFile.GetEntry(innerZipName);
                    if (zipEntry == null)
                    {
                        log.ErrorFormat("{0} not found in zip", innerZipName);
                    }


                    using (var stream = zipFile.GetInputStream(zipEntry))
                    {
                        // The stream above is not seekable. We need to copy it to a memory
                        // stream first, then we can instantiate another ZipFile object from
                        // the memory stream
                        using (var memoryStream = new MemoryStream())
                        {
                            stream.CopyTo(memoryStream);
                            memoryStream.Position = 0;

                            using (var innerZipFile = new ZipFile(memoryStream))
                            {
                                foreach (ZipEntry innerZipEntry in innerZipFile)
                                {
                                    if (zipEntry.IsDirectory)
                                        continue;

                                    //log.Debug(innerZipEntry.Name);

                                    if (innerZipEntry.Name.Contains(".dsf"))
                                    {
                                        using (Stream dsfFileStream = innerZipFile.GetInputStream(innerZipEntry))
                                        {
                                            // Get the bytes of the dsf file
                                            using (MemoryStream dsfFileMemoryStream = new MemoryStream())
                                            {
                                                byte[] buffer = new byte[4096];
                                                StreamUtils.Copy(dsfFileStream, dsfFileMemoryStream, buffer);
                                                dsfFileData = dsfFileMemoryStream.ToArray();
                                            }
  
                                        }
                                    }


                                }
                            }
                        }
                    }

                }
            }

            var dsf2TextManager = new DSF2TextManager();
            var dsfAsText = dsf2TextManager.GetTextDSFFile(dsfFileData, settings);

            var dsfTextFileParser = new DSFTextFileParser();
            var dsfFile = dsfTextFileParser.ParseFromString(dsfAsText);

            dsfAsText = null;
            dsfFileData = null;
            return dsfFile;
        }
    }
}