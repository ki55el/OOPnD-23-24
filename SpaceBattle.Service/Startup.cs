using Swashbuckle.AspNetCore.Swagger;

namespace SpaceBattle.Service;

internal sealed class Startup
{
    public static void ConfigureServices(IServiceCollection services)
    {
        services.AddServiceModelWebServices(o =>
        {
            o.Title = "SpaceBattle API";
        });

        services.AddSingleton(new SwaggerOptions());
    }

    public static void Configure(IApplicationBuilder app)
    {
        app.UseMiddleware<SwaggerMiddleware>();
        app.UseSwaggerUI();

        app.UseServiceModel(builder =>
        {
            builder.AddService<Endpoint>();
            builder.AddServiceWebEndpoint<Endpoint, IEndpoint>(new WebHttpBinding
            {
                MaxReceivedMessageSize = 5242880,
                MaxBufferSize = 65536,
            }, "api", behavior =>
            {
                behavior.HelpEnabled = true;
                behavior.AutomaticFormatSelectionEnabled = true;
            });
        });
    }
}
