
namespace RustDeskApi.Domain
{
    public class UserPeer
    {
        public Guid UserId { get; set; }
        public string PeerId { get; set; }

        public string UserName { get; set; }

        public string HostName { get; set; }

        public string Platform { get; set; }

        public string Alias { get; set; }

        public string[] Tags { get; set; } = Array.Empty<string>();
    }
}
