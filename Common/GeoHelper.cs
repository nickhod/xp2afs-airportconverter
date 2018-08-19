using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Common
{
    public static class GeoHelper
    {
        /// <summary>
        /// Calculate the bearing of the rhumb line between two points]
        /// https://stackoverflow.com/questions/2042599/direction-between-2-latitude-longitude-points-in-c-sharp
        /// </summary>
        /// <param name="coordinate1"></param>
        /// <param name="coordinate2"></param>
        /// <returns></returns>
        public static double DegreeBearing(GeoCoordinate coordinate1, GeoCoordinate coordinate2)
        {
            var dLon = DegreesToRadians(coordinate2.Longitude - coordinate1.Longitude);
            var dPhi = Math.Log(
                Math.Tan(DegreesToRadians(coordinate2.Latitude) / 2 + Math.PI / 4) / Math.Tan(DegreesToRadians(coordinate1.Latitude) / 2 + Math.PI / 4));
            if (Math.Abs(dLon) > Math.PI)
                dLon = dLon > 0 ? -(2 * Math.PI - dLon) : (2 * Math.PI + dLon);
            return ToBearing(Math.Atan2(dLon, dPhi));
        }

        public static double DegreesToRadians(double degrees)
        {
            return degrees * (Math.PI / 180);
        }

        public static double RadiansToDegrees(double radians)
        {
            return radians * 180 / Math.PI;
        }

        public static double ToBearing(double radians)
        {
            // convert radians to degrees (as bearing: 0...360)
            return (RadiansToDegrees(radians) + 360) % 360;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="posA"></param>
        /// <param name="posB"></param>
        /// <returns></returns>
        public static GeoCoordinate MidPoint(GeoCoordinate coordinate1, GeoCoordinate coordinate2)
        {
            GeoCoordinate midPoint = new GeoCoordinate();

            double dLon = DegreesToRadians(coordinate2.Longitude - coordinate1.Longitude);
            double Bx = Math.Cos(DegreesToRadians(coordinate2.Latitude)) * Math.Cos(dLon);
            double By = Math.Cos(DegreesToRadians(coordinate2.Latitude)) * Math.Sin(dLon);

            midPoint.Latitude = RadiansToDegrees(Math.Atan2(
                         Math.Sin(DegreesToRadians(coordinate1.Latitude)) + Math.Sin(DegreesToRadians(coordinate2.Latitude)),
                         Math.Sqrt(
                             (Math.Cos(DegreesToRadians(coordinate1.Latitude)) + Bx) *
                             (Math.Cos(DegreesToRadians(coordinate1.Latitude)) + Bx) + By * By)));

            midPoint.Longitude = coordinate1.Longitude + RadiansToDegrees(Math.Atan2(By, Math.Cos(DegreesToRadians(coordinate1.Latitude)) + Bx));

            return midPoint;
        }
    }
}
