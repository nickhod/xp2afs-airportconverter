using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace XP2AFSAirportConverter.Data.Models
{
    public class City
    {
        [Key]
        public long CityId { get; set; }

        public string Name { get; set; }
    }
}
