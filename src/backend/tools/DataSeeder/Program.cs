using Catalog.Domain.Entities;
using Catalog.Domain.Enums;
using Catalog.Infrastructure.Data.Contexts;
using Identity.Domain.Entities;
using Identity.Infrastructure.Data.Contexts;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

IConfiguration configuration = new ConfigurationBuilder()
    .SetBasePath(AppContext.BaseDirectory)
    .AddJsonFile("appsettings.json", optional: false)
    .Build();

int userCount = configuration.GetValue<int>("SeedSettings:UserCount");
int minClicksPerUser = configuration.GetValue<int>("SeedSettings:MinClicksPerUser");
int maxClicksPerUser = configuration.GetValue<int>("SeedSettings:MaxClicksPerUser");
int minRandomClicksPerUser = configuration.GetValue<int>("SeedSettings:MinRandomClicksPerUser");
int maxRandomClicksPerUser = configuration.GetValue<int>("SeedSettings:MaxRandomClicksPerUser");

ServiceCollection services = new ServiceCollection();

services.AddLogging(builder => builder.AddConsole());

services.AddDbContext<IdentityDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("IdentityDbConnection")));

services.AddDbContext<CatalogDbContext>(options =>
    options.UseSqlServer(configuration.GetConnectionString("CatalogDbConnection")));

services.AddIdentityCore<User>(options =>
{
    options.Password.RequireDigit = true;
    options.Password.RequireLowercase = true;
    options.Password.RequireUppercase = true;
    options.Password.RequiredLength = 8;
    options.Password.RequireNonAlphanumeric = true;
    options.User.RequireUniqueEmail = true;
})
    .AddRoles<Role>()
    .AddEntityFrameworkStores<IdentityDbContext>();

await using ServiceProvider provider = services.BuildServiceProvider();
await using AsyncServiceScope scope = provider.CreateAsyncScope();

ILogger logger = scope.ServiceProvider.GetRequiredService<ILoggerFactory>().CreateLogger("DataSeeder");

UserManager<User> userManager = scope.ServiceProvider.GetRequiredService<UserManager<User>>();
RoleManager<Role> roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<Role>>();
CatalogDbContext catalogDb = scope.ServiceProvider.GetRequiredService<CatalogDbContext>();

const string SeedEmailPrefix = "seeduser";
const string SeedPassword = "SeedUser@2026";
const string CustomerRole = "Customer";

bool fakeUsersAlreadySeeded = await userManager.Users.CountAsync(u => u.Email!.StartsWith(SeedEmailPrefix)) >= userCount;

if (fakeUsersAlreadySeeded)
    logger.LogInformation("Already seeded {Count} fake users previously, skipping fake user creation. To reseed, delete the seeduser*@tickethub.local users first.", userCount);

if (await roleManager.FindByNameAsync(CustomerRole) == null)
    await roleManager.CreateAsync(new Role { Name = CustomerRole });

List<Guid> categoriesWithEvents = await catalogDb.Events
    .Where(e => e.Status == EventStatus.Published)
    .Select(e => e.CategoryId)
    .Distinct()
    .ToListAsync();

if (categoriesWithEvents.Count == 0)
{
    logger.LogWarning("No Published events found in the Catalog DB. Create/approve events before running DataSeeder.");
    return;
}

Dictionary<Guid, List<Guid>> eventsByCategory = await catalogDb.Events
    .Where(e => e.Status == EventStatus.Published)
    .GroupBy(e => e.CategoryId)
    .ToDictionaryAsync(g => g.Key, g => g.Select(e => e.Id).ToList());

Random random = new Random();
List<Guid> newUserIds = new List<Guid>();

if (!fakeUsersAlreadySeeded)
{
    for (int i = 0; i < userCount; i++)
    {
        string email = $"{SeedEmailPrefix}{i}@tickethub.local";

        User user = new User
        {
            FullName = $"Seed User {i}",
            UserName = email,
            Email = email,
            EmailConfirmed = true
        };

        IdentityResult result = await userManager.CreateAsync(user, SeedPassword);
        if (!result.Succeeded)
        {
            logger.LogWarning("Failed to create user {Email}: {Errors}", email, string.Join(", ", result.Errors.Select(e => e.Description)));
            continue;
        }

        await userManager.AddToRoleAsync(user, CustomerRole);
        newUserIds.Add(user.Id);
    }

    logger.LogInformation("Created {Count} fake users.", newUserIds.Count);

    List<UserEventClick> tasteVectorClicks = new List<UserEventClick>();

    foreach (Guid userId in newUserIds)
    {
        List<Guid> preferredCategories = categoriesWithEvents
            .OrderBy(_ => random.Next())
            .Take(Math.Min(2, categoriesWithEvents.Count))
            .ToList();

        List<Guid> preferredEvents = preferredCategories
            .SelectMany(categoryId => eventsByCategory.GetValueOrDefault(categoryId, new List<Guid>()))
            .Distinct()
            .ToList();

        List<Guid> otherEvents = eventsByCategory
            .Where(kv => !preferredCategories.Contains(kv.Key))
            .SelectMany(kv => kv.Value)
            .Distinct()
            .ToList();

        if (preferredEvents.Count == 0)
            continue;

        int totalClicks = random.Next(minClicksPerUser, maxClicksPerUser + 1);
        int preferredClickCount = (int)(totalClicks * 0.8);
        int otherClickCount = totalClicks - preferredClickCount;

        tasteVectorClicks.AddRange(GenerateClicksForUser(userId, preferredEvents, preferredClickCount, random));

        if (otherEvents.Count > 0)
            tasteVectorClicks.AddRange(GenerateClicksForUser(userId, otherEvents, otherClickCount, random));
    }

    catalogDb.UserEventClicks.AddRange(tasteVectorClicks);
    await catalogDb.SaveChangesAsync();

    logger.LogInformation("Seeded {Count} taste-vector clicks for {UserCount} fake users.", tasteVectorClicks.Count, newUserIds.Count);
}

// Diversity pass: every Customer-role user (real accounts + the fake ones just created)
// gets a handful of purely random clicks across the whole Published event pool, on top of
// any taste-vector clicks above. Useful when the real event catalog is small, since it
// spreads interactions wider instead of relying only on the biased taste-vector pattern.
List<Guid> allEventIds = eventsByCategory.Values.SelectMany(e => e).Distinct().ToList();
IList<User> allCustomerUsers = await userManager.GetUsersInRoleAsync(CustomerRole);

List<UserEventClick> randomClicks = new List<UserEventClick>();

foreach (User user in allCustomerUsers)
{
    int randomClickCount = random.Next(minRandomClicksPerUser, maxRandomClicksPerUser + 1);
    randomClicks.AddRange(GenerateClicksForUser(user.Id, allEventIds, randomClickCount, random));
}

catalogDb.UserEventClicks.AddRange(randomClicks);
await catalogDb.SaveChangesAsync();

logger.LogInformation("Seeded {Count} random diversity clicks across {UserCount} Customer users (real + fake).", randomClicks.Count, allCustomerUsers.Count);

static List<UserEventClick> GenerateClicksForUser(Guid userId, List<Guid> candidateEvents, int clickCount, Random random)
{
    List<UserEventClick> result = new List<UserEventClick>();

    for (int i = 0; i < clickCount; i++)
    {
        Guid eventId = candidateEvents[random.Next(candidateEvents.Count)];
        EventClickType clickType = random.NextDouble() < 0.25 ? EventClickType.PurchaseIntent : EventClickType.ViewDetail;
        DateTime clickedAt = DateTime.UtcNow.AddDays(-random.Next(0, 60)).AddHours(-random.Next(0, 24));

        result.Add(new UserEventClick(eventId, userId, clickType, clickedAt));
    }

    return result;
}
