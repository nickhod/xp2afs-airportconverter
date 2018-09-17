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
        public int Index { get; set; }
        public string SurfaceType { get; set; }
        public string ShoulderType { get; set; }
    }

    public class ScriptPavement : Drop
    {
        public IList<ScriptNode> Nodes { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
    }

    public class ScriptBuilding : Drop
    {
        public IList<ScriptNode> Nodes { get; set; }
        public double Height { get; set; }
        public string Name { get; set; }
        public int Index { get; set; }
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
        public bool Render { get; set; }
    }

    public class ScriptModel : Drop
    {
        public ScriptModel()
        {
            this.Runways = new List<ScriptRunway>();
            this.DATPavements = new List<ScriptPavement>();
            this.DSFPavements = new List<ScriptPavement>();
            this.Buildings = new List<ScriptBuilding>();
        }

        public IList<ScriptPavement> DATPavements { get; set; }
        public IList<ScriptPavement> DSFPavements { get; set; }
        public IList<ScriptRunway> Runways { get; set; }
        public IList<ScriptBuilding> Buildings { get; set; }
        public string AirportName { get; set; }
        public string AirportICAO { get; set; }
        public DateTime GeneratedOn { get; set; }
        public string SaveFilePath { get; set; }
        public string TexturesPath { get; set; }
    }
}
