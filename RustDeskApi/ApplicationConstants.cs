using Microsoft.IdentityModel.Tokens;
using System.Text;

namespace RustDeskApi
{
    internal static class ApplicationConstants
    {
        public const string AccountKey = "account";
        public const string UserId = "userId";

        public class Jwt
        {
            public const string Issuer = "RustDeskApi";
            public const string Audience = "RustDeskApi.Client";

            public static readonly SymmetricSecurityKey SigningKey =
                new(Encoding.ASCII.GetBytes("359BC904-AF39-4E93-84D4-FDD996C4BBD8"));
        }

        public static class Claims
        {
            public const string UserId = "api-user-id";
        }
    }
}
