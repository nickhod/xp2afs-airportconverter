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

        private List<ConverterAction> actions;
        private List<string> icaoCodes;
        private ResourceMapper resourceMapper;
        private bool overwriteDownloads;


        public XP2AFSConverterManager()
        {
            Settings = new Settings();
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

            foreach (ConverterAction action in this.actions)
            {
                switch (action)
                {
                    case ConverterAction.DownloadAirports:
                        log.Info("Starting Download Airports action");
                        this.DownloadAirports(icaoCodes);
                        log.Info("Finished Download Airports action");
                        break;
                    case ConverterAction.GetAirportList:
                        log.Info("Starting Get Airport List action");
                        this.GetAirportList();
                        log.Info("Finished Get Airport List action");
                        break;
                    case ConverterAction.GenerateRenderScripts:
                        log.Info("Starting Generate Render Scripts action");
                        this.GenerateRenderScripts(icaoCodes);
                        log.Info("Finished Generate Render Scripts action");
                        break;
                    case ConverterAction.RunRenderScripts:
                        log.Info("Starting Run Render Scripts action");
                        this.RunRenderScripts(icaoCodes);
                        log.Info("Finished Run Render Scripts action");
                        break;
                    case ConverterAction.BuildAirports:
                        log.Info("Starting Build Airports action");
                        this.BuildAirports();
                        log.Info("Finished Build Airports action");
                        break;
                    case ConverterAction.UploadAirports:
                        log.Info("Starting Uplooad Airports action");
                        this.UploadAirports();
                        log.Info("Finished Uplooad Airports action");
                        break;
                    case ConverterAction.AirportCsvList:
                        log.Info("Starting Airport CSV List action");
                        this.GenerateAirportCsvList();
                        log.Info("Finished Airport CSV List action");
                        break;
                    case ConverterAction.ImportAirportCsvList:
                        log.Info("Starting Import Airport CSV List action");
                        this.ImportAirportCsvList();
                        log.Info("Finished Import Airport CSV List action");
                        break;

                }
            }

            log.Info("All actions complete");

        }


        private void DownloadAirports(List<string> icaoCodes)
        {
            var downloadAirportProcessor = new DownloadAirportsProcessor();
            downloadAirportProcessor.DownloadAirports(icaoCodes, overwriteDownloads);
        }

        private void GetAirportList()
        {
            var airportListProcessor = new AirportListProcessor();
            airportListProcessor.GetAirportList();
        }

        private void GenerateRenderScripts(List<string> icaoCodes)
        {
            var generateRenderScriptProcessor = new GenerateRenderScriptsProcessor();
            generateRenderScriptProcessor.ConvertAirports(icaoCodes);
        }

        private void RunRenderScripts(List<string> icaoCodes)
        {
            var runRenderScriptProcessor = new RunRenderScriptsProcessor();
            runRenderScriptProcessor.RunRenderScripts(icaoCodes);
        }

        private void BuildAirports()
        {
            var buildAirportsProcessor = new BuildAirportsProcessor();
            buildAirportsProcessor.BuildAirports(icaoCodes);
        }

        private void UploadAirports()
        {
            var uploadAirportsProcessor = new UploadAirportsProcessor();
            uploadAirportsProcessor.UploadAirports(icaoCodes);
        }

        private void GenerateAirportCsvList()
        {
            log.Info("Generating Airport CSV File");

            AirportListProcessor airportListProcessor = new AirportListProcessor();
            AirportCsvListProcessor csvListProcessor = new AirportCsvListProcessor();

            var airportsFile = Settings.XP2AFSConverterFolder + @"xp_airports.xml";
            var airportList = airportListProcessor.DeserializeAirportListFile(airportsFile);

            csvListProcessor.GenerateAirportCsvList(airportList);
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
                    case "generaterenderscripts":
                        this.actions.Add(ConverterAction.GenerateRenderScripts);
                        break;
                    case "buildairports":
                        this.actions.Add(ConverterAction.BuildAirports);
                        break;
                    case "uploadairports":
                        this.actions.Add(ConverterAction.UploadAirports);
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
                    // It's a path to a file of ICAO codes
                    if (args[1].Contains(".txt"))
                    {
                        string filePath = args[1];

                        if (File.Exists(filePath))
                        {
                            var icaoCodesArray = File.ReadAllLines(args[1]);
                            this.icaoCodes = icaoCodes.ToList();
                        }
                        else
                        {
                            log.Error("Airport code list file given but file does not exist");
                        }

                    }
                    // It's a csv list of ICAO codes
                    else
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

    }
}