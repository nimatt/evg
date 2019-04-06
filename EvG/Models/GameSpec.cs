using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace EvG.Models
{
    public class GameSpec
    {
        private static readonly string MapBase = "./wwwroot/assets/maps/";
        private static readonly RNGCryptoServiceProvider random = new RNGCryptoServiceProvider();
        public dynamic Map { get; private set; }
        public string Name { get; }
        public string Tilemap { get; }
        public bool[][] FloorMap { get; private set; }
        public Unit[] Units { get; private set; }
        public Player[] Players { get; set; }
        public bool Active { get; set; }

        public GameSpec() : this(GetRandomMap()) { }

        public GameSpec(string map)
        {
            using (StreamReader reader = new StreamReader(MapBase + map + ".json"))
            using (var jsonReader = new JsonTextReader(reader))
            {
                var serializer = new JsonSerializer();
                dynamic game = serializer.Deserialize(jsonReader);
                Name = game.name;
                Tilemap = $"/assets/maps/{game.tilemap}";
                CreateMap(serializer, game);
                CreateUnits(game);
            }
        }

        private static string GetRandomMap()
        {
            var maps = Directory.GetFiles(MapBase, "*.json").Select(f => f.Substring(0, f.LastIndexOf('.'))).ToArray();
            var nums = new byte[2];
            random.GetBytes(nums);
            var map = maps[(nums[0] << 8 | nums[1]) % maps.Length];
            map = map.Split(new []{ '\\', '/' }).Last();
            return map;
        }

        private void CreateMap(JsonSerializer serializer, dynamic game)
        {
            using (StreamReader mapReader = new StreamReader(MapBase + game.tilemap))
            using (var jsonMapReader = new JsonTextReader(mapReader))
            {
                var imageBase = Tilemap.Substring(0, Tilemap.LastIndexOf('/') + 1);
                Map = serializer.Deserialize(jsonMapReader);
                for (var i = 0; i < Map.tilesets.Count; i++)
                {
                    Map.tilesets[i].image = imageBase + Map.tilesets[i].image;
                }
                BuildFloorMap(game);
            }
        }

        private void CreateUnits(dynamic game)
        {
            var random = new Random();
            int width = Map.width;
            int height = Map.height;
            HashSet<int> unitPositions = new HashSet<int>();
            Units = new Unit[game.units.Count];
            for (var i = 0; i < game.units.Count; i++)
            {
                var unit = game.units[i];
                Units[i] = new Unit()
                {
                    Type = game.units[i].type
                };

                int x = random.Next(0, width);
                int y = random.Next(0, width);
                while (!FloorMap[x][y] || unitPositions.Contains((x << 16) | y))
                {
                    x = random.Next(0, width);
                    y = random.Next(0, width);
                }
                unitPositions.Add((x << 16) | y);
                Units[i].X = x;
                Units[i].Y = y;
            }
        }

        private void BuildFloorMap(dynamic game)
        {
            var floorData = Map.layers[0].data;
            int mapWidth = Map.width;
            FloorMap = new bool[mapWidth][];
            for (var i = 0; i < mapWidth; i++)
            {
                FloorMap[i] = new bool[Map.height];
            }
            HashSet<int> openTiles = new HashSet<int>();
            foreach (int openTile in game.openTiles)
            {
                openTiles.Add(openTile);
            }
            for (var i = 0; i < floorData.Count; i++)
            {
                FloorMap[i % mapWidth][i / mapWidth] = openTiles.Contains((int)floorData[i]);
            }
        }
    }
}
