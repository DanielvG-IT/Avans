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
  public decimal Latitude { get; set; }

  [Required]
  public decimal Longitude { get; set; }

  [Required]
  public required string Location { get; set; }
}