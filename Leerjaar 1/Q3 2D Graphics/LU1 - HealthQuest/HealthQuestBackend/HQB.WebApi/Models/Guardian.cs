namespace HQB.WebApi.Models;
public class Guardian
{
    public Guid ID { get; set; }
    public required string FirstName { get; set; }
    public required string LastName { get; set; }
    public required string UserID { get; set; }  // Foreign key to auth_AspNetUsers
}