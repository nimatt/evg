using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class AnimationSpec
    {
        public int Start { get; set; }
        public int End { get; set; }
        public int FrameRate { get; set; }
        public int Repeat { get; set; }
    }
}
