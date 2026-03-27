using BuildingBlocks.API.Extensions;
using Identity.API.Extensions;
using Identity.Application.Common.Mappers;
using Identity.Common.Options;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCustomDb(builder.Configuration);
builder.Services.AddCustomControllers();

builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddCustomRateLimit(builder.Configuration
    .GetSection("RateLimitConfig")?
    .Get<AppSettings>()?
    .RateLimitConfig);

builder.Services.RegisterSecurityService(builder.Configuration);
builder.Services.AddCustomApiVersioning();
builder.Services.AddCustomSwagger();

builder.Services.Register();

builder.Services.AddAuthorization();
// Learn more about configuring OpenAPI at https://aka.ms/aspnet/openapi

builder.Services.AddAutoMapper(cfg => cfg.AddMaps(typeof(UserProfile).Assembly));

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
