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
        public ActionResult SetGameConfig([FromBody]GameConfig config)
        {
            _gameEngine.GameConfig.GameValue = config.GameValue;
            _gameEngine.GameConfig.ForceMove = config.ForceMove;
            return Ok();
        }
    }
}