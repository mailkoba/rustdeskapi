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

                    if (!long.TryParse(context.User.FindFirstValue(ApplicationConstants.Claims.Id),
                                       out var id))
                    {
                        throw new Exception("Token does not contains id!");
                    }

                    var uuid = context.User.FindFirstValue(ApplicationConstants.Claims.Uuid);
                    if (string.IsNullOrWhiteSpace(uuid))
                    {
                        throw new Exception("Token does not contains user uuid!");
                    }

                    var storageService = context.RequestServices.GetRequiredService<IStorageService>();

                    var user = storageService.GetUserById(userId);

                    if (user == null)
                    {
                        throw new Exception($"User not found by id = '{userId:D}'");
                    }

                    context.Items[ApplicationConstants.AccountKey] = user;
                    context.Items[ApplicationConstants.UserId] = userId;
                    context.Items[ApplicationConstants.Id] = id;
                    context.Items[ApplicationConstants.Uuid] = uuid;
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
