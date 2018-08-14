using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.Converters
{
    public class DATToTSCConverter
    {
        public TSCFile Convert(DATFile datFile)
        {
            var tscFile = new TSCFile();
            tscFile.ICAO = datFile.AirportHeader.ICAOCode;
            return tscFile;
        }
    }
}
