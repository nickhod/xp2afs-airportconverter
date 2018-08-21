using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.AFS;
using XP2AFSAirportConverter.Models;

namespace XP2AFSAirportConverter.Converters
{
    public class DSFConverter
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        private DSFFile dsfFile;

        public void Convert(DSFFile dsfFile, TSCFile tscFile, TOCFile tocFile)
        {
            this.dsfFile = dsfFile;
        }
    }
}
