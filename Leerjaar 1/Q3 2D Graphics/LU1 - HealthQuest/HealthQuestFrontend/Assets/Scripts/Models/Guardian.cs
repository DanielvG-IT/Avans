using System;

/// <summary>
/// Represents a guardian entity.
/// </summary>
[Serializable]
public class Guardian
{
    /// <summary>
    /// Gets or sets the unique identifier for the guardian.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the first name of the guardian.
    /// </summary>
    public string FirstName;

    /// <summary>
    /// Gets or sets the last name of the guardian.
    /// </summary>
    public string LastName;

    /// <summary>
    /// Gets or sets the user ID associated with the guardian.
    /// This is a foreign key to the auth_AspNetUsers table.
    /// </summary>
    public string UserID;
}
