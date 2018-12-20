using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class GameEventArgs: EventArgs
    {
        public string EventType { get; set; }

        public Player Winner { get; set; }

        public GameEventArgs(string eventType)
        {
            EventType = eventType;
        }
    }
}
