using BuildingBlocks.API.Extensions;
using BuildingBlocks.API.Middlewares;
using BuildingBlocks.Contracts.Options;
using Finance.API.Extensions;
using FluentValidation;
using FluentValidation.AspNetCore;
using Hangfire;

var builder = WebApplication.CreateBuilder(args);

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

builder.Services.AddFluentValidationAutoValidation();
builder.Services.AddValidatorsFromAssembly(typeof(Program).Assembly);
builder.Services.AddCustomFluentValidation();
builder.Services.AddHttpContextAccessor();

builder.Services.AddCustomHangfire(builder.Configuration.GetConnectionString("HangfireDbConnection")!);

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

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRateLimiter();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

using (var scope = app.Services.CreateScope())
{
    var recurringJobManager = scope.ServiceProvider.GetRequiredService<IRecurringJobManager>();
    recurringJobManager.AddOrUpdate<Finance.Infrastructure.Interfaces.IServices.IReleaseFundsJobService>(
        "ReleaseFundsJob",
        job => job.ProcessReleaseFundsAsync(CancellationToken.None),
        "0 0 * * *",
        new RecurringJobOptions { TimeZone = TimeZoneInfo.Utc });
}

app.Run();
