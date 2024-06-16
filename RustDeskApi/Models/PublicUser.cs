namespace RustDeskApi.Models
{
    public class PublicUser
    {
        public string Name { get; set; }

        public string Email { get; set; }

        public string Note { get; set; }

        public int Status { get; set; } = 1;

        public bool IsAdmin { get; set; }
    }
}
