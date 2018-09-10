using System;
using System.Collections.Generic;
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
    }
}
