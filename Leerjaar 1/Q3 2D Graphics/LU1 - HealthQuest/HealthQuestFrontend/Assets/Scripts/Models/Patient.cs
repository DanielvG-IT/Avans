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
    public Guid id { get; set; }

    /// <summary>
    /// Gets or sets the first name of the patient.
    /// </summary>
    public string firstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the patient.
    /// </summary>
    public string lastName { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's guardian.
    /// </summary>
    public Guid guardianID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's treatment.
    /// </summary>
    public Guid treatmentID { get; set; }

    /// <summary>
    /// Gets or sets the unique identifier of the patient's doctor.
    /// </summary>
    public Guid doctorID { get; set; }

    /// <summary>
    /// Gets or sets the URL or path to the patient's avatar image.
    /// </summary>
    public string avatar { get; set; }
}