using EntryManager.Shared.Interop;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenApi();

var services = builder.Services;
var config = builder.Configuration;
var env = builder.Environment;

services.AddServices(config, env);

var app = builder.Build();

var appInfo = app.Services.GetService<AppInfo>();

if (!env.IsProduction())
{
    app.MapOpenApi();
    
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", $"{appInfo.ServiceName}-{appInfo.Version}");
        options.RoutePrefix = "swagger";
    });
}

app.MapControllers();
app.Run();