using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class RemotePlayer: Player
    {
        public string Address { get; set; }

        private HttpClient _client;

        private JsonSerializerSettings SerializationSettings = new JsonSerializerSettings
        {
            ContractResolver = new DefaultContractResolver
            {
                NamingStrategy = new CamelCaseNamingStrategy()
            },
            Formatting = Formatting.None
        };

        public RemotePlayer()
        {
            _client = new HttpClient();
            _client.Timeout = TimeSpan.FromSeconds(1);
        }

        public override async Task<Action[]> GetActions(Game game, Unit unit, Unit[] units, Unit[] foes)
        {
            var data = JsonConvert.SerializeObject(new
            {
                game.Spec.FloorMap,
                Unit = unit,
                Units = units,
                Foes = foes
            }, SerializationSettings);

            try
            {
                var response = await _client.PostAsync(Address, new StringContent(data));
                if (response.IsSuccessStatusCode)
                {
                    using (var rawReader = new StreamReader(await response.Content.ReadAsStreamAsync()))
                    using (var reader = new JsonTextReader(rawReader))
                    {
                        var serializer = new JsonSerializer();
                        return serializer.Deserialize<Action[]>(reader);
                    }
                }
                else
                {
                    return new Action[0];
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                return new Action[0];
            }
        }

        public override async Task GameEnded(Game game, Unit[] units, Unit[] foes)
        {
            var data = JsonConvert.SerializeObject(new
            {
                game.Winner,
                Units = units,
                Foes = foes
            }, SerializationSettings);

            await _client.PostAsync(Address + "/end", new StringContent(data));
        }
    }
}
