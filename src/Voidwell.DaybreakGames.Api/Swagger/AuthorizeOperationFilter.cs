using Microsoft.AspNetCore.Authorization;
using Microsoft.OpenApi;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Voidwell.DaybreakGames.Api.Swagger;

/// <summary>
/// Marks only the operations that require authorization with the bearer security requirement,
/// leaving anonymous operations callable without a token.
/// </summary>
public class AuthorizeOperationFilter : IOperationFilter
{
    private readonly string _schemeName;

    public AuthorizeOperationFilter(string schemeName)
    {
        _schemeName = schemeName;
    }

    public void Apply(OpenApiOperation operation, OperationFilterContext context)
    {
        var metadata = context.ApiDescription.ActionDescriptor.EndpointMetadata;

        var requiresAuthorization = metadata.OfType<IAuthorizeData>().Any()
            && !metadata.OfType<IAllowAnonymous>().Any();

        if (!requiresAuthorization)
        {
            return;
        }

        operation.Responses ??= new OpenApiResponses();
        operation.Responses.TryAdd("401", new OpenApiResponse { Description = "Unauthorized" });
        operation.Responses.TryAdd("403", new OpenApiResponse { Description = "Forbidden" });

        operation.Security ??= new List<OpenApiSecurityRequirement>();
        operation.Security.Add(new OpenApiSecurityRequirement
        {
            [new OpenApiSecuritySchemeReference(_schemeName, context.Document)] = new List<string>()
        });
    }
}
