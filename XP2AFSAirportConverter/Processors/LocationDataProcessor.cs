using log4net;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Data;
using XP2AFSAirportConverter.Data.Models;

namespace XP2AFSAirportConverter.Processors
{
    public class GeoNamesCountryCodeResponse
    {
        [JsonProperty("languages")]
        public string Languages { get; set; }

        [JsonProperty("Distance")]
        public string Distance { get; set; }

        [JsonProperty("countryCode")]
        public string CountryCode { get; set; }

        [JsonProperty("countryName")]
        public string CountryName { get; set; }
    }


    public class LocationDataProcessor
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        public void GetMissingLocationData()
        {

            var wsTemplate = "http://api.geonames.org/countryCodeJSON?formatted=true&lat={0}&lng={1}&username=conceptdelta&style=full";

            using (ApplicationDbContext context = new ApplicationDbContext())
            {
                int airportCount = context.Airports.Count();

                for (int i =0; i < airportCount; i++)
                {
                    log.InfoFormat("Getting country data for airport {0} of {1}", i + 1, airportCount);

                    Airport airport = context.Airports.Where(a => a.AirportId == i+1).FirstOrDefault();

                    if (airport != null)
                    {
                        if (!airport.CountryId.HasValue)
                        {
                            using (var client = new WebClient())
                            {
                                var json = client.DownloadString(String.Format(wsTemplate, airport.Latitude, airport.Longitude));
                                var countryName = "Unknown";

                                if (!json.Contains("no country code found"))
                                {
                                    GeoNamesCountryCodeResponse response = JsonConvert.DeserializeObject<GeoNamesCountryCodeResponse>(json);

                                    if (!String.IsNullOrEmpty(response.CountryName))
                                    {
                                        countryName = response.CountryName;
                                    }
                                    else
                                    {
                                        log.Error("Something is wrong");
                                        break;
                                    }

                                }

                                Country country = context.Countries.Where(c => c.Name == countryName).FirstOrDefault();

                                if (country == null)
                                {
                                    country = new Country();
                                    country.Name = countryName;
                                    context.Countries.Add(country);
                                    context.SaveChanges();
                                }

                                airport.CountryId = country.CountryId;
                                context.SaveChanges();

                            }
                        }
                    }
                
                }

            }
        }
    }
}
