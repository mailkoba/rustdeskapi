
namespace RustDeskApi.Services
{
    public interface IScopeProvider
    {
        Guid? UserId { get; }

        long? Id { get; }

        string Uuid { get; }
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

        public long? Id
        {
            get
            {
                if (_context.Items.TryGetValue(ApplicationConstants.Id, out var id) &&
                    id != null &&
                    long.TryParse(id.ToString(), out var idValue))
                {
                    return idValue;
                }

                return null;
            }
        }

        public string Uuid
        {
            get
            {
                if (_context.Items.TryGetValue(ApplicationConstants.Uuid, out var uuid) && uuid != null)
                {
                    return uuid.ToString();
                }

                return null;
            }
        }

        private readonly HttpContext _context;
    }
}
