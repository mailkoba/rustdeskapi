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
        public ApiController(ILogger logger)
        {
            _logger = logger;
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

        [HttpPost]
        [Route("api/logout")]
        public IActionResult Logout(LogoutModel logoutModel)
        {
            return Ok();
        }

        [HttpPost]
        [Route("api/currentUser")]
        public UserModel GetCurrentUser(LogoutModel logoutModel)
        {
            return new UserModel
            {
                Name = logoutModel.Id.ToString()
            };
        }

        [HttpPost]
        [Route("api/ab/get")]
        public AbModel GetAb([FromServices] IStorageService storageService,
                             [FromServices] IScopeProvider scopeProvider)
        {
            storageService.GetUserTagsAndPeers(scopeProvider.UserId.Value, out var tags, out var peers);

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
                                    [FromServices] IStorageService storageService,
                                    [FromServices] IScopeProvider scopeProvider)
        {
            var abData = JsonSerializer.Deserialize<AbData>(abModel.Data);

            storageService.UpdateUserTagsAndPeers(scopeProvider.UserId.Value,
                                                  abData.Tags,
                                                  abData.Peers);

            return Ok();
        }

        private readonly ILogger _logger;
    }
}