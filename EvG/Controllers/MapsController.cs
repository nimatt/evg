using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace EvG.Controllers
{
    [Route("api/[controller]")]
    public class MapsController : Controller
    {
        [HttpGet]
        public string Get()
        {
            using (StreamReader sr = new StreamReader("./wwwroot/assets/maps/test.json"))
            {
                return sr.ReadToEnd();
            };
        }
    }
}
