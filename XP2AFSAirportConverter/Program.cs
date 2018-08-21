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

            var xp2AFSConverterManager = new XP2AFSConverterManager();
            xp2AFSConverterManager.RunActions(args);

            Console.ReadKey();
        }
    }
}
