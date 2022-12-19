using System.Text.Json.Serialization;

namespace RustDeskApi.Models
{
    public class AbModel
    {
        [JsonPropertyName("data")]
        public string Data { get; set; }
    }

    public class AbData
    {
        [JsonPropertyName("tags")]
        public string[] Tags { get; set; } = Array.Empty<string>();

        [JsonPropertyName("peers")]
        public PeerModel[] Peers { get; set; } = Array.Empty<PeerModel>();
    }
}
