using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Models;

namespace XP2AFSAirportConverter.XP
{
    public class DSFTextFileParser
    {
        private DSFFile dsfFile;
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");


        public DSFFile ParseFromString(string data)
        {
            this.dsfFile = new DSFFile();

            return this.dsfFile;
        }
    }
}
