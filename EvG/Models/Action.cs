using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class Action
    {
        public ActionType Type { get; set; }
        public Direction Direction { get; set; }

        public Action() { }

        public Action(ActionType type, Direction dir)
        {
            Type = type;
            Direction = dir;
        }
    }
}
