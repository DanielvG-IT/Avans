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
    [Column(TypeName = "varchar(50)")]
    public LitterCategory Type { get; set; }

    [Required]
    public DateTime TimeStamp { get; set; } = DateTime.UtcNow;

    [Range(0, 1)]
    public double Confidence { get; set; }

    [Required]
    [Column(TypeName = "varchar(50)")]
    public WeatherCategory Weather { get; set; }

    [Required]
    public int Temperature { get; set; }

    [Required]
    public bool IsHoliday { get; set; }

    [Required]
    public int CameraId { get; set; }

    [ForeignKey(nameof(CameraId))]
    public Camera Camera { get; set; } = null!;
}