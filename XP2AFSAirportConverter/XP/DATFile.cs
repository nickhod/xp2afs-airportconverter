﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;

namespace XP2AFSAirportConverter.XP
{
    //https://developer.x-plane.com/article/airport-data-apt-dat-file-format-specification
    //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT850-Spec.pdf
    //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT1000-Spec.pdf

    public class DATFile
    {
        public AirportHeader AirportHeader { get; set; }
        public IList<LandRunway> LandRunways { get; set; }
        public IList<WaterRunway> WaterRunways { get; set; }
        public Helipad Helipad { get; set; }
        public AirportBoundary AirportBoundary { get; set; }
        public IList<Pavement> Pavements { get; set; }
        public IList<LinearFeature> LinearFeatures { get; set; }
        public IList<Metadata> Metadata { get; set; }

        public Dictionary<string, string> MetadataLookup
        {
            get
            {
                Dictionary<string, string> lookup = new Dictionary<string, string>();

                if (this.Metadata != null)
                {
                    foreach (var metadata in this.Metadata)
                    {
                        lookup.Add(metadata.Key, metadata.Value);
                    }
                }

                return lookup;

            }
        }
    }

    public class Metadata
    {
        public string Key { get; set; }
        public string Value { get; set; }
    }

    public class AirportHeader
    {
        public double ElevationFeet { get; set; }
        public string ICAOCode { get; set; }
        public string Name { get; set; }
        public AirportType AirportType { get; set; }
    }

    public class LandRunway
    {
        public double Width { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public ShoulderType ShoulderType { get; set; }
        public double Smoothness { get; set; }
        public bool HasCenterLineLights { get; set; }
        public RunwayEdgeLights EdgeLights { get; set; }
        public bool AutoGenerateDistanceRemainingSigns { get; set; }
        public LandRunwayEnd End1 { get; set; }
        public LandRunwayEnd End2 { get; set; }
    }

    public class LandRunwayEnd
    {
        public string Number { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double LengthOfDisplacedThreshold { get; set; }
        public double LengthOfOverrun { get; set; }
        public RunwayMarkings RunwayMarkings  { get; set; }
        public ApproachLighting ApproachLighting { get; set; }
        public bool FlagForRunwayTouchdownZoneLighting { get; set; }
        public REILLights REILLights { get; set; }
    }


    public class WaterRunway
    {
        public double Width { get; set; }

        public bool HasPerimeterBuoys { get; set; }
        public WaterRunwayEnd End1 { get; set; }
        public WaterRunwayEnd End2 { get; set; }

    }

    public class WaterRunwayEnd
    {
        public string Number { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }

    public class Helipad
    {
        public string Designator { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Orientation { get; set; }
        public double Length { get; set; }
        public double Width { get; set; }
        public SurfaceType SurfaceCode { get; set; }
        public string HelipadMarkings { get; set; }
        public ShoulderType ShoulderSurfaceType { get; set; }
        public double Smoothness { get; set; }
        public bool HasEdgeLighting { get; set; }

    }

    public class Pavement
    {
        public IList<Node> Nodes { get; set; }
        public SurfaceType SurfaceType { get; set; }
        public double Smoothness { get; set; }
        public double Orientation { get; set; }
        public string Description { get; set; }
    }

    public class LinearFeature
    {
        public string Description { get; set; }
        public IList<Node> Nodes { get; set; }

    }

    public class AirportBoundary
    {
        public string Description { get; set; }
        public IList<Node> Nodes { get; set; }

    }

    public class Node
    {
        public int RowCode { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double? BezierControlPoint1Latitude { get; set; }
        public double? BezierControlPoint1Longitude { get; set; }

        public double? BezierControlPoint2Latitude { get; set; }
        public double? BezierControlPoint2Longitude { get; set; }
        public bool SplitBezier { get; set; }
        public bool IsCurve { get; set; }
        public bool End { get; set; }
        public bool CloseLoop { get; set; }

        public LineType LineType { get; set; }

        public LineType LightingLineType { get; set; }
    }



    public class Viewpoint
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double HeightInFeet { get; set; }
        public string Name { get; set; }
    }

    public class StarupLocationp
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Heading { get; set; }
        public string Name { get; set; }
    }

    public class LightBeacon
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public BeaconType BeaconType { get; set; }
        public string Name { get; set; }
    }

    public class Windsock
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public bool IsIlluminated { get; set; }
        public string Name { get; set; }
    }

    public class Sign
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public double Orientation { get; set; }
    }

    public class LightingObject
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public LightingObjectType LightingObjectType { get; set; }
        public double Orientation { get; set; }
        public double VisualGlideslopeAngle { get; set; }
        public string AssociatedRunwayNumber { get; set; }
        public string Description { get; set; }
    }

    public class TrafficFlow
    {

    }

    public class TrafficFlowWindRule
    {

    }

    public class TrafficFlowCeilingRule
    {

    }

    public class TrafficFlowVisibilityRule
    {

    }

    public class TrafficTimeRule
    {
    }

    public class RunwayInUseRule
    {

    }

    public class VFRPatternRule
    {
        public string Runway { get; set; }
        public Direction Direction { get; set; }
    }

    public class TaxiRoutingNetwork
    {

    }

    public class TaxiRoutingNode
    {

    }

    public class TaxiRoutingEdge
    {
        
    }

    public class EdgeActiveZone
    {

    }

    public class TaxiLocation
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public double Heading { get; set; }

        public TaxiLocationType TaxiLocationType { get; set; }

        public AirplaneType AirplaneType { get; set; }
    }

    public class ATCRecorded
    {

    }

    public class ATCUnicom
    {

    }

    public class ATCClearanceDelivery
    {

    }

    public class ATCGround
    {

    }

    public class ATCTower
    {

    }

    public class ATCApproach
    {

    }

    public class ATCDeparture
    {

    }
}
