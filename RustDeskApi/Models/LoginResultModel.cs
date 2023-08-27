using System.Text.Json.Serialization;

namespace RustDeskApi.Models
{
    public class LoginResultModel
    {
        public string Error { get; set; }

        [JsonPropertyName("access_token")]
        public string AccessToken { get; set; }

        public string Type { get; set; } = "access_token";

        public UserModel User { get; set; }
    }
}
