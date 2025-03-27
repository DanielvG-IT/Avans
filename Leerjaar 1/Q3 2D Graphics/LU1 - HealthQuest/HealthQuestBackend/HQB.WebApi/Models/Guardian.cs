namespace HQB.WebApi.Models;
/// <summary>
/// Represents a guardian entity.
/// </summary>
public class Guardian
{
    /// <summary>
    /// Gets or sets the unique identifier for the guardian.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the first name of the guardian.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the guardian.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with the guardian.
    /// This is a foreign key to the auth_AspNetUsers table.
    /// </summary>
    public string? UserID { get; set; }
}