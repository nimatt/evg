using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvG.Models;
using Microsoft.AspNetCore.Mvc;

namespace EvG.Controllers
{
    [Route("api/game/[controller]")]
    public class SpecController : Controller
    {
        private readonly IGameEngine GameEngine;

        public SpecController(IGameEngine gameEngine)
        {
            GameEngine = gameEngine;
        }

        [HttpGet]
        public ActionResult<GameSpec> Get()
        {
            if (GameEngine.CurrentGame == null)
            {
                return Ok(null);
            }

            return Ok(GameEngine.CurrentGame.Spec);
        }
    }
}
