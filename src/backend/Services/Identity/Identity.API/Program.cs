using BuildingBlocks.API.Extensions;
using Identity.API.Extensions;
using Identity.Common.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCustomControllers();

builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddCustomRateLimit(builder.Configuration
    .GetSection("RateLimitConfig")?
    .Get<AppSettings>()?
    .RateLimitConfig);

builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomSwagger();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
