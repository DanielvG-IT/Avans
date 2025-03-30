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
    public Guid id { get; set; }

    /// <summary>
    /// Gets or sets the first name of the doctor.
    /// </summary>
    public string firstName { get; set; }

    /// <summary>
    /// Gets or sets the last name of the doctor.
    /// </summary>
    public string lastName { get; set; }

    /// <summary>
    /// Gets or sets the specialization of the doctor.
    /// </summary>
    public string specialization { get; set; }
}