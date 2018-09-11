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
                Console.WriteLine("XP2AFSAirportConverter getlist");
                Console.WriteLine("XP2AFSAirportConverter download");
                Console.WriteLine("XP2AFSAirportConverter download overwrite");
                Console.WriteLine("XP2AFSAirportConverter convert all");
                Console.WriteLine("XP2AFSAirportConverter convert EGFF,KPDX");
                Console.WriteLine("XP2AFSAirportConverter getlist");
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
