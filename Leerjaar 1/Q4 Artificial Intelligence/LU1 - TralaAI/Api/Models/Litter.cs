using Api.Models.Enums;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Api.Models;

public class Litter
{
    [Key]
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int Id { get; set; }

    [Column(TypeName = "varchar(50)")]
    public LitterCategory? Type { get; set; }

    public DateTime TimeStamp { get; set; }
    public double Confidence { get; set; }

    [Column(TypeName = "varchar(50)")]
    public WeatherCategory? Weather { get; set; } // TODO: Possibly map to WeatherCondition enum

    public int Temperature { get; set; }
    public string? Location { get; set; } // TODO: Possibly map to Location enum
    public bool IsHoliday { get; set; } //! Komt van externe API
}