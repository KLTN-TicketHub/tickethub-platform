using AI.API.Extensions;
using AI.Infrastructure.Interfaces.IServices;
using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.Contracts.Options;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddCustomControllers();

builder.Services.AddCustomOptions(builder.Configuration);

builder.Services.AddCustomGrpc(builder.Configuration);

builder.Services.AddCustomDb(builder.Configuration);

builder.Services.AddCustomHangfire(builder.Configuration.GetConnectionString("HangfireDbConnection")!);

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

builder.Services.AddHttpContextAccessor();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseCustomSwaggerUI();
    app.UseHangfireDashboard("/hangfire");
}

app.UseExceptionHandling();
app.UseRequestResponseLogging();
await app.UseDatabaseInitialization();

RecurringJob.AddOrUpdate<IRecommendationService>(
    "recommendation-sync-and-train",
    service => service.SyncAndTrainAsync(default),
    Cron.Daily());

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
