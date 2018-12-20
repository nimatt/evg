using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace EvG.Controllers
{
    [Route("api/[controller]")]
    public class GameController : Controller
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IGameEngine GameEngine;
        private JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

        public GameController(IHttpContextAccessor httpContextAccessor, IGameEngine gameEngine)
        {
            _httpContextAccessor = httpContextAccessor;
            GameEngine = gameEngine;
        }

        [HttpGet]
        public async Task Get()
        {
            var response = _httpContextAccessor.HttpContext.Response;
            response.Headers.Add("Content-Type", "text/event-stream");

            EventHandler<GameEventArgs> createdHandler = async (object sender, GameEventArgs args) =>
            {
                await response.WriteAsync($"data: {{ \"type\": \"game-created\"}}\n\n");
                response.Body.Flush();
            };
            EventHandler<GameEventArgs> endedHandler = async (object sender, GameEventArgs args) =>
            {
                await response.WriteAsync($"data: {{ \"type\": \"game-ended\"}}\n\n");
            };

            EventHandler<PlayerEventArgs> playerHandler = async (object sender, PlayerEventArgs args) =>
            {
                await response.WriteAsync($"data: {{ \"type\": \"{args.EventType}\", \"player\": {JsonConvert.SerializeObject(args.Player, SerializationSettings)}}}\n\n");
            };
            GameEngine.OnGameCreated += createdHandler;
            GameEngine.OnGameEnded += endedHandler;
            GameEngine.OnPlayerCreated += playerHandler;
            GameEngine.OnPlayerUpdated += playerHandler;

            try
            {
                await new Task(() => { });
            }
            finally
            {
                GameEngine.OnGameCreated -= createdHandler;
                GameEngine.OnGameEnded -= endedHandler;
                GameEngine.OnPlayerCreated -= playerHandler;
                GameEngine.OnPlayerUpdated -= playerHandler;
            }
        }

        [HttpPost]
        public ActionResult Post([FromBody]string action)
        {
            if (action == "new")
            {
                GameEngine.NewGame(new GameSpec());
            }
            if (action == "start")
            {
                GameEngine.CurrentGame.Start();
            }
            return Ok();
        }
    }
}
