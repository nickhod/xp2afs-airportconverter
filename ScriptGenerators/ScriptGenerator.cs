using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.ScriptGenerators
{
    public abstract class ScriptGenerator
    {
        public abstract void GenerateScripts(string icao, DATFile datFile, DSFFile dsfFile, string outputFolder);
    }
}
