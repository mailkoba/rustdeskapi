using System.Security.Claims;
using RustDeskApi.Services;

namespace RustDeskApi.Filters
{
    public class GetAccountMiddleware
    {
        private readonly RequestDelegate _next;

        public GetAccountMiddleware(RequestDelegate next)
        {
            _next = next;
        }

        public async Task Invoke(HttpContext context)
        {
            if (context?.User.Identity != null && context.User.Identity.IsAuthenticated)
            {
                try
                {
                    if (!Guid.TryParse(context.User.FindFirstValue(ApplicationConstants.Claims.UserId),
                                       out var userId))
                    {
                        throw new Exception("Token does not contains user id!");
                    }

                    var storageService = context.RequestServices.GetRequiredService<IStorageService>();

                    var user = storageService.GetUserById(userId);

                    if (user == null)
                    {
                        throw new Exception($"User not found by id = '{userId:D}'");
                    }

                    context.Items[ApplicationConstants.AccountKey] = user;
                    context.Items[ApplicationConstants.UserId] = userId;
                }
                catch (Exception e)
                {
                    context.RequestServices
                           .GetRequiredService<ILogger<GetAccountMiddleware>>()
                           .LogError(e, e.Message);
                }
            }

            await _next(context);
        }
    }
}
