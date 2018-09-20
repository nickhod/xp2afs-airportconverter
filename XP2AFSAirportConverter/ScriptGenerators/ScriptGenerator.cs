using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.ScriptGenerators.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.ScriptGenerators
{
    public abstract class ScriptGenerator
    {
        protected DATFile datFile;
        protected DSFFile dsfFile;
        protected TSCFile tscFile;
        protected string icao;

        public abstract void GenerateScripts(string icao, DATFile datFile, DSFFile dsfFile, TSCFile tscFile, string outputFolder, string texturesFolder);

        /// <summary>
        /// 
        /// </summary>
        protected void CalculateRunways(ScriptModel scriptModel)
        {
            int i = 0;
            foreach (var runway in this.datFile.LandRunways)
            {
                var scriptRunway = new ScriptRunway();

                double runwayShoulderSize = 0;

                // Might be wrong but X-Plane seems to use around 1/5th of the runway width as a shoulder
                if (runway.ShoulderType != ShoulderType.NoShoulder)
                {
                    runwayShoulderSize = runway.Width / 5;
                }

                scriptRunway.Width = runway.Width + (runwayShoulderSize*2);

                var end1Coord = new GeoCoordinate(runway.End1.Latitude, runway.End1.Longitude);
                var end2Coord = new GeoCoordinate(runway.End2.Latitude, runway.End2.Longitude);

                var runwayLength = end1Coord.GetDistanceTo(end2Coord);
                scriptRunway.Length = runwayLength + (runwayShoulderSize * 2);

                var angle = GeoHelper.DegreeBearing(end1Coord, end2Coord);
                scriptRunway.Angle = -angle;

                var runwayMidPoint = GeoHelper.MidPoint(end1Coord, end2Coord);
                var runwayPosition = GeoCoordinateToPoint(tscFile.Location, runwayMidPoint);
                scriptRunway.X = runwayPosition.X;
                scriptRunway.Y = runwayPosition.Y;
                scriptRunway.Index = i;

                switch (runway.ShoulderType)
                {
                    case ShoulderType.AsphaltShoulder:
                        scriptRunway.ShoulderType = "asphalt";
                        break;
                    case ShoulderType.ConcreneShoulder:
                        scriptRunway.ShoulderType = "concrete";
                        break;
                    case ShoulderType.NoShoulder:
                        scriptRunway.ShoulderType = "none";
                        break;
                }

                switch (runway.SurfaceType)
                {
                    case SurfaceType.Asphalt:
                        scriptRunway.SurfaceType = "asphalt";
                        break;
                    case SurfaceType.Concrete:
                        scriptRunway.SurfaceType = "concrete";
                        break;
                    case SurfaceType.Grass:
                    case SurfaceType.Dirt:
                    case SurfaceType.Gravel:
                    case SurfaceType.DryLakeBed:
                    case SurfaceType.Water:
                    case SurfaceType.SnowOrIce:
                    case SurfaceType.Transparent:
                        scriptRunway.SurfaceType = "transparent";
                        break;
                }

                scriptModel.Runways.Add(scriptRunway);
                i++;
            }
        }

        protected void CalculateDATFilePavements(ScriptModel scriptModel)
        {
            int i = 0;
            foreach (var pavement in this.datFile.Pavements)
            {
                var scriptPavement = new ScriptPavement();
                scriptPavement.Nodes = new List<ScriptNode>();
                scriptPavement.Name = pavement.Description;

                bool lastNodeWasCloseLoop = true;
                Point lastPosition = null;
                ScriptNode lastScriptNode = null;

                foreach (Node node in pavement.Nodes)
                {
                    var scriptNode = new ScriptNode();

                    var nodeCoord = new GeoCoordinate(node.Latitude, node.Longitude);
                    var nodePosition = GeoCoordinateToPoint(tscFile.Location, nodeCoord);

                    bool duplicate = false;

                    if (lastPosition != null)
                    {
                        if (nodePosition.X == lastPosition.X && nodePosition.Y == nodePosition.Y)
                        {
                            duplicate = true;
                            Debug.WriteLine("duplicate");
                        }
                    }

                    lastPosition = nodePosition;


                    GeoCoordinate bezierControl1Coord = null;
                    GeoCoordinate bezierControl2Coord = null;
                    Point bezierControl1Position = null;
                    Point bezierControl2Position = null;

                    scriptNode.X = nodePosition.X;
                    scriptNode.Y = nodePosition.Y;
                    scriptNode.IsBezier = false;
                    scriptNode.Render = true;

                    if (lastNodeWasCloseLoop)
                    {
                        scriptNode.OpenLoop = true;
                    }

                    if (node.CloseLoop)
                    {
                        scriptNode.CloseLoop = true;
                        lastNodeWasCloseLoop = true;
                    }
                    else
                    {
                        lastNodeWasCloseLoop = false;
                    }

                    if (node.BezierControlPoint1Latitude.HasValue && node.BezierControlPoint1Longitude.HasValue)
                    {
                        bezierControl1Coord = new GeoCoordinate(node.BezierControlPoint1Latitude.Value, node.BezierControlPoint1Longitude.Value);
                        bezierControl2Coord = new GeoCoordinate(node.BezierControlPoint2Latitude.Value, node.BezierControlPoint2Longitude.Value);
                        bezierControl1Position = GeoCoordinateToPoint(tscFile.Location, bezierControl1Coord);
                        bezierControl2Position = GeoCoordinateToPoint(tscFile.Location, bezierControl2Coord);

                        scriptNode.BezierControl1X = bezierControl2Position.X;
                        scriptNode.BezierControl1Y = bezierControl2Position.Y;

                        scriptNode.BezierControl2X = bezierControl1Position.X;
                        scriptNode.BezierControl2Y = bezierControl1Position.Y;

                        scriptNode.IsBezier = true;

                        if (node.SplitBezier)
                        {
                            scriptNode.IsBezierCorner = true;
                        }
                        else
                        {
                            scriptNode.IsBezierCorner = false;
                        }
                    }

                    if (node.IsCurve)
                    {
                        scriptNode.IsCurve = true;
                    }
                    else
                    {
                        scriptNode.IsCurve = false;
                    }

                    scriptPavement.Nodes.Add(scriptNode);
                }

                scriptPavement.Index = i;
                scriptModel.DATPavements.Add(scriptPavement);
                i++;
            }


        }

        protected void CalculateDSFFileBuildings(ScriptModel scriptModel)
        {
            int i = 0;
            foreach (var polygon in this.dsfFile.Polygons)
            {
                // Don't bother with fences or hedges for the moment
                if (!polygon.Reference.ToLower().Contains("fence") &&
                    !polygon.Reference.ToLower().Contains("hedge"))
                {
                    if (polygon.WindingCollections != null)
                    {
                        foreach (var polygonWindingCollection in polygon.WindingCollections)
                        {
                            if (polygonWindingCollection.HeightMeters.HasValue)
                            {
                                foreach (var winding in polygonWindingCollection.Windings)
                                {
                                    ScriptBuilding scriptBuilding = new ScriptBuilding();
                                    scriptBuilding.Height = polygonWindingCollection.HeightMeters.Value;
                                    scriptBuilding.Index = i;
                                    scriptBuilding.Nodes = new List<ScriptNode>();

                                    var polyRefFileInfo = new FileInfo(polygon.Reference);
                                    scriptBuilding.Name = polyRefFileInfo.Name.Replace(polyRefFileInfo.Extension, "");

                                    for (int j = 0; j < winding.Points.Count; j++)
                                    {
                                        var node = winding.Points[j];
                                        var scriptNode = new ScriptNode();

                                        var nodeCoord = new GeoCoordinate(node.Latitude, node.Longitude);
                                        var nodePosition = GeoCoordinateToPoint(tscFile.Location, nodeCoord);

                                        scriptNode.X = nodePosition.X;
                                        scriptNode.Y = nodePosition.Y;

                                        scriptNode.Render = true;

                                        if (j == 0)
                                        {
                                            scriptNode.OpenLoop = true;
                                        }

                                        if (j == winding.Points.Count - 1)
                                        {
                                            scriptNode.CloseLoop = true;
                                        }

                                        scriptBuilding.Nodes.Add(scriptNode);
                                    }

                                    scriptModel.Buildings.Add(scriptBuilding);
                                    i++;
                                }
                            }
                        }

                    }
                }

            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="originCoordinate"></param>
        /// <param name="geoCoordinate"></param>
        /// <returns></returns>
        protected Point GeoCoordinateToPoint(GeoCoordinate originCoordinate, GeoCoordinate geoCoordinate)
        {
            Point point = new Point();

            // A GeoCoordinate where the latitude is changed from the origin, but not the longitude
            var latitudeChanged = new GeoCoordinate();
            latitudeChanged.Latitude = geoCoordinate.Latitude;
            latitudeChanged.Longitude = originCoordinate.Longitude;

            // A GeoCoordinate where the longitude is changed from the origin, but not the latitude
            var longitudeChanged = new GeoCoordinate();
            longitudeChanged.Latitude = originCoordinate.Latitude;
            longitudeChanged.Longitude = geoCoordinate.Longitude;

            var distanceLatitude = originCoordinate.GetDistanceTo(latitudeChanged);
            var distanceLongitude = originCoordinate.GetDistanceTo(longitudeChanged);

            if (geoCoordinate.Latitude < originCoordinate.Latitude)
            {
                distanceLatitude *= -1;
            }

            if (geoCoordinate.Longitude < originCoordinate.Longitude)
            {
                distanceLongitude *= -1;
            }

            point.Y = distanceLatitude;
            point.X = distanceLongitude;

            return point;
        }
    }
}
