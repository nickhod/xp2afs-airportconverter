using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;

namespace XP2AFSAirportConverter.Data.Models
{
    public class Region
    {
        [Key]
        public long RegionId { get; set; }
        public string Name { get; set; }
    }
}
