using Newtonsoft.Json;
using System.ComponentModel.DataAnnotations;

namespace Api.Models.Auth
{
  public class LoginRequest
  {
    [Required]
    [EmailAddress]
    [JsonProperty("email")]
    public required string Email { get; set; }

    [Required]
    [DataType(DataType.Password)]
    [JsonProperty("password")]
    public required string Password { get; set; }
  }
}