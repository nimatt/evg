using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class GameConfigInput
    {
        public int? ActionDelay { get; set; }
        public int? GameValue { get; set; }
        public bool? ForceMove { get; set; }
        public bool? BloodLust { get; set; }
        public bool? Fog { get; set; }
        public bool? RandomOrder { get; set; }
        public bool? StaticOrder { get; set; }
        public float? PlayerTimeout { get; set; }
    }
}
