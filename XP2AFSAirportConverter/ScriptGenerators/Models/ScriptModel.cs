using DotLiquid;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.ScriptGenerators.Models
{
    public class ScriptRunway : Drop
    {
        public double Width { get; set; }
        public double Length { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
        public double Angle { get; set; }
    }

    public class ScriptModel : Drop
    {
        public ScriptModel()
        {
            this.Runways = new List<ScriptRunway>();
        }
        public IList<ScriptRunway> Runways { get; set; }
    }
}
