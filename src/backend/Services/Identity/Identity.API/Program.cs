using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Middlewares;
using FluentValidation;
using FluentValidation.AspNetCore;
using Identity.API.Endpoints;
using Identity.API.Extensions;
using Identity.Application.Common.Mappers;
using Identity.Application.Features.Auth.Commands.LoginAdmin.Initiate;
using Identity.Application.Features.Auth.Validators;
using Identity.Common.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCustomDb(builder.Configuration);
builder.Services.AddCustomControllers();

builder.Services.AddCustomOptions(builder.Configuration);
builder.Services.AddMassTransitWithRabbitMq();
builder.Services.AddMemoryCache();

#region MediatR
builder.Services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(InitiateAdminLoginCommand).Assembly));
#endregion

builder.Services.AddCustomRateLimit(
    builder.Configuration
        .GetSection("AppSettings")
        .Get<AppSettings>()!
        .RateLimit);

builder.Services.RegisterSecurityService(builder.Configuration);
builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomSwagger();

builder.Services.Register();
builder.Services.AddHttpClient();
builder.Services.AddCustomRedis(builder.Configuration);
builder.Services.AddAuthorization();
builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(LoginRequestValidator).Assembly);
builder.Services.AddCustomFluentValidation();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile).Assembly));

var app = builder.Build();

#region Database Initialization
await app.UseDatabaseInitialization();
#endregion

app.MapWellKnownEndpoints();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwaggerUI();
}

app.UseHttpsRedirection();

app.UseRateLimiter();

app.UseAuthorization();

#region Custom Middlewares
app.UseExceptionHandling();
app.UseRequestResponseLogging();
#endregion

app.MapControllers();

app.Run();
