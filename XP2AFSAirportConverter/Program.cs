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
                log.Info("Usage: XP2AFSAirportConverter <command> <options>");
                log.Info("");
                log.Info("XP2AFSAirportConverter getlist");
                log.Info("XP2AFSAirportConverter download");
                log.Info("XP2AFSAirportConverter download overwrite");
                log.Info("XP2AFSAirportConverter convert all");
                log.Info("XP2AFSAirportConverter convert EGFF,KPDX");
                log.Info("XP2AFSAirportConverter getlist");
                log.Info("XP2AFSAirportConverter airportcsvlist");
            }
            else
            {
                xp2AFSConverterManager.RunActions(args);
            }

            Console.ReadKey();
        }
    }
}
