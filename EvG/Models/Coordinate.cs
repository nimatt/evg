using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class Coordinate
    {
        public int X { get; set; }
        public int Y { get; set; }

        public Coordinate(int x, int y)
        {
            X = x;
            Y = y;
        }

        public override bool Equals(object obj)
        {
            var coord = obj as Coordinate;
            if (coord == null)
            {
                return false;
            }

            return X == coord.X && Y == coord.Y;
        }

        public override int GetHashCode()
        {
            return X << 8 + Y;
        }
    }
}
