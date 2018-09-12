using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.AFS;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Converters;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.ScriptGenerators;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.Processors
{
    public class GenerateRenderScriptsProcessor
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");
        private DATConverter datConverter;
        private DSFConverter dsfConverter;

        public GenerateRenderScriptsProcessor()
        {
            this.datConverter = new DATConverter();
            this.dsfConverter = new DSFConverter();
        }

        public void ConvertAirports(List<string> icaoCodes)
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

            var airportXPFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(icaoCode, XP2AFSConverterManager.Settings);
            var airportZipFilename = airportXPFullDirectory + @"\" + icaoCode + ".zip";
            var airportFilename = airportXPFullDirectory + @"\airport.xml";
            var airportSceneryFilename = airportXPFullDirectory + @"\scenery.xml";

            var airportAFSFullDirectory = DirectoryHelper.GetAirportAFSFullDirectory(icaoCode, XP2AFSConverterManager.Settings);

            // Make sure the 'input' folder exists
            if (!Directory.Exists(airportAFSFullDirectory + @"\Input"))
            {
                Directory.CreateDirectory(airportAFSFullDirectory + @"\Input");
            }

            var tscFilename = airportAFSFullDirectory + @"\Input\" + icaoCode + ".tsc";
            var tocFilename = airportAFSFullDirectory + @"\Input\" + icaoCode + ".toc";
            var tmcFilename = airportAFSFullDirectory + @"\Input\" + icaoCode + ".tmc";

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
    }
}
