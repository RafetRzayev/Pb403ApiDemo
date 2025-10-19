using Microsoft.AspNetCore.Identity;

namespace Pb403ApiDemo.DataContext
{
    public class DataInitializer
    {
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly UserManager<IdentityUser> _userManager;

        public DataInitializer(RoleManager<IdentityRole> roleManager, UserManager<IdentityUser> userManager)
        {
            _roleManager = roleManager;
            _userManager = userManager;
        }

        public async Task SeedData()
        {
            var roles = new[] { "SuperAdmin", "Admin", "User" };

            foreach (var role in roles)
            {
                if (!await _roleManager.RoleExistsAsync(role))
                {
                    var identityRole = new IdentityRole { Name = role };
                    var result = await _roleManager.CreateAsync(identityRole);
                    if (!result.Succeeded)
                    {
                        throw new Exception($"Failed to create role: {role}");
                    }
                }
            }

            var superAdminUser = new IdentityUser
            {
                UserName = "superadmin",
                Email = ""
            };

            var userExists = await _userManager.FindByNameAsync(superAdminUser.UserName);

            if (userExists == null)
            {
                var createUserResult = await _userManager.CreateAsync(superAdminUser, "1234");

               if (createUserResult.Succeeded)
               {
                    var assignRoleResult = await _userManager.AddToRoleAsync(superAdminUser, "SuperAdmin");
                    if (!assignRoleResult.Succeeded)
                    {
                        throw new Exception("Failed to assign SuperAdmin role to the user.");
                    }
               }
               else
               {
                    throw new Exception("Failed to create SuperAdmin user.");
               }
            }
        } 
    }
}
