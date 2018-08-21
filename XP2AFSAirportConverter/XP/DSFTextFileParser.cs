using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Models;

namespace XP2AFSAirportConverter.XP
{

    public class DSFTextFileParser
    {
        private DSFFile dsfFile;
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        private int? currentPolygonIndex;
        private DSFPolygonPointWinding currentPolygonWinding;
        private DSFPolygonWindingCollection currentPolygonWindingCollection;


        // https://github.com/X-Plane/xptools/tree/master/src/DSFTools
        //https://developer.x-plane.com/?article=dsf-usage-in-x-plane

        public DSFFile ParseFromString(string data)
        {

            this.dsfFile = new DSFFile();

            foreach (var line in data.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries))
            {
                string[] elements = line.Split(' ');

                if (elements.Length > 0)
                {
                    switch(elements[0])
                    {
                        case "OBJECT_DEF":
                            this.ParseObjectDef(elements);
                            break;
                        case "POLYGON_DEF":
                            this.ParsePolygonDef(elements);
                            break;
                        case "OBJECT":
                            this.ParseObjectLocation(elements);
                            break;
                        case "BEGIN_POLYGON":
                            this.ParseBeginPolygon(elements);
                            break;
                        case "BEGIN_WINDING":
                            this.ParseBeginWinding(elements);
                            break;
                        case "POLYGON_POINT":
                            this.ParsePolygonPoint(elements);
                            break;
                        case "END_WINDING":
                            this.ParseEndPolygonWinding(elements);
                            break;
                        case "END_POLYGON":
                            this.ParseEndPolygon(elements);
                            break;
                    }
                }
            }

            return this.dsfFile;
        }

        public void ParseObjectDef(string[] elements)
        {
            DSFObject obj = new DSFObject();

            obj.Reference = elements[1];
            this.dsfFile.Objects.Add(obj);
        }
        public void ParseObjectLocation(string[] elements)
        {
            int objectIndex;
            if (int.TryParse(elements[1], out objectIndex))
            {
                if (objectIndex < this.dsfFile.Objects.Count)
                {
                    var obj = this.dsfFile.Objects[objectIndex];
                    var position = new DSFObjectPosition();

                    double latitude;
                    if (double.TryParse(elements[3], out latitude))
                    {
                        position.Latitude = latitude;
                    }
                    else
                    {
                        log.Error("Error parsingp object latitude");
                    }

                    double longitude;
                    if (double.TryParse(elements[2], out longitude))
                    {
                        position.Longitude = longitude;
                    }
                    else
                    {
                        log.Error("Error parsing object longitude");
                    }

                    obj.Positions.Add(position);
                }
                else
                {
                    log.Error("An object has been referenced that is not in the object collection");
                }


            }
            else
            {
                log.Error("Error parsing object index");
            }



        }

        public void ParsePolygonDef(string[] elements)
        {
            DSFPolygon poly = new DSFPolygon();

            poly.Reference = elements[1];

            var fileExtension = Path.GetExtension(poly.Reference);

            switch(fileExtension)
            {
                case "fac":
                    poly.PolygonType = PolygonType.Facade;
                    break;
                case "for":
                    poly.PolygonType = PolygonType.Forest;
                    break;
                case "pol":
                    poly.PolygonType = PolygonType.Polygon;
                    break;
                default:
                    poly.PolygonType = PolygonType.Polygon;
                    break;
            }

            this.dsfFile.Polygons.Add(poly);
        }

        public void ParseBeginPolygon(string [] elements)
        {
            int polygonIndex;
            if (int.TryParse(elements[1], out polygonIndex))
            {
                this.currentPolygonIndex = polygonIndex;
            }
            else
            {
                log.Error("Error parsing polygon index");
            }

            int value2;
            if (!int.TryParse(elements[2], out value2))
            {
                log.Error("Error parsing polygon value 2");
            }

            int value3;
            if (!int.TryParse(elements[3], out value3))
            {
                log.Error("Error parsing polygon value 3");
            }

            if (polygonIndex < dsfFile.Polygons.Count)
            {
                var polygon = this.dsfFile.Polygons[polygonIndex];
                var polygonWindingCollection = new DSFPolygonWindingCollection();
                polygonWindingCollection.NumberOfValuesPerPolygonPoint = value3;

                switch (polygon.PolygonType)
                {
                    case PolygonType.Polygon:
                        polygonWindingCollection.TextureHeading = value2;
                        break;
                    case PolygonType.Facade:
                        polygonWindingCollection.HeightMeters = value2;
                        break;
                    case PolygonType.Forest:
                        polygonWindingCollection.Density = value2;
                        break;
                    case PolygonType.Line:
                        break;
                    case PolygonType.String:
                        break;
                    case PolygonType.AutogenBlock:
                        break;
                    default:
                        break;
                }

                this.currentPolygonWindingCollection = polygonWindingCollection;
            }
            else
            {
                log.Error("A polygon has been referenced that is not in the polygons collection");
            }


        }

        public void ParseBeginWinding(string[] elements)
        {
            // We shouldn't be here without an open WindingCollection
            if (this.currentPolygonWindingCollection != null)
            {
                DSFPolygonPointWinding winding = new DSFPolygonPointWinding();
                this.currentPolygonWinding = winding;
            }
        }

        public void ParsePolygonPoint(string[] elements)
        {
            // We shouldn't be here without an open Winding
            if (this.currentPolygonWinding != null)
            {
                DSFPolygonPoint point = new DSFPolygonPoint();

                double latitude;
                if (double.TryParse(elements[2], out latitude))
                {
                    point.Latitude = latitude;
                }
                else
                {
                    log.Error("Error parsingp polygon point latitude");
                }

                double longitude;
                if (double.TryParse(elements[1], out longitude))
                {
                    point.Longitude = longitude;
                }
                else
                {
                    log.Error("Error parsing polygon point longitude");
                }

                // The number of extra values to process, excluding the lat lon
                var extraValuesCount = this.currentPolygonWindingCollection.NumberOfValuesPerPolygonPoint - 2;
                var currentPolygon = this.dsfFile.Polygons[this.currentPolygonIndex.Value];
                var extraValues = new List<double>();

                for (int i = 0; i < extraValuesCount; i++)
                {
                    double extraValue;
                    if (double.TryParse(elements[i+2], out extraValue))
                    {
                        extraValues.Add(extraValue);
                    }
                    else
                    {
                        log.Error("Error parsing polygon point extra value");
                    }
                }

                switch (currentPolygon.PolygonType)
                {
                    case PolygonType.Polygon:
                        if (extraValuesCount == 2)
                        {
                            if (this.currentPolygonWindingCollection.TextureHeading == 65535)
                            {
                                // Control lon, control lat
                                point.ControlLongitude = extraValues[0];
                                point.ControlLatitude = extraValues[1];
                            }
                            else
                            {
                                // S, T
                                point.S = extraValues[0];
                                point.T= extraValues[1];
                            }
                        }
                        else if (extraValuesCount == 6)
                        {
                            // Control lon, control lat, S, T, control S, control T
                            point.ControlLongitude = extraValues[0];
                            point.ControlLatitude = extraValues[1];
                            point.S = extraValues[2];
                            point.T = extraValues[3];
                            point.ControlS = extraValues[4];
                            point.ControlT = extraValues[5];
                        }
                        break;
                    case PolygonType.Facade:
                        if (extraValuesCount == 1)
                        {
                            // Wall type
                            point.WallType = (int)extraValues[0];
                        }
                        else if (extraValuesCount == 2)
                        {
                            // Bezier lon, Bezier lat
                            point.BezierLongitude = extraValues[0];
                            point.BerizerLatitude = extraValues[1];
                        }
                        else if (extraValuesCount == 3)
                        {
                            // Wall type, Bezier lon, Bezier lat
                            point.WallType = (int)extraValues[0];
                            point.BezierLongitude = extraValues[1];
                            point.BerizerLatitude = extraValues[2];
                        }
                        break;
                    case PolygonType.Forest:
                        break;
                    case PolygonType.Line:
                        break;
                    case PolygonType.String:
                        break;
                    case PolygonType.AutogenBlock:
                        break;
                    default:
                        break;
                }


                if (this.currentPolygonWinding.Points == null)
                {
                    this.currentPolygonWinding.Points = new List<DSFPolygonPoint>();
                }

                this.currentPolygonWinding.Points.Add(point);
            }

        }

        public void ParseEndPolygonWinding(string[] elements)
        {
            if (this.currentPolygonWindingCollection.Windings == null)
            {
                this.currentPolygonWindingCollection.Windings = new List<DSFPolygonPointWinding>();
            }

            this.currentPolygonWindingCollection.Windings.Add(this.currentPolygonWinding);
            this.currentPolygonWinding = null;
        }

        public void ParseEndPolygon(string[] elements)
        {
            if (this.currentPolygonIndex.HasValue)
            {
                var polygon = this.dsfFile.Polygons[this.currentPolygonIndex.Value];

                if (polygon.WindingCollections == null)
                {
                    polygon.WindingCollections = new List<DSFPolygonWindingCollection>();
                }
                polygon.WindingCollections.Add(this.currentPolygonWindingCollection);

                this.currentPolygonWindingCollection = null;
                this.currentPolygonIndex = null;
            }

        }
    }
}
