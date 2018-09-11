using CsvHelper;
using log4net;
using MySql.Data.MySqlClient;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Data;
using XP2AFSAirportConverter.Data.Models;
using XP2AFSAirportConverter.Models;

namespace XP2AFSAirportConverter.Processors
{
    public class AirportCsvListImportProcessor
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");

        private Dictionary<string, Country> countryLookup;
        private Dictionary<string, Region> regionLookup;
        private Dictionary<string, City> cityLookup;

        public AirportCsvListImportProcessor()
        {
            countryLookup = new Dictionary<string, Country>();
            regionLookup = new Dictionary<string, Region>();
            cityLookup = new Dictionary<string, City>();
        }

        public void ImportAirportCsvList(string filePath)
        {
            // While testing
            //SET FOREIGN_KEY_CHECKS = 0;
            //truncate table airports;
            //truncate table cities;
            //truncate table regions;
            //truncate table countries;
            //SET FOREIGN_KEY_CHECKS = 1;

            try
            {
                var reader = new StreamReader(filePath);
                var csv = new CsvReader(reader);

                csv.Read();
                csv.ReadHeader();
                int i = 0;
                while (csv.Read())
                {
                    log.InfoFormat("Adding airport to database {0}", i + 1);

                    var record = csv.GetRecord<AirportCsvListItem>();

                    using (ApplicationDbContext context = new ApplicationDbContext())
                    {
                        Airport airport = context.Airports.Where(a => a.Code == record.Code).FirstOrDefault();

                        bool addAirport = false;
                        if (airport == null)
                        {
                            addAirport = true;
                            airport = new Airport();
                        }

                        airport.Code = record.Code.Trim();
                        airport.FAACode = record.FAACode.Trim();
                        airport.HasPavement = record.HasPavements;
                        airport.IATA = record.IATA.Trim();
                        airport.ICAO = record.ICAO.Trim();
                        airport.IsHelipad = record.IsHelipad;
                        airport.Latitude = record.Latitude.Value;
                        airport.Longitude = record.Longitude.Value;
                        airport.Name = record.Name.Trim();
                        airport.RegionCode = record.RegionCode.Trim();
                        airport.RunwayCount = record.RunwayCount;
                        airport.RunwaySurfaceType = record.RunwaySurfaceType;
                        airport.UpdatedOn = DateTime.UtcNow;
                        airport.XP3D = record.Is3D;

                        // Country
                        if (!string.IsNullOrEmpty(record.Country))
                        {
                            Country country;

                            if (!countryLookup.TryGetValue(record.Country, out country))
                            {
                                country = new Country();
                                country.Name = record.Country;
                                context.Countries.Add(country);
                                context.SaveChanges();

                                this.countryLookup.Add(country.Name, country);
                            }

                            airport.CountryId = country.CountryId;
                        }


                        // Region
                        if (!string.IsNullOrEmpty(record.Region))
                        {
                            Region region;

                            if (!regionLookup.TryGetValue(record.Region, out region))
                            {
                                region = new Region();
                                region.Name = record.Region;
                                context.Regions.Add(region);
                                context.SaveChanges();

                                this.regionLookup.Add(region.Name, region);
                            }

                            airport.RegionId = region.RegionId;
                        }


                        // City
                        if (!string.IsNullOrEmpty(record.City))
                        {
                            City city;

                            if (!cityLookup.TryGetValue(record.City, out city))
                            {
                                city = new City();
                                city.Name = record.City;
                                context.Cities.Add(city);
                                context.SaveChanges();

                                this.cityLookup.Add(city.Name, city);
                            }

                            airport.CityId = city.CityId;
                        }

                        if (addAirport)
                        {
                            context.Airports.Add(airport);
                        }

                        context.SaveChanges();
                        i++;
                    }

                }





            }
            catch (Exception ex)
            {

            }

            //using (MySqlConnection connection = new MySqlConnection())
            //{ 

            //    connection.Open();



            //}
        }
    }
}
