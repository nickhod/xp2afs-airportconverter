using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.SceneryGateway.Models;

namespace XP2AFSAirportConverter.Common
{
    public class DirectoryHelper
    {
        public static string GetAirportXPFullDirectory(Airport airport, Settings settings)
        {
            var airportFirstLetter = airport.Icao[0];
            var airportFullDirectory = settings.XPlaneXP2AFSConverterFolder + airportFirstLetter + @"\" + airport.Icao;
            return airportFullDirectory;
        }

        public static string GetAirportXPFullDirectory(string icaoCode, Settings settings)
        {
            var airportFirstLetter = icaoCode[0];
            var airportFullDirectory = settings.XPlaneXP2AFSConverterFolder + airportFirstLetter + @"\" + icaoCode;
            return airportFullDirectory;
        }

        public static string GetAirportAFSFullDirectory(string icaoCode, Settings settings)
        {
            var airportFirstLetter = icaoCode[0];
            var airportFullDirectory = settings.AFSXP2AFSConverterFolder + airportFirstLetter + @"\" + icaoCode;
            return airportFullDirectory;
        }

        public static string GetTexturesDirectory(Settings settings)
        {
            var texturesDirectory = settings.XP2AFSConverterFolder + "textures";
            return texturesDirectory;
        }


        public static bool CheckIfXPAirportIsDownloaded(string icaoCode, Settings settings)
        {
            var airportFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(icaoCode, settings);
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

    }
}
