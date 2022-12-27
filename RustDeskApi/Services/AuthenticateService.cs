using System.IdentityModel.Tokens.Jwt;
using Microsoft.Extensions.Options;
using RustDeskApi.Models;
using RustDeskApi.Settings;
using System.Security.Claims;
using Microsoft.IdentityModel.Tokens;

namespace RustDeskApi.Services
{
    public interface IAuthenticateService
    {
        LoginResultModel Login(LoginModel loginModel);
    }

    public class AuthenticateService : IAuthenticateService
    {
        public AuthenticateService(IStorageService storageService,
                                   IOptions<ApiSettings> settings,
                                   ILogger logger)
        {
            _storageService = storageService;
            _apiSettings = settings.Value;
            _logger = logger;
        }

        public LoginResultModel Login(LoginModel loginModel)
        {
            try
            {
                return LoginInternal(loginModel);
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return new LoginResultModel
                {
                    Error = e.Message
                };
            }
        }

        private readonly IStorageService _storageService;
        private readonly ApiSettings _apiSettings;
        private readonly ILogger _logger;

        private LoginResultModel LoginInternal(LoginModel loginModel)
        {
            if (loginModel == null)
            {
                throw new ArgumentNullException(nameof(loginModel));
            }

            if (string.IsNullOrWhiteSpace(loginModel.Username))
            {
                throw new ArgumentNullException(nameof(loginModel.Username));
            }

            if (string.IsNullOrWhiteSpace(loginModel.Password))
            {
                throw new ArgumentNullException(nameof(loginModel.Password));
            }

            var user = _apiSettings.Users
                                   .FirstOrDefault(x => x.Login.Equals(loginModel.Username,
                                                                       StringComparison.InvariantCultureIgnoreCase));

            if (user == null)
            {
                throw new Exception($"User {loginModel.Password} not found!");
            }

            if (!user.Password.Equals(loginModel.Password))
            {
                throw new Exception($"Wrong password for user {loginModel.Password}!");
            }

            var dbUser = _storageService.GetOrCreateUser(user.Login);

            return new LoginResultModel
            {
                AccessToken = CreateToken(dbUser.Id.ToString("D"), user.Login, loginModel.Id, loginModel.Uuid),
                User = new UserModel
                {
                    Name = user.Login
                }
            };
        }

        private string CreateToken(string userId, string login, long id, string uuid)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var ticket = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ApplicationConstants.Claims.UserId, userId),
                    new Claim(ApplicationConstants.Claims.Id, id.ToString()),
                    new Claim(ApplicationConstants.Claims.Uuid, uuid),
                    new Claim(ClaimTypes.Name, login)
                }),
                IssuedAt = DateTime.UtcNow,
                Expires = new DateTime(9999, 1, 1),
                Issuer = ApplicationConstants.Jwt.Issuer,
                Audience = ApplicationConstants.Jwt.Audience,
                SigningCredentials = new SigningCredentials(ApplicationConstants.Jwt.SigningKey,
                                                            SecurityAlgorithms.HmacSha256Signature)
            };

            var token = tokenHandler.CreateToken(ticket);

            return tokenHandler.WriteToken(token);
        }
    }
}
