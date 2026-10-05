using Microsoft.OpenApi;

namespace Voidwell.DaybreakGames.Api.Swagger;

public static class SwaggerExtensions
{
    private const string _bearerScheme = "Bearer";

    public static IServiceCollection AddApiSwagger(this IServiceCollection services)
    {
        services.AddSwaggerGen(options =>
        {
            options.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = "Voidwell.DaybreakGames",
                Version = "v1"
            });

            // Authentication is optional: the scheme is only defined here and is applied per operation
            // by AuthorizeOperationFilter, so anonymous endpoints can be called without a token.
            options.AddSecurityDefinition(_bearerScheme, new OpenApiSecurityScheme
            {
                Type = SecuritySchemeType.Http,
                Scheme = "bearer",
                BearerFormat = "JWT",
                Description = "Access token for endpoints that require authorization. Not needed for public endpoints."
            });

            options.OperationFilter<AuthorizeOperationFilter>(_bearerScheme);
        });

        return services;
    }

    public static WebApplication UseApiSwagger(this WebApplication app)
    {
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "Voidwell.DaybreakGames v1");
        });

        return app;
    }
}
