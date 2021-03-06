﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class GameConfig
    {
        public int ActionDelay { get; set; } = 400;
        public int GameValue { get; set; } = 1;
        public bool ForceMove { get; set; }
        public bool BloodLust { get; set; }
        public bool Fog { get; set; }
        public bool RandomOrder { get; set; }
        public bool StaticOrder { get; set; }
    }
}
