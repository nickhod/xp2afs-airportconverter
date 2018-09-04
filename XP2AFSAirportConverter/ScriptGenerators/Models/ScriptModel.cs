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

    public class ScriptPavement : Drop
    {
        public IList<ScriptNode> Nodes { get; set; }
        public string Name { get; set; }
    }

    public class ScriptNode : Drop
    {
        public double X { get; set; }
        public double Y { get; set; }

        public double BezierControlX { get; set; }
        public double BezierControlY { get; set; }
        public bool End { get; set; }
        public bool CloseLoop { get; set; }
        public bool OpenLoop { get; set; }
        public bool IsBezier { get; set; }
    }

    public class ScriptModel : Drop
    {
        public ScriptModel()
        {
            this.Runways = new List<ScriptRunway>();
            this.Pavements = new List<ScriptPavement>();
        }

        public IList<ScriptPavement> Pavements { get; set; }
        public IList<ScriptRunway> Runways { get; set; }
        public string AirportName { get; set; }
        public string AirportICAO { get; set; }
        public DateTime GeneratedOn { get; set; }
    }
}
