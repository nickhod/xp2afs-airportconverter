using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XP2AFSAirportConverter.Common
{
    public static class DoubleExtensions
    {
        public static string ToStringInvariant(this double value)
        {
            return value.ToString("0.################", CultureInfo.InvariantCulture);
        }
    }
}
