using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class PlayerEventArgs: EventArgs
    {
        public string EventType { get; set; }
        public Player Player { get; set; }
    }
}
