using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Data;

[Table("Litters")]
public class Litter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Type { get; set; } = string.Empty;

    [Required]
    [Column(TypeName = "datetime")]
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    [Range(0, 1)]
    public double? Confidence { get; set; }

    [Required]
    [MaxLength(50)]
    [Column(TypeName = "varchar(50)")]
    public string Weather { get; set; } = string.Empty;

    [Required]
    [Range(-100, 100, ErrorMessage = "Temperature must be between -100 and 100.")]
    public int Temperature { get; set; }

    [Required]
    public bool IsHoliday { get; set; }

    // Navigation property for Camera
    [Required]
    public virtual Camera Camera { get; set; } = null!;

    // Explicit foreign key
    [ForeignKey(nameof(Camera))]
    public int CameraId { get; set; }

    // Not mapped enum helpers
    [NotMapped]
    public LitterCategory LitterCategory
    {
        get
        {
            if (Enum.TryParse<LitterCategory>(Type, true, out var category))
                return category;
            return LitterCategory.Unknown;
        }
        set => Type = value.ToString().ToLowerInvariant();
    }

    [NotMapped]
    public WeatherCategory WeatherCategory
    {
        get
        {
            if (Enum.TryParse<WeatherCategory>(Weather, true, out var category))
                return category;
            return WeatherCategory.Unknown;
        }
        set => Weather = value.ToString().ToLowerInvariant();
    }
}
