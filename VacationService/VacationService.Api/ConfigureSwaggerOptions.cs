using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VacationService.Api;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        const string validDescription = "Api book a vacation, with flight, hotel and rental car";
        var deprecatedDescription =
            $"{validDescription}{Environment.NewLine}This Api version has been deprecated";
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = "Vacations",
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