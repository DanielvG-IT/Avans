using Microsoft.AspNetCore.Identity;

namespace TralaAI.CoreApi.Interfaces
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IdentityUser user, DateTime expireDate);
    }
}