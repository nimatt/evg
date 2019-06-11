using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvG.Controllers
{
    [ApiController]
    public class AdminController : Controller
    {
        private readonly IGameEngine _gameEngine;

        public AdminController(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        [HttpPost("api/[controller]/update-score")]
        public ActionResult UpdateScore([FromBody]RemotePlayer player)
        {
            _gameEngine.UpdatePlayerScore(player);
            return Ok();
        }

        [HttpPut("api/[controller]/game-config")]
        public ActionResult SetGameConfig([FromBody]GameConfigInput config)
        {
            if (config == null)
                return Ok();

            if (config.GameValue != null)
                _gameEngine.GameConfig.GameValue = (int)config.GameValue;

            if (config.ActionDelay != null)
                _gameEngine.GameConfig.ActionDelay = (int)config.ActionDelay;

            if (config.ForceMove != null)
                _gameEngine.GameConfig.ForceMove = (bool)config.ForceMove;

            if (config.BloodLust != null)
                _gameEngine.GameConfig.BloodLust = (bool)config.BloodLust;

            if (config.Fog != null)
                _gameEngine.GameConfig.Fog = (bool)config.Fog;

            if (config.RandomOrder != null)
                _gameEngine.GameConfig.RandomOrder = (bool)config.RandomOrder;

            if (config.StaticOrder != null)
                _gameEngine.GameConfig.RandomOrder = (bool)config.StaticOrder;

            if (config.PlayerTimeout != null)
                _gameEngine.SetPlayerTimeout((float)config.PlayerTimeout);


            return Ok();
        }
    }
}