using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class UnitAnimations
    {
        public AnimationSpec Right { get; set; }
        public AnimationSpec Left { get; set; }
        public AnimationSpec HurtRight { get; set; }
        public AnimationSpec HurtLeft { get; set; }
        public AnimationSpec AttackLeft { get; set; }
        public AnimationSpec AttackRight { get; set; }
        public AnimationSpec AttackUp { get; set; }
        public AnimationSpec AttackDown { get; set; }
        public AnimationSpec DeathRight { get; set; }
        public AnimationSpec DeathLeft   { get; set; }
    }
}
