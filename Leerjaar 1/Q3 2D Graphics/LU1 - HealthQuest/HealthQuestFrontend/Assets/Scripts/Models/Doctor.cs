using System;

/// <summary>
/// Represents a doctor with an ID, first name, last name, and specialization.
/// </summary>
[Serializable]
public class Doctor
{
    /// <summary>
    /// Gets or sets the unique identifier for the doctor.
    /// </summary>
    public string id;

    /// <summary>
    /// Gets or sets the first name of the doctor.
    /// </summary>
    public string firstName;

    /// <summary>
    /// Gets or sets the last name of the doctor.
    /// </summary>
    public string lastName;

    /// <summary>
    /// Gets or sets the specialization of the doctor.
    /// </summary>
    public string specialization;
}