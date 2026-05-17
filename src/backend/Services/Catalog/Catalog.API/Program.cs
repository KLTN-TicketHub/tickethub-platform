using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.Contracts.Options;
using Catalog.API.Extensions;
using Catalog.Application.Common.Mappers;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCustomControllers();

builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddCustomRateLimit(
    builder.Configuration.GetSection("AppSettings:RateLimit").Get<RateLimitConfig>());

builder.Services.Register();

#region MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CatalogProfile).Assembly));
#endregion

// Register JWT auth from Identity (uses ConfigurationManager internally)
builder.Services.AddJwtAuthentication(builder.Configuration);

builder.Services.AddCustomSwagger();
builder.Services.AddCustomApiVersioning();

builder.Services.AddCustomRedis(builder.Configuration);
builder.Services.AddAuthorization();

#region AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CatalogProfile).Assembly));
#endregion

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwaggerUI();
}

app.UseExceptionHandling();

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.UseRequestResponseLogging();

app.MapControllers();

app.Run();
