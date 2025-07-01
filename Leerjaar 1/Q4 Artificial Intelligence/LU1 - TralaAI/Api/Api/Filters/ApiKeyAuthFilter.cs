using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Interfaces;

namespace Api.Filters
{
    public class ApiKeyAuthFilter(IApiKeyService apiKeyService) : IActionFilter
    {
        private readonly IApiKeyService _apiKeyService = apiKeyService;

        public void OnActionExecuting(ActionExecutingContext context)
        {
            // Try to get the API key from the request header
            if (!context.HttpContext.Request.Headers.TryGetValue("X-API-KEY", out var apiKeyHeader))
            {
                context.Result = new UnauthorizedObjectResult("Missing API key.");
                return;
            }

            // Check if the API key is not empty or whitespace
            if (string.IsNullOrWhiteSpace(apiKeyHeader))
            {
                context.Result = new UnauthorizedObjectResult("API key cannot be empty.");
                return;
            }

            // Validate the API key format (must be a GUID)
            if (!Guid.TryParse(apiKeyHeader, out var apiKey))
            {
                context.Result = new UnauthorizedObjectResult("API key format is invalid.");
                return;
            }

            // Check if the API key is valid
            if (!_apiKeyService.IsValidApiKey(apiKey))
            {
                context.Result = new UnauthorizedObjectResult("API key is invalid.");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Deze kun je leeg laten, tenzij je iets wil doen ná de actie
        }
    }
}