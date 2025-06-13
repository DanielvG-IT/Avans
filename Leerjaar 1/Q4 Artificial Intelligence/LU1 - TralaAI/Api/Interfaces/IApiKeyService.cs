namespace Api.Interfaces
{
    public interface IApiKeyService
    {
        bool IsValidApiKey(Guid apiKey);
    }
}