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
    public string id;

    /// <summary>
    /// Gets or sets the first name of the patient.
    /// </summary>
    public string firstName;

    /// <summary>
    /// Gets or sets the last name of the patient.
    /// </summary>
    public string lastName;

    /// <summary>
    /// Gets or sets the unique identifier of the patient's guardian.
    /// </summary>
    public string guardianID;

    /// <summary>
    /// Gets or sets the unique identifier of the patient's treatment.
    /// </summary>
    public string treatmentID;

    /// <summary>
    /// Gets or sets the unique identifier of the patient's doctor.
    /// </summary>
    public string doctorID;

    /// <summary>
    /// Gets or sets the URL or path to the patient's avatar image.
    /// </summary>
    public string avatar;
}