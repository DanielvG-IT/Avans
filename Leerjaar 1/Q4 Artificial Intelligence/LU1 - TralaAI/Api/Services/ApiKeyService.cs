using Api.Data;

namespace Api.Services
{
    public class ApiKeyService(LitterDbContext context)
    {
        private readonly LitterDbContext _context = context;

        public bool IsValidApiKey(Guid apiKey)
        {
            var recievedApiKey = apiKey.ToString();
            if (string.IsNullOrEmpty(recievedApiKey))
                return false;

            var apiKeyEntity = _context.ApiKeys.FirstOrDefault(x => x.Key.ToString() == recievedApiKey);
            if (apiKeyEntity is null || !apiKeyEntity.IsActive)
                return false;

            return true;
        }
    }
}