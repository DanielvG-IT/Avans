using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Api.Models
{
    public class FastApiWeatherRequirements
    {
        public required string Condition { get; set; }
        public required double Temperature { get; set; }
    }
}