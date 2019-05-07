using EvG.Models;
using Moq;
using NUnit.Framework;

namespace Tests
{
    public class GameEngineTests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void DefaultConfig()
        {
            var engine = new GameEngine();
            Assert.AreEqual(1, engine.GameConfig.GameValue);
            Assert.AreEqual(400, engine.GameConfig.ActionDelay);
            Assert.IsFalse(engine.GameConfig.Fog);
            Assert.IsFalse(engine.GameConfig.BloodLust);
            Assert.IsFalse(engine.GameConfig.ForceMove);
            Assert.IsFalse(engine.GameConfig.RandomOrder);
            Assert.IsFalse(engine.GameConfig.StaticOrder);
        }

        [Test]
        public void NoGameCreatedWhenNoPlayersRegistered()
        {
            var engine = new GameEngine();
            Assert.IsNull(engine.CurrentGame);
            engine.NewGame(new Mock<GameSpec>().Object);
            Assert.IsNull(engine.CurrentGame);
        }

        [Test]
        public void NoPlayersCreatedByDefault()
        {
            var engine = new GameEngine();
            Assert.IsNotNull(engine.Players);
            Assert.AreEqual(0, engine.Players.Count);
        }

        [Test]
        public void PossibleToCreatePlayer()
        {
            var engine = new GameEngine();
            engine.AddOrUpdatePlayer(new Player
            {
                Id = "Test",
                Name = "Test",
                Score = 0
            });
            Assert.IsNotNull(engine.Players);
            Assert.AreEqual(1, engine.Players.Count);
            Assert.AreEqual("Test", engine.Players[0].Id);
            Assert.AreEqual("Test", engine.Players[0].Name);
        }

        [Test]
        public void NotPossibleToSetScoreWhenCreatingPlayer()
        {
            var engine = new GameEngine();
            engine.AddOrUpdatePlayer(new Player
            {
                Id = "Test",
                Name = "Test",
                Score = 5
            });
            Assert.IsNotNull(engine.Players);
            Assert.AreEqual(1, engine.Players.Count);
            Assert.AreEqual(0, engine.Players[0].Score);
        }

        [Test]
        public void PossibleToUpdatePlayerName()
        {
            var engine = new GameEngine();
            engine.AddOrUpdatePlayer(new Player
            {
                Id = "Test",
                Name = "Test",
                Score = 0
            });

            engine.AddOrUpdatePlayer(new Player
            {
                Id = "Test",
                Name = "New Name",
                Score = 0
            });
            Assert.IsNotNull(engine.Players);
            Assert.AreEqual(1, engine.Players.Count);
            Assert.AreEqual("Test", engine.Players[0].Id);
            Assert.AreEqual("New Name", engine.Players[0].Name);
        }
    }
}