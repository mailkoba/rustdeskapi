using System.Text.Json.Serialization;

namespace RustDeskApi.Models
{
    public class PeerModel
    {
        [JsonPropertyName("id")]
        public string Id { get; set; }

        [JsonPropertyName("username")]
        public string Username { get; set; }

        [JsonPropertyName("hostname")]
        public string Hostname { get; set; }

        [JsonPropertyName("platform")]
        public string Platform { get; set; }

        [JsonPropertyName("alias")]
        public string Alias { get; set; }

        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}
