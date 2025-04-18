namespace HQB.WebApi.Models;
/// <summary>
/// Represents a doctor with an ID, first name, last name, and specialization.
/// </summary>
public class Doctor
{
    /// <summary>
    /// Gets or sets the unique identifier for the doctor.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the user ID associated with the guardian.
    /// This is a foreign key to the auth_AspNetUsers table.
    /// </summary>
    public required string UserID { get; set; }

    /// <summary>
    /// Gets or sets the first name of the doctor.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the doctor.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets the specialization of the doctor.
    /// </summary>
    public required string Specialization { get; set; }
}