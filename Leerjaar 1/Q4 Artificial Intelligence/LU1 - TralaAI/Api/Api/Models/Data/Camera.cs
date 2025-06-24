using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models.Data;

[Table("Cameras")]
public class Camera
{
  [Key]
  [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
  public int Id { get; set; }

  [Required]
  [Column(TypeName = "decimal(10,6)")]
  public decimal Latitude { get; set; }

  [Required]
  [Column(TypeName = "decimal(10,6)")]
  public decimal Longitude { get; set; }

  [Required]
  public required string Location { get; set; }

  // Navigation property for EF Core one-to-many relationship
  public virtual ICollection<Litter> Litters { get; set; } = [];
}