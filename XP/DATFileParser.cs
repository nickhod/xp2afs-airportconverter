using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.XP
{
    //https://developer.x-plane.com/article/airport-data-apt-dat-file-format-specification
    //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT850-Spec.pdf
    //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT1000-Spec.pdf

    public static class RowCode
    {
        public static int LandAirportHeader = 1;
        public static int SeaplaneBaseHeader = 16;
        public static int HeliportHeader = 17;
        public static int LandRunway = 100;
        public static int WaterRunway = 101;
        public static int Helipad = 102;
        public static int Pavement = 110;
        public static int LinearFeature = 120;
        public static int AirportBoundary = 130;
        public static int Node = 111;



    }

    public class DATFileParser
    {
        //https://developer.x-plane.com/article/airport-data-apt-dat-file-format-specification
        //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT1000-Spec.pdf
        //http://developer.x-plane.com/wp-content/uploads/2015/11/XP-APT850-Spec.pdf


        public DATFile Parse(string data)
        {
            DATFile datFile = new DATFile();

            StringReader strReader = new StringReader(data);
            while (true)
            {
                var line = strReader.ReadLine();
                if (line != null)
                {
                    int i = 0;
                }
                else
                {
                    break;

                }
            }

            return datFile;
        }

    }
}
