using GeoAPI.Geometries;
using log4net;
using NetTopologySuite.Geometries;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.AFS;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.Converters
{
    public class DATConverter
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        private DATFile datFile;

        public void Convert(DATFile datFile, TSCFile tscFile, TOCFile tocFile)
        {
            this.datFile = datFile;

            tscFile.ICAO = datFile.AirportHeader.ICAOCode;
            tscFile.AirportLongName = datFile.AirportHeader.Name;
            tscFile.AirportShortName = datFile.AirportHeader.Name;

            foreach (var landRunway in datFile.LandRunways)
            {
                TSCRunway runway = new TSCRunway();
                runway.Width = landRunway.Width;

                runway.End1 = new TSCRunwayEnd();
                runway.End2 = new TSCRunwayEnd();

                ConvertRunwayEnd(runway.End1, landRunway.End1);
                ConvertRunwayEnd(runway.End2, landRunway.End2);

                tscFile.Runways.Add(runway);
            }

            tscFile.Location = this.CalculateAirportCenterPoint();
        }

        /// <summary>
        /// Calculate the center point of the airport
        /// The DAT data should give us an airport boundary which will be an irregular polygon
        /// If this is present, we work out the center of its bounding box
        /// If this isn't present we just get the end of a runway
        /// </summary>
        /// <returns></returns>
        private GeoCoordinate CalculateAirportCenterPoint()
        {
            GeoCoordinate centerPoint = new GeoCoordinate();

            if (this.datFile.AirportBoundary != null && this.datFile.AirportBoundary.Nodes != null && this.datFile.AirportBoundary.Nodes.Count > 0)
            {
                GeometryFactory geometryFactory = new GeometryFactory();
                List<Geometry> geometries = new List<Geometry>();

                foreach (var node in this.datFile.AirportBoundary.Nodes)
                {
                    Coordinate coord = new Coordinate(node.Longitude, node.Latitude);
                    NetTopologySuite.Geometries.Point geometry = (NetTopologySuite.Geometries.Point)geometryFactory.CreatePoint(coord);
                    geometries.Add(geometry);
                }

                GeometryCollection geometryCollection = new GeometryCollection(geometries.ToArray());
                var boundingBox = geometryCollection.Envelope;
                centerPoint.Longitude = boundingBox.Centroid.Coordinate.X;
                centerPoint.Latitude = boundingBox.Centroid.Coordinate.Y;
            }
            else
            {
                // As a plan B get the center of the first or only runway
                // TODO - use the center point of a runway if we don't have a bounding box
                if (this.datFile.Helipad != null)
                {
                    centerPoint.Latitude = this.datFile.Helipad.Latitude;
                    centerPoint.Longitude = this.datFile.Helipad.Longitude;
                }

                if (this.datFile.WaterRunways!= null && this.datFile.WaterRunways.Count > 0)
                {
                    centerPoint.Latitude = this.datFile.WaterRunways[0].End1.Latitude;
                    centerPoint.Longitude = this.datFile.WaterRunways[0].End1.Longitude;
                }

                if (this.datFile.LandRunways != null && this.datFile.LandRunways.Count > 0)
                {
                    centerPoint.Latitude = this.datFile.LandRunways[0].End1.Latitude;
                    centerPoint.Longitude = this.datFile.LandRunways[0].End1.Longitude;
                }
            }

            //log.DebugFormat("{0} {1}", centerPoint.Latitude, centerPoint.Longitude);
            return centerPoint;
        }

        private void ConvertRunwayEnd(TSCRunwayEnd tscRunwayEnd, LandRunwayEnd landRunwayEnd)
        {
            tscRunwayEnd.ApproachLighting = landRunwayEnd.ApproachLighting;
            tscRunwayEnd.EndpointLocation = new GeoCoordinate(landRunwayEnd.Latitude, landRunwayEnd.Longitude);
            tscRunwayEnd.Name = landRunwayEnd.Number;
            tscRunwayEnd.REILLights = landRunwayEnd.REILLights;

            // Technically not right an can be improved as we have the length of the threshold
            tscRunwayEnd.ThresholdLocation = tscRunwayEnd.EndpointLocation;

            // In X-Plane, PAIP / VASI is manually placed. We need to figure out something to get these out of the
            // list of lighting objects. Hard given multple runways
            //tscRunwayEnd.PAPI = 

        }
    }
}
