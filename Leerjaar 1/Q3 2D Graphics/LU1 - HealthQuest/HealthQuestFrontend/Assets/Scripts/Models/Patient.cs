using System;

/// <summary>
/// Represents a patient in the HealthQuest system.
/// </summary>
[Serializable]
public class Patient
{
    /// <summary>
    /// Gets or sets the unique identifier for the patient.
    /// </summary>
    public Guid ID { get; set; }

    /// <summary>
    /// Gets or sets the first name of the patient.
    /// </summary>
    public string FirstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the patient.
    /// </summary>
    public string LastName { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's guardian.
    /// </summary>
    public Guid GuardianID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's treatment.
    /// </summary>
    public Guid TreatmentID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's doctor.
    /// </summary>
    public Guid DoctorID { get; set; }

    /// <summary>
    /// Gets or sets the URL or path to the patient's avatar image.
    /// </summary>
    public string Avatar { get; set; }
}