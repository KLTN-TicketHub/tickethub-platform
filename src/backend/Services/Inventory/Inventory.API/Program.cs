using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.Contracts.Options;
using BuildingBlocks.Logging;
using FluentValidation;
using FluentValidation.AspNetCore;
using Inventory.API.Extensions;
using Inventory.API.Hubs;
using Inventory.API.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCustomLogging(builder.Configuration, "Inventory");

builder.AddCustomKestrel();

// Add services to the container.

builder.Services.AddCustomControllers();

builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddCustomDb(builder.Configuration);

builder.Services.AddMassTransitWithRabbitMq();

builder.Services.AddCustomRateLimit(
    builder.Configuration.GetSection("AppSettings:RateLimit").Get<RateLimitConfig>());

builder.Services.Register();

// Register JWT auth from Identity
builder.Services.AddJwtAuthentication(builder.Configuration);
builder.Services.AddAuthorization();

builder.Services.AddCustomSwagger();
builder.Services.AddCustomApiVersioning();

builder.Services.AddCustomRedis(builder.Configuration);
builder.Services.AddCustomHealthChecks(builder.Configuration);
builder.Services.AddHostedService<RedisKeyspaceNotificationHostedService>();

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddCustomFluentValidation();
builder.Services.AddHttpContextAccessor();
builder.Services.AddSignalR();
builder.Services.AddGrpc();
builder.Services.AddCustomGrpcClients(builder.Configuration);

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
app.MapGrpcService<InventoryGrpcService>();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapHub<SeatMapHub>("/hubs/seatmap");
app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
