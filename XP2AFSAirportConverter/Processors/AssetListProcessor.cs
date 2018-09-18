using log4net;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;
using XP2AFSAirportConverter.Models;
using XP2AFSAirportConverter.SceneryGateway.Models;
using XP2AFSAirportConverter.XP;

namespace XP2AFSAirportConverter.Processors
{
    public class AssetListProcessor
    {
        private readonly ILog log = LogManager.GetLogger("XP2AFSAirportConverter");


        public void GenerateAssetList(AirportList airportList)
        {
            Dictionary<string, AssetListItem> assetLookup = CreateAssetLookup(airportList);

            WriteAssetListFiles(assetLookup);
        }

        private void WriteAssetListFiles(Dictionary<string, AssetListItem> assetLookup)
        {
            var assetMapsDirectory = DirectoryHelper.GetAssetMapsDirectory(XP2AFSConverterManager.Settings);

            if (!Directory.Exists(assetMapsDirectory))
            {
                Directory.CreateDirectory(assetMapsDirectory);
            }

            var polAssets = assetLookup.Select(x => x.Value)
                .Where(x => x.Name.Contains(".pol"))
                .OrderByDescending(x => x.Count).ToList();

            var objAssets = assetLookup.Select(x => x.Value)
                .Where(x => x.Name.Contains(".obj"))
                .OrderByDescending(x => x.Count).ToList();

            var agpAssets = assetLookup.Select(x => x.Value)
                .Where(x => x.Name.Contains(".agp"))
                .OrderByDescending(x => x.Count).ToList();

            var facAssets = assetLookup.Select(x => x.Value)
                .Where(x => x.Name.Contains(".fac"))
                .OrderByDescending(x => x.Count).ToList();


            log.Info("Writing files");
            WriteAssetListFile(assetMapsDirectory + "\\pol.csv", polAssets);
            WriteAssetListFile(assetMapsDirectory + "\\obj.csv", objAssets);
            WriteAssetListFile(assetMapsDirectory + "\\agp.csv", agpAssets);
            WriteAssetListFile(assetMapsDirectory + "\\fac.csv", facAssets);
        }

        private void WriteAssetListFile(string filePath, List<AssetListItem> assets)
        {
            using (var fs = new FileStream(filePath, FileMode.OpenOrCreate, FileAccess.Write))
            {
                using (var fw = new StreamWriter(fs))
                {
                    foreach (var asset in assets)
                    {
                        fw.WriteLine(string.Format("{0},none,{1}", asset.Name, asset.Count));
                    }
                }
            }
        }

        private Dictionary<string, AssetListItem> CreateAssetLookup(AirportList airportList)

        {
            Dictionary<string, AssetListItem> assetList = new Dictionary<string, AssetListItem>();

            int i = 0;
            foreach (var airport in airportList.Airports)
            {
                log.InfoFormat("Reading assets for airport {0} of {1}", i + 1, airportList.Airports.Count);

                var airportXPFullDirectory = DirectoryHelper.GetAirportXPFullDirectory(airport.AirportCode, XP2AFSConverterManager.Settings);
                var airportZipFilename = airportXPFullDirectory + @"\" + airport.AirportCode + ".zip";
                var airportAFSFullDirectory = DirectoryHelper.GetAirportAFSFullDirectory(airport.AirportCode, XP2AFSConverterManager.Settings);

                if (File.Exists(airportZipFilename))
                {
                    // Parse the DST and DAT files
                    var dsfFileLoader = new DSFFileLoader();
                    var dsfFile = dsfFileLoader.GetDSFFileFromXPZip(airport.AirportCode, airportZipFilename);

                    foreach (var obj in dsfFile.Objects)
                    {
                        if (!assetList.ContainsKey(obj.Reference))
                        {
                            var item = new AssetListItem();
                            item.Name = obj.Reference;
                            assetList.Add(obj.Reference, item);
                        }

                        assetList[obj.Reference].Count++;
                    }

                    foreach (var poly in dsfFile.Polygons)
                    {
                        if (!assetList.ContainsKey(poly.Reference))
                        {
                            var item = new AssetListItem();
                            item.Name = poly.Reference;
                            assetList.Add(poly.Reference, item);
                        }

                        assetList[poly.Reference].Count++;
                    }
                }

                i++;

                //if(i == 1000)
                //{
                //    break;
                //}
            }

            return assetList;


        }
    }
}
