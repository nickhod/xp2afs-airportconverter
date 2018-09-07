﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        protected ScriptModel scriptModel;
        protected DATFile datFile;
        protected DSFFile dsfFile;
        protected TSCFile tscFile;
        protected string icao;

        public abstract void GenerateScripts(string icao, DATFile datFile, DSFFile dsfFile, TSCFile tscFile, string outputFolder);

        /// <summary>
        /// 
        /// </summary>
        protected void CalculateRunways()
        {
            int i = 0;
            foreach (var runway in this.datFile.LandRunways)
            {
                var scriptRunway = new ScriptRunway();
                scriptRunway.Width = runway.Width;

                var end1Coord = new GeoCoordinate(runway.End1.Latitude, runway.End1.Longitude);
                var end2Coord = new GeoCoordinate(runway.End2.Latitude, runway.End2.Longitude);

                var runwayLength = end1Coord.GetDistanceTo(end2Coord);
                scriptRunway.Length = runwayLength;

                var angle = GeoHelper.DegreeBearing(end1Coord, end2Coord);
                scriptRunway.Angle = -angle;

                var runwayMidPoint = GeoHelper.MidPoint(end1Coord, end2Coord);
                var runwayPosition = GeoCoordinateToPoint(tscFile.Location, runwayMidPoint);
                scriptRunway.X = runwayPosition.X;
                scriptRunway.Y = runwayPosition.Y;
                scriptRunway.Index = i;

                this.scriptModel.Runways.Add(scriptRunway);
                i++;
            }
        }

        protected void CalculateDATFilePavements()
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


                    GeoCoordinate bezierCoord = null;
                    Point bezierPosition = null;

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

                    if (node.BezierControlPointLatitude.HasValue && node.BezierControlPointLongitude.HasValue)
                    {
                        bezierCoord = new GeoCoordinate(node.BezierControlPointLatitude.Value, node.BezierControlPointLongitude.Value);
                        bezierPosition = GeoCoordinateToPoint(tscFile.Location, bezierCoord);

                        scriptNode.BezierControlX = bezierPosition.X;
                        scriptNode.BezierControlY = bezierPosition.Y;
                        scriptNode.IsBezier = true;
                    }

                    if (!duplicate)
                    {
                        lastScriptNode = scriptNode;
                        scriptPavement.Nodes.Add(scriptNode);
                    }
                    else
                    {
                        // This is a duplicate, but it's a bezier so we need to render it
                        if (scriptNode.IsBezier)
                        {
                            //if (lastScriptNode.IsBezier)
                            //{
                            //    lastScriptNode = scriptNode;
                            //    scriptPavement.Nodes.Add(scriptNode);
                            //}
                            //else
                            //{

                            //}

                            lastScriptNode.Render = false;

                            if (lastScriptNode.CloseLoop)
                            {
                                scriptNode.CloseLoop = true;
                            }

                            if (lastScriptNode.OpenLoop)
                            {
                                scriptNode.OpenLoop = true;
                            }

                            lastScriptNode = scriptNode;
                            scriptPavement.Nodes.Add(scriptNode);
                        }
                        // Not a duplicate, don't add it so it wont be rendered
                        else
                        {
                            if (scriptNode.CloseLoop)
                            {
                                lastScriptNode.CloseLoop = true;
                            }

                            if (scriptNode.OpenLoop)
                            {
                                lastScriptNode.OpenLoop = true;
                            }
                        }


                    }


                }

                scriptPavement.Index = i;
                this.scriptModel.DATPavements.Add(scriptPavement);
                i++;
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
