using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace EvG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PlayersController : ControllerBase
    {
        private readonly IGameEngine _gameEngine;

        public PlayersController(IGameEngine gameEngine)
        {
            _gameEngine = gameEngine;
        }

        [HttpGet]
        public ActionResult<Player[]> Get()
        {
            return Ok(_gameEngine.Players);
        }

        [HttpPost]
        public ActionResult Post([FromBody]RemotePlayer player)
        {
            _gameEngine.AddOrUpdatePlayer(player);
            return Ok();
        }
    }
}