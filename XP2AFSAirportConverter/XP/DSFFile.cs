using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;

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
        public PolygonType PolygonType { get; set; }
        public List<DSFPolygonWindingCollection> WindingCollections { get; set; }
    }

    public class DSFPolygonWindingCollection
    {
        public double? HeightMeters { get; set; }
        public int Density { get; set; }
        public bool Chain { get; set; }
        public bool Ring { get; set; }
        public double SpacingMeters { get; set; }
        public double TextureHeading { get; set; }
        public List<DSFPolygonPointWinding> Windings { get; set; }
        public int NumberOfValuesPerPolygonPoint { get; set; }

    }

    public class DSFPolygonPointWinding
    {
        public List<DSFPolygonPoint> Points { get; set; }
    }

    public class DSFPolygonPoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double BerizerLatitude { get; set; }
        public double BezierLongitude { get; set; }
        public double ControlLatitude { get; set; }
        public double ControlLongitude { get; set; }
        public int WallType { get; set; }
        public double S { get; set; }
        public double T { get; set; }
        public double ControlS { get; set; }
        public double ControlT { get; set; }
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
