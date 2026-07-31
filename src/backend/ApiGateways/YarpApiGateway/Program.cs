using BuildingBlocks.API.Extensions;
using YarpApiGateway.Extensions;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomLogging(builder.Configuration, "Gateway");

// Add services to the container.

builder.Services.AddControllers();
builder.Services.AddOpenApi();
builder.Services.AddReverseProxy()
    .LoadFromConfig(builder.Configuration.GetSection("ReverseProxy"));

builder.Services.AddCustomCors(builder.Configuration);
builder.Services.AddCustomHealthChecks();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.UseCors(CorsExtension.GetPolicyName());

app.UseWebSockets();

app.MapReverseProxy();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
