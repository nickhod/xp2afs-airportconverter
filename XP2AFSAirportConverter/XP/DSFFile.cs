using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Models
{
    public class DSFObject
    {
        public DSFObject()
        {
            this.Positions = new List<DSFObjectPosition>();
        }

        public string Reference { get; set; }
        public List<DSFObjectPosition> Positions { get; set; }
    }

    public class DSFObjectPosition
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }
    }

    public class DSFPolygon
    {
        public string Reference { get; set; }
        public List<DSFPolygonPoint> Points { get; set; }
    }

    public class DSFPolygonPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class DSFFile
    {
        public List<DSFObject> Objects { get; set; }
        public List<DSFPolygon> Polygons { get; set; }

        public DSFFile()
        {
            this.Objects = new List<DSFObject>();
            this.Polygons = new List<DSFPolygon>();
        }
    }
}
