﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class AttackEventArgs: EventArgs
    {
        public Unit Unit;
        public Unit Target;
    }
}
