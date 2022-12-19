
namespace RustDeskApi.Services
{
    public interface IScopeProvider
    {
        Guid? UserId { get; }
    }

    public class ScopeProvider : IScopeProvider
    {
        public ScopeProvider(IHttpContextAccessor httpContextAccessor)
        {
            _context = httpContextAccessor.HttpContext;
        }

        public Guid? UserId
        {
            get
            {
                if (_context.Items.TryGetValue(ApplicationConstants.UserId, out var userId) &&
                    userId != null &&
                    Guid.TryParse(userId.ToString(), out var userIdValue))
                {
                    return userIdValue;
                }

                return null;
            }
        }

        private readonly HttpContext _context;
    }
}
