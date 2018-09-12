using log4net;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;
using System.Xml.Serialization;
using XP2AFSAirportConverter.SceneryGateway;
using XP2AFSAirportConverter.SceneryGateway.Models;

namespace XP2AFSAirportConverter.Processors
{
    public class AirportListProcessor
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");
        private SceneryGatewayApi sceneryGatewayApi;

        public AirportListProcessor()
        {
            this.sceneryGatewayApi = new SceneryGatewayApi();
        }

        public void GetAirportList()
        {
            log.Info("Getting list of all airports");
            var airportList = this.sceneryGatewayApi.GetAllAirports();

            // Clean up the airport name so XML deserialization doesn't fail
            foreach (AirportListItem airport in airportList.Airports)
            {
                TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

                airport.AirportName = HttpUtility.HtmlDecode(airport.AirportName);
                airport.AirportName = airport.AirportName.Replace(@"&#x13; ", "");
                airport.AirportName = airport.AirportName.Replace(@"&#xC;", "");
                airport.AirportName = Regex.Replace(airport.AirportName, @"&#x?[^;]{1,6};", string.Empty);
                airport.AirportName = textInfo.ToTitleCase(airport.AirportName);
            }

            log.Info("Writing list of all airports to file");
            using (MemoryStream ms = new MemoryStream())
            {
                var writer = new StreamWriter(ms, Encoding.UTF8);
                var serializer = new XmlSerializer(typeof(AirportList));
                serializer.Serialize(writer, airportList);
                writer.Flush();

                //if the serialization succeed, rewrite the file.
                File.WriteAllBytes(XP2AFSConverterManager.Settings.XP2AFSConverterFolder + @"xp_airports.xml", ms.ToArray());
            }
        }

        public AirportList DeserializeAirportListFile(string airportsFile)
        {
            AirportList airportList = null;

            if (File.Exists(airportsFile))
            {
                log.Info("Loading airports from file");

                // Annoying special characters issue.
                // Not sure why these don't work given that we're using UTF8 to encode and decode
                var airportFileString = File.ReadAllText(airportsFile);
                airportFileString = Regex.Replace(airportFileString, @"&#x?[^;]{1,6};", string.Empty);

                using (MemoryStream memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(airportFileString)))
                {
                    XmlSerializer xmlSerializer = new XmlSerializer(typeof(AirportList));
                    StreamReader reader = new StreamReader(memoryStream, Encoding.UTF8);
                    airportList = (AirportList)xmlSerializer.Deserialize(reader);
                }
            }
            else
            {
                log.Info("Airport file not found");
            }

            return airportList;
        }
    }
}
