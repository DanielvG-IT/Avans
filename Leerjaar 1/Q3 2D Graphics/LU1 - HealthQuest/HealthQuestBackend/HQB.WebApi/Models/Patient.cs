namespace HQB.WebApi.Models;
/// <summary>
/// Represents a patient in the HealthQuest system.
/// </summary>
public class Patient
{
    /// <summary>
    /// Gets or sets the unique identifier for the patient.
    /// </summary>
    public Guid? ID { get; set; }

    /// <summary>
    /// Gets or sets the first name of the patient.
    /// </summary>
    public required string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the patient.
    /// </summary>
    public required string LastName { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's guardian.
    /// </summary>
    public Guid? GuardianID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's treatment.
    /// </summary>
    public Guid? TreatmentID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's doctor.
    /// </summary>
    public Guid? DoctorID { get; set; }

    /// <summary>
    /// Gets or sets the URL or path to the patient's avatar image.
    /// </summary>
    public required string Avatar { get; set; }

    /// <summary>
    /// Gets or sets the permission if the doctor can access journal entries.
    /// </summary>
    public bool DoctorAccessJournal { get; set; }

    /// <summary>
    /// Gets or sets the permission if the guardian can access journal entries.
    /// </summary>
    public bool GuardianAccessJournal { get; set; }
}