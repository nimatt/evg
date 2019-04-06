using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class Game
    {
        public static readonly int MaxRounds = 30;

        public GameSpec Spec { get; }
        public Unit[] Units { get; private set; }
        public event EventHandler<MoveEventArgs> OnUnitMoved;
        public event EventHandler<AttackEventArgs> OnUnitAttacked;
        public event EventHandler<DeathEventArgs> OnUnitDied;
        public event EventHandler OnMaxRounds;
        public event EventHandler<GameEventArgs> OnGameEnded;
        public GameConfig GameConfig { get; set; }
        public Player[] Players { get; }
        public Player Winner
        {
            get
            {
                var unitGroups = Units.Where((u) => u.Health > 0).GroupBy((u) => u.Type).ToArray();
                if (round < MaxRounds)
                {
                    return unitGroups.Length == 1 ? PlayerLookup[unitGroups[0].ToArray()[0].Id] : null;
                }
                else
                {
                    var groupHealth = new int[unitGroups.Length];
                    for (var i = 0; i < unitGroups.Length; i++)
                    {
                        foreach (var unit in unitGroups[i])
                        {
                            groupHealth[i] += unit.Health;
                        }
                    }
                    if (groupHealth[0] > groupHealth[1])
                    {
                        return PlayerLookup[unitGroups[0].ToArray()[0].Id];
                    }
                    else if (groupHealth[0] < groupHealth[1])
                    {
                        return PlayerLookup[unitGroups[1].ToArray()[0].Id];
                    }

                    return null;
                }
            }
        }

        private Task Updater;
        private Dictionary<string, Player> PlayerLookup = new Dictionary<string, Player>();
        private int round = 0;

        public Game(GameSpec spec, Player player1, Player player2, GameConfig gameConfig)
        {
            GameConfig = gameConfig;
            Spec = spec;
            Units = spec.Units.Select((u) => u).ToArray();
            foreach (var unit in Units)
            {
                PlayerLookup.Add(unit.Id, unit.Type == Units[0].Type ? player1 : player2);
            }
            Players = new Player[] { player1, player2 };
        }

        public void Start()
        {
            Spec.Active = true;
            Updater = new Task(async () =>
            {
                while (Winner == null && round < MaxRounds)
                {
                    var movingUnits = Units
                        .Where((u) => u.Health > 0)
                        .OrderBy((u) => (u.Y << 8) | u.X);
                    foreach (var unit in movingUnits)
                    {
                        if (unit.Health <= 0)
                            continue;

                        var actions = await PlayerLookup[unit.Id].GetActions(
                            this,
                            unit,
                            Units.Where((u) => u.Type == unit.Type).ToArray(),
                            Units.Where((u) => u.Type != unit.Type).ToArray()
                        );
                        var performedActions = new HashSet<ActionType>();
                        foreach (var action in actions)
                        {
                            if (!performedActions.Contains(action.Type) ||
                               (action.Type == ActionType.Move && GameConfig.ForceMove && unit.Health > 1))
                            {
                                if (performedActions.Contains(action.Type) && action.Type == ActionType.Move && GameConfig.ForceMove)
                                {
                                    unit.Health--;
                                }
                                PerformAction(action, unit);
                                performedActions.Add(action.Type);
                                await Task.Delay(500);
                            }
                        }
                    }
                    round++;
                }
                if (round == MaxRounds)
                {
                    OnMaxRounds?.Invoke(this, new EventArgs());
                }
                OnGameEnded?.Invoke(this, new GameEventArgs("game-ended") { Winner = Winner });
                foreach (var player in Players)
                {
                    var playerUnit = Units.First(u => PlayerLookup[u.Id] == player);
                    try
                    {
                        await player.GameEnded(
                            this,
                            Units.Where(u => u.Type == playerUnit.Type).ToArray(),
                            Units.Where(u => u.Type != playerUnit.Type).ToArray()
                        );
                    } catch { /* We dont' care */ }
                }
                Spec.Active = false;
            });
            Updater.Start();
        }

        private void PerformAction(Action action, Unit unit)
        {
            switch (action.Type)
            {
                case ActionType.Move:
                    if (CanMove(unit, action.Direction))
                    {
                        unit.Move(action.Direction);
                        OnUnitMoved?.Invoke(this, new MoveEventArgs { Unit = unit });
                    }
                    break;
                case ActionType.Attack:
                    Unit target = GetTarget(unit, action);
                    if (target != null)
                    {
                        unit.Attack(target);
                        if (target.Health <= 0)
                        {
                            OnUnitDied?.Invoke(this, new DeathEventArgs { Unit = target });
                        }
                    }
                    OnUnitAttacked?.Invoke(this, new AttackEventArgs { Unit = unit, Target = target });
                    break;
            }
        }

        private Unit GetTarget(Unit unit, Action attackAction)
        {
            var targetX = unit.X;
            var targetY = unit.Y;
            switch (attackAction.Direction)
            {
                case Direction.Down:
                    targetY++;
                    break;
                case Direction.Left:
                    targetX--;
                    break;
                case Direction.Right:
                    targetX++;
                    break;
                case Direction.Up:
                    targetY--;
                    break;
            }
            var target = Units.FirstOrDefault((u) => u.Health > 0 && u.X == targetX && u.Y == targetY);
            return target;
        }

        private bool CanMove(Unit unit, Direction dir)
        {
            switch (dir)
            {
                case Direction.Down:
                    return IsOpen(unit.X, unit.Y + 1);
                case Direction.Left:
                    return IsOpen(unit.X - 1, unit.Y);
                case Direction.Right:
                    return IsOpen(unit.X + 1, unit.Y);
                case Direction.Up:
                    return IsOpen(unit.X, unit.Y - 1);
                default:
                    return false;
            }
        }

        private bool IsOpen(int x, int y)
        {
            return Spec.FloorMap[x][y] && !Units.Any((u) => u.X == x && u.Y == y && u.Health > 0);
        }
    }
}
