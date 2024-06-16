
namespace RustDeskApi.Models
{
    public class UsersResponseModel
    {
        public int Total { get; set; }

        public PublicUser[] Data { get; set; }
    }
}
