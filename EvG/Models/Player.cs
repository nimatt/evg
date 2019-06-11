using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class Player
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public int Score { get; set; }

        public virtual void SetTimeout(float timeout)
        {
            return;
        }

        public virtual Task<Action[]> GetActions(Game game, Unit unit, Unit[] units, Unit[] foes)
        {
            return Task.FromResult(new Action[0]);
        }

        public virtual Task GameEnded(Game game, Unit[] units, Unit[] foes)
        {
            return Task.CompletedTask;
        }
    }
}
