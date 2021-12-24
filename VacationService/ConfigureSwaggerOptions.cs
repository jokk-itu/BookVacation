using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace VacationService;

public class ConfigureSwaggerOptions : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    public void Configure(SwaggerGenOptions options)
    {
        const string validDescription = "API book a vacation, with flight, hotel and rental car";
        var deprecatedDescription =
            $"API book a vacation, with flight, hotel and rental car.{Environment.NewLine}This API version has been deprecated";
        foreach (var description in _provider.ApiVersionDescriptions)
        {
            var info = new OpenApiInfo
            {
                Title = "Book a Vacation",
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