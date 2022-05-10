using Newtonsoft.Json;

namespace CarService.WebAPI.SeedData
{
    public class UpdateCarForm
    {
        [JsonProperty("id")]
        public int Id { get; set; }

        [JsonProperty("make")]
        public string Make { get; set; }

        [JsonProperty("model")]
        public string Model { get; set; }

        [JsonProperty("price")]
        public uint Price { get; set; }

        [JsonProperty("year")]
        public uint Year { get; set; }
    }
}
