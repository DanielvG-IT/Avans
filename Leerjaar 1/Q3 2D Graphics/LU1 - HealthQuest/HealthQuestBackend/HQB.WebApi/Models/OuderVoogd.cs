using Microsoft.AspNetCore.Identity;

namespace HQB.WebApi.Models;
public class OuderVoogd : IdentityUser
{
    public required string Voornaam { get; set; }
    public required string Achternaam { get; set; }
}