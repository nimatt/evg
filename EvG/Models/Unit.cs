using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class Unit
    {
        public string Id { get; } = Guid.NewGuid().ToString();
        public string Type { get; set; }
        public int Health { get; set; } = 9;
        public int Power { get; private set; } = 3;
        public int X { get; set; }
        public int Y { get; set; }

        public void Move(Direction dir)
        {
            if (dir == Direction.Up)
                Y--;
            else if (dir == Direction.Down)
                Y++;
            else if (dir == Direction.Left)
                X--;
            else if (dir == Direction.Right)
                X++;
        }

        public void Attack(Unit target)
        {
            target.Health = Math.Max(target.Health - Power, 0);
        }
    }
}
