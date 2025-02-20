namespace CoreLink.WebApi.Models
{
    public class User
    {
        public Guid UserId;
        public required string Username;
        public required string Password;
    }
}
