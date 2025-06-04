using Microsoft.AspNetCore.Identity;

namespace Api.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IdentityUser user, DateTime expireDate);
    }
}