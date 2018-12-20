using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EvG.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.IO;
using Newtonsoft.Json;

namespace EvG.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UnitsController : ControllerBase
    {
        [HttpGet]
        public ActionResult<UnitSpec[]> Get()
        {
            var files = Directory.GetFiles("./wwwroot/assets/units", "*.json");

            return Ok(files.Select((file) =>
            {
                using (var reader = new StreamReader(file))
                using (var jsonReader = new JsonTextReader(reader))
                {
                    var serializer = new JsonSerializer();
                    var unitSpec = serializer.Deserialize<UnitSpec>(jsonReader);
                    unitSpec.SpriteMap = "/assets/units/" + unitSpec.SpriteMap;
                    return unitSpec;
                }
            }).ToArray());
        }
    }
}