using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Api.Interfaces;

namespace Api.Filters
{
    public class ApiKeyAuthFilter : IActionFilter
    {
        private readonly IApiKeyService _apiKeyService;

        public ApiKeyAuthFilter(IApiKeyService apiKeyService)
        {
            _apiKeyService = apiKeyService;
        }

        public void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.HttpContext.Request.Headers.TryGetValue("ApiKey", out var apiKeyHeader) ||
                string.IsNullOrWhiteSpace(apiKeyHeader) ||
                !Guid.TryParse(apiKeyHeader, out var apiKey) ||
                !_apiKeyService.IsValidApiKey(apiKey))
            {
                context.Result = new UnauthorizedObjectResult("Invalid or missing API key.");
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            // Deze kun je leeg laten, tenzij je iets wil doen ná de actie
        }
    }
}