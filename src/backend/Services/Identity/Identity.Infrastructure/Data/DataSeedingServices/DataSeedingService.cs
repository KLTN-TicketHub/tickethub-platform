using Identity.Application.Common.Interfaces.IDataSeedingServices;
using Identity.Common.Options;
using Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;

namespace Identity.Infrastructure.Data.DataSeedingServices
{
    public class DataSeedingService : IDataSeedingService
    {
        private readonly Contexts.IdentityDbContext _dbContext;
        private readonly UserManager<User> _userManager;
        private readonly RoleManager<Role> _roleManager;
        private readonly AdminAccount _adminAccount;
        public DataSeedingService(
            UserManager<User> userManager,
            RoleManager<Role> roleManager,
            IOptions<AdminAccount> adminAccount,
            Contexts.IdentityDbContext applicationDbContext)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _adminAccount = adminAccount.Value;
            _dbContext = applicationDbContext;
        }
        public async Task SeedDataAsync(CancellationToken cancellationToken = default)
        {
            if (!await _dbContext.Roles.AnyAsync(cancellationToken))
            {
                List<Role> roles = new List<Role>
                {
                    new Role {Name = "Admin" },
                    new Role {Name = "Customer" },
                    new Role {Name = "Moderator"},
                    new Role {Name = "Organizer"},
                    new Role {Name = "Staff"}
                };

                foreach (var role in roles)
                    await _roleManager.CreateAsync(role);
            }

            if (!await _dbContext.Users.AnyAsync(cancellationToken))
            {
                Role? role = await _roleManager.FindByNameAsync("Admin");
                User user = new User
                {
                    FullName = _adminAccount.Account.FullName,
                    UserName = _adminAccount.Account.UserName,
                    Email = _adminAccount.Account.Email,
                    PhoneNumber = _adminAccount.Account.PhoneNumber,

                    LockoutEnabled = true
                };

                IdentityResult result = await _userManager.CreateAsync(user, _adminAccount.Account.Password);

                if (!result.Succeeded)
                    throw new Exception($"Failed to create admin user: {string.Join(", ", result.Errors.Select(e => e.Description))}");

                await _userManager.AddToRoleAsync(user, role.Name);

                string token = await _userManager.GenerateEmailConfirmationTokenAsync(user);

                IdentityResult confirmResult = await _userManager.ConfirmEmailAsync(user, token);

                if (!confirmResult.Succeeded)
                    throw new Exception($"Failed to confirm admin email:" +
                        $" {string.Join(", ", confirmResult.Errors.Select(e => e.Description))}");
            }
        }
    }
}
