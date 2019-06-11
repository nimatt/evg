using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;

namespace EvG.Models
{
    public class GameEngine: IGameEngine
    {
        public Game CurrentGame { get; private set; }
        public List<Player> Players { get; } = new List<Player>();
        public GameConfig GameConfig { get; set; } = new GameConfig();

        public event EventHandler<GameEventArgs> OnGameCreated;
        public event EventHandler<GameEventArgs> OnGameEnded;
        public event EventHandler<PlayerEventArgs> OnPlayerCreated;
        public event EventHandler<PlayerEventArgs> OnPlayerUpdated;

        private RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        private Stack<Player> playerStack = new Stack<Player>();

        public void NewGame(GameSpec spec)
        {
            if (Players.Count < 2)
                return;

            if (CurrentGame != null)
            {
                CurrentGame.OnGameEnded -= HandleGameEnding;
            }

            var player1 = DrawPlayer(null);
            var player2 = DrawPlayer(player1);
            if (player1 == null || player2 == null)
            {
                return;
            }

            CurrentGame = new Game(spec, player1, player2, GameConfig);
            CurrentGame.Spec.Players = new Player[] { player1, player2 };
            CurrentGame.OnGameEnded += HandleGameEnding;
            OnGameCreated?.Invoke(this, new GameEventArgs("game-created"));
        }

        public void AddOrUpdatePlayer(Player player)
        {
            var existing = Players.FirstOrDefault((p) => p.Id == player.Id);
            if (existing != null)
            {
                existing.Name = player.Name;
                OnPlayerUpdated?.Invoke(
                    this,
                    new PlayerEventArgs { EventType = "player-updated", Player = existing }
                );
            }
            else
            {
                player.Score = 0;
                Players.Add(player);
                OnPlayerCreated?.Invoke(
                    this,
                    new PlayerEventArgs { EventType = "player-created", Player = player }
                );
            }
        }

        public void UpdatePlayerScore(Player player)
        {
            var existing = Players.FirstOrDefault((p) => p.Id == player.Id);
            if (existing != null)
            {
                existing.Score = player.Score;
                OnPlayerUpdated?.Invoke(
                    this,
                    new PlayerEventArgs { EventType = "player-updated", Player = existing }
                );
            }
        }

        private void HandleGameEnding(object sender, GameEventArgs args)
        {
            var winner = CurrentGame.Winner;
            OnGameEnded?.Invoke(this, new GameEventArgs("game-ended") { Winner = winner });
            if (winner != null)
            {
                winner.Score += GameConfig.GameValue;
                OnPlayerUpdated?.Invoke(this, new PlayerEventArgs { EventType = "player-updated", Player = winner });
            }
        }

        private Player DrawPlayer(Player exclude)
        {
            if (playerStack.Count == 0)
            {
                var bytes = new byte[Players.Count];
                random.GetBytes(bytes);
                playerStack = new Stack<Player>(Players
                    .Zip(bytes, (player, order) => new { player, order })
                    .OrderBy(o => o.order)
                    .Select(o => o.player));

                if (exclude != null && playerStack.Count == 1 && playerStack.Peek() == exclude)
                {
                    return null;
                }
            }

            var draw = playerStack.Pop();
            if (exclude != null && exclude.Id == draw.Id)
            {
                var second = DrawPlayer(draw);
                playerStack.Push(draw);
                draw = second;
            }
            return draw;
        }

        public void SetPlayerTimeout(float playerTimeout)
        {
            if (Players != null)
            {
                Players.ForEach(player => player.SetTimeout(playerTimeout));
            }
        }
    }
}
