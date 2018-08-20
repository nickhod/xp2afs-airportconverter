using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Models;

namespace XP2AFSAirportConverter.XP
{

    public class DSFTextFileParser
    {
        private DSFFile dsfFile;
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        // https://github.com/X-Plane/xptools/tree/master/src/DSFTools
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
                            break;
                        case "BEGIN_WINDING":
                            break;
                        case "POLYGON_POINT":
                            break;
                        case "END_WINDING":
                            break;
                        case "END_POLYGON":
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
                log.Error("Error parsing object index");
            }



        }

        public void ParsePolygonDef(string[] elements)
        {
            DSFPolygon poly = new DSFPolygon();

            poly.Reference = elements[1];
            this.dsfFile.Polygons.Add(poly);
        }


    }
}
