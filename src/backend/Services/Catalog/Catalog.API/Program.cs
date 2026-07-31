using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.Contracts.Options;
using BuildingBlocks.Logging;
using Catalog.API.Extensions;
using Catalog.API.Services;
using Catalog.Application.Common.Mappers;
using FluentValidation;
using FluentValidation.AspNetCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomLogging(builder.Configuration, "Catalog");

builder.AddCustomKestrel();

// Add services to the container.

builder.Services.AddCustomControllers();

builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddCustomDb(builder.Configuration);

builder.Services.AddMassTransitWithRabbitMq();

builder.Services.AddCustomRateLimit(
    builder.Configuration.GetSection("AppSettings:RateLimit").Get<RateLimitConfig>());

builder.Services.Register();

#region MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(CatalogProfile).Assembly));
#endregion

// Register JWT auth from Identity (uses ConfigurationManager internally)
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddCustomSwagger();
builder.Services.AddCustomApiVersioning();

builder.Services.AddCustomRedis(builder.Configuration);
builder.Services.AddCustomHealthChecks(builder.Configuration);
builder.Services.AddHostedService<EventClickFlushHostedService>();

#region AutoMapper
builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(CatalogProfile).Assembly));
#endregion

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddValidatorsFromAssembly(typeof(CatalogProfile).Assembly);
builder.Services.AddCustomFluentValidation();
builder.Services.AddHttpContextAccessor();
builder.Services.AddGrpc();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwaggerUI();
}

app.UseExceptionHandling();
app.UseRequestResponseLogging();
await app.UseDatabaseInitialization();

app.UseHttpsRedirection();
app.UseStaticFiles();

app.MapGrpcService<Catalog.API.Services.CatalogGrpcService>();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
