using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace FlightService.Api;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        const string validDescription = "Api to handle flights";
        var deprecatedDescription =
            $"{validDescription}{Environment.NewLine}This Api version has been deprecated";
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = "Flights",
                Version = description.ApiVersion.ToString(),
                Description = description.IsDeprecated ? deprecatedDescription : validDescription
            };

            options.SwaggerDoc(description.GroupName, info);
        }
    }

    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }
}