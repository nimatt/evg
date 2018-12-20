using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class UnitSpec
    {
        public string Name { get; set; }
        public string SpriteMap { get; set; }
        public int TileHeight { get; set; }
        public int TileWidth { get; set; }
        public float Scale { get; set; }
        public float HealthBarOffset { get; set; }
        public UnitAnimations Animations { get; set; }
    }
}
