using System;
using Newtonsoft.Json;

namespace EoJSON {
    // Configuration myDeserializedClass = JsonConvert.DeserializeObject<Root>(myJsonResponse);
    public class Configuration {
        [JsonProperty("obs_port")]
        public int OBSPort { get; set; } = 1337;
        [JsonProperty("game_scene")]
        public string GameScene { get; set; } = "GameScene";
        [JsonProperty("menu_scene")]
        public string MenuScene { get; set; } = "MenuScene";
    }
 }
