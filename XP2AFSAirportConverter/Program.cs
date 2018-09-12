using log4net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using XP2AFSAirportConverter.Common;

namespace XP2AFSAirportConverter
{
    public class Program
    {

        static void Main(string[] args)
        {
            log4net.Config.XmlConfigurator.Configure();

            var log = LogManager.GetLogger("XP2AFSAirportConverter");

            var xp2AFSConverterManager = new XP2AFSConverterManager();

            if (args.Length == 0)
            {
                Console.WriteLine("Usage: XP2AFSAirportConverter <command> <options>");
                Console.WriteLine("");
                Console.WriteLine("-- Get a csv list of all available airports");
                Console.WriteLine("XP2AFSAirportConverter getlist");
                Console.WriteLine("");
                Console.WriteLine("-- Download files for specified airports");
                Console.WriteLine("XP2AFSAirportConverter download");
                Console.WriteLine("XP2AFSAirportConverter download EGFF,KPDX");
                Console.WriteLine("XP2AFSAirportConverter download \"C:\\Temp\\icao.txt\"");
                Console.WriteLine("XP2AFSAirportConverter download overwrite");
                Console.WriteLine("");
                Console.WriteLine("-- Generate render script for specified airports");
                Console.WriteLine("XP2AFSAirportConverter generaterenderscripts EGFF,KPDX");
                Console.WriteLine("XP2AFSAirportConverter generaterenderscripts \"C:\\Temp\\icao.txt\"");
                Console.WriteLine("");
                Console.WriteLine("-- Run render scripts for specified airports");
                Console.WriteLine("XP2AFSAirportConverter runrenderscripts EGFF,KPDX");
                Console.WriteLine("XP2AFSAirportConverter runrenderscripts \"C:\\Temp\\icao.txt\"");
                Console.WriteLine("");
                Console.WriteLine("-- Build specified airports");
                Console.WriteLine("XP2AFSAirportConverter buildairports EGFF,KPDX");
                Console.WriteLine("XP2AFSAirportConverter buildairports \"C:\\Temp\\icao.txt\"");
                Console.WriteLine("");
                Console.WriteLine("-- Upload specified airports");
                Console.WriteLine("XP2AFSAirportConverter uploadairports EGFF,KPDX");
                Console.WriteLine("XP2AFSAirportConverter uploadairports \"C:\\Temp\\icao.txt\"");
                Console.WriteLine("");
                Console.WriteLine("-- Generate a csv list of all relevant airports");
                Console.WriteLine("XP2AFSAirportConverter airportcsvlist");
                Console.WriteLine("XP2AFSAirportConverter importairportcsvlist");
            }
            else
            {
                xp2AFSConverterManager.RunActions(args);
            }

            Console.ReadKey();
        }
    }
}
