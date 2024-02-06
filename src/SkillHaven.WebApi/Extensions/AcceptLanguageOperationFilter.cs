using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace SkillHaven.WebApi.Extensions
{
    public class AcceptLanguageOperationFilter : IOperationFilter
    {
        public void Apply(OpenApiOperation operation, OperationFilterContext context)
        {
            if (operation.Parameters == null)
                operation.Parameters = new List<OpenApiParameter>();

            operation.Parameters.Add(new OpenApiParameter
            {
                Name = "Accept-Language",
                In = ParameterLocation.Header,
                Description = "Request language (default:en-EN)",
                Required = false,
                AllowEmptyValue = false,
                Schema = new OpenApiSchema { Type = "string" }
            });
        }
    }

}
