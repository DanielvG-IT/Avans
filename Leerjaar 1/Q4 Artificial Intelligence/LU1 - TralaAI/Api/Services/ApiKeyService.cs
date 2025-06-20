using Api.Data;
using Api.Interfaces;

namespace Api.Services
{
    public class ApiKeyService(LitterDbContext context) : IApiKeyService
    {
        private readonly LitterDbContext _context = context;

        public bool IsValidApiKey(Guid apiKey)
        {
            if (apiKey == Guid.Empty)
                return false;

            var apiKeyEntity = _context.ApiKeys.FirstOrDefault(x => x.Key == apiKey);
            if (apiKeyEntity is null || !apiKeyEntity.IsActive || apiKeyEntity.Type != "Backend")
                return false;

            if (apiKeyEntity.ExpiresAt.HasValue && apiKeyEntity.ExpiresAt <= DateTime.UtcNow)
                return false;

            return true;
        }
    }
}