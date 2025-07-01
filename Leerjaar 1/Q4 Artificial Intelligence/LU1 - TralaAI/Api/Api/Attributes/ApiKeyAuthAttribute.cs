using Microsoft.AspNetCore.Mvc;
using Api.Filters;

namespace Api.Attributes
{

    public class ApiKeyAuthAttribute : TypeFilterAttribute
    {
        public ApiKeyAuthAttribute() : base(typeof(ApiKeyAuthFilter))
        {
        }
    }
}