using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Common
{
    public class GeoCoordinate
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public GeoCoordinate()
        {
        }

        public GeoCoordinate(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public double GetDistanceTo(GeoCoordinate end2Coord)
        {
            var coord1 = new System.Device.Location.GeoCoordinate(this.Latitude, this.Longitude);
            var coord2 = new System.Device.Location.GeoCoordinate(end2Coord.Latitude, end2Coord.Longitude);

            return coord1.GetDistanceTo(coord2);
        }
    }

    public class GeoCoordinate3d : GeoCoordinate
    {
        public double Height { get; set; }

        public GeoCoordinate3d()
        {
        }

        public GeoCoordinate3d(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
        }

        public GeoCoordinate3d(double latitude, double longitude, double height)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.Height = height;
        }
    }
}
