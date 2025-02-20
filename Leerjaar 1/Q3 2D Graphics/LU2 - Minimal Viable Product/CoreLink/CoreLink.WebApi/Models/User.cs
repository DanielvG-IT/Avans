namespace CoreLink.WebApi.Models;
/// <summary>
/// Represents a user in the system.
/// </summary>
public class User
{
    /// <summary>
    /// Gets or sets the email of the user.
    /// </summary>
    public required string email;

    /// <summary>
    /// Gets or sets the password of the user.
    /// </summary>
    public required string password;
}