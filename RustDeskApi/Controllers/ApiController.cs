using System.Text.Json;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RustDeskApi.Models;
using RustDeskApi.Services;

namespace RustDeskApi.Controllers
{
    [ApiController]
    public class ApiController : ControllerBase
    {
        public ApiController(ILogger logger,
                             IScopeProvider scopeProvider)
        {
            _logger = logger;
            _scopeProvider = scopeProvider;
        }

        [HttpPost]
        [Route("api/login")]
        [AllowAnonymous]
        public IActionResult Login(LoginModel loginModel,
                                   [FromServices] IAuthenticateService authenticateService)
        {
            try
            {
                return Ok(authenticateService.Login(loginModel));
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/login-options")]
        [AllowAnonymous]
        public IActionResult LoginOptions()
        {
            try
            {
                return Ok(Array.Empty<string>());
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("api/logout")]
        public IActionResult Logout(LogoutModel logoutModel)
        {
            try
            {
                CheckSecurity(logoutModel.Id, logoutModel.Uuid);

                return Ok();
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return Unauthorized();
            }
        }

        [HttpGet]
        [Route("api/users")]
        public IActionResult GetUsers([FromServices] IStorageService storageService,
                                      [FromQuery] int current = 1,
                                      [FromQuery] int pageSize = 100,
                                      [FromQuery] string accessible = "",
                                      [FromQuery] int status = 1)
        {
            try
            {
                var users = storageService.GetAllUsers();

                return Ok(new UsersResponseModel
                {
                    Total = users.Length,
                    Data = users.Skip(pageSize * current - 1)
                                .Take(pageSize)
                                .Select(x => new PublicUser
                                {
                                    Name = x.Name,
                                    IsAdmin = true
                                })
                                .ToArray()
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("api/currentUser")]
        public IActionResult GetCurrentUser(CurrentUserModel logoutModel,
                                            [FromServices] IStorageService storageService)
        {
            try
            {
                CheckSecurity(logoutModel.Id, logoutModel.Uuid);

                return Ok(new UserModel
                {
                    Name = storageService.GetUserById(_scopeProvider.UserId.Value).Name
                });
            }
            catch (Exception e)
            {
                _logger.LogError(e, e.Message);

                return Unauthorized();
            }
        }

        [HttpPost]
        [Route("api/ab/get")]
        public AbModel GetAbByPost([FromServices] IStorageService storageService)
        {
            storageService.GetUserTagsAndPeers(_scopeProvider.UserId.Value, out var tags, out var peers);

            return new AbModel
            {
                Data = JsonSerializer.Serialize(new AbData
                {
                    Tags = tags,
                    Peers = peers
                })
            };
        }

        [HttpGet]
        [Route("api/ab")]
        public AbModel GetAbByGet([FromServices] IStorageService storageService)
        {
            storageService.GetUserTagsAndPeers(_scopeProvider.UserId.Value, out var tags, out var peers);

            return new AbModel
            {
                Data = JsonSerializer.Serialize(new AbData
                {
                    Tags = tags,
                    Peers = peers
                })
            };
        }

        [HttpPost]
        [Route("api/ab")]
        public IActionResult PostAb(AbModel abModel,
                                    [FromServices] IStorageService storageService)
        {
            var abData = JsonSerializer.Deserialize<AbData>(abModel.Data);

            storageService.UpdateUserTagsAndPeers(_scopeProvider.UserId.Value,
                                                  abData.Tags,
                                                  abData.Peers);

            return Ok();
        }

        private readonly ILogger _logger;
        private readonly IScopeProvider _scopeProvider;

        private void CheckSecurity(long id, string uuid)
        {
            if (!_scopeProvider.Id.HasValue || _scopeProvider.Id.Value != id)
            {
                throw new Exception("Not matched id!");
            }

            if (string.IsNullOrWhiteSpace(_scopeProvider.Uuid) ||
                string.IsNullOrWhiteSpace(uuid) ||
               !_scopeProvider.Uuid.Equals(uuid, StringComparison.InvariantCultureIgnoreCase))
            {
                throw new Exception("Not matched uuid!");
            }
        }
    }
}
