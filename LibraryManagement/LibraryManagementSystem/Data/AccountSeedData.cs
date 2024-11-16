using LibraryManagementSystem.Models;
using Microsoft.AspNetCore.Identity;

namespace LibraryManagementSystem.Data
{
    public class AccountSeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider, UserManager<User> userManager, RoleManager<Role> roleManager)
        {
            var roleNames = new[] { "ADMINISTRATOR", "LIBRARIAN", "CUSTOMER" };

            // Tạo các role nếu chưa có
            foreach (var roleName in roleNames)
            {
                var roleExist = await roleManager.RoleExistsAsync(roleName);
                if (!roleExist)
                {
                    var role = new Role
                    {
                        Id = Guid.NewGuid().ToString(),
                        Name = roleName
                    };
                    await roleManager.CreateAsync(role);
                }
            }

            // Kiểm tra nếu người dùng admin chưa có
            var user = await userManager.FindByEmailAsync("admin@example.com");
            if (user == null)
            {
                user = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "admin@gmail.com",
                    Email = "admin@gmail.com",
                    Fullname = "Admin User"
                };

                await userManager.CreateAsync(user, "123");

                // Gán user vào role Administrator
                await userManager.AddToRoleAsync(user, "ADMINISTRATOR");
            }

            // Kiểm tra nếu người dùng librarian chưa có
            var librarian = await userManager.FindByEmailAsync("librarian@gmail.com");
            if (librarian == null)
            {
                librarian = new User
                {
                    Id = Guid.NewGuid().ToString(),
                    UserName = "librarian@gmail.com",
                    Email = "librarian@gmail.com",
                    Fullname = "Librarian User"
                };

                await userManager.CreateAsync(librarian, "123");

                // Gán user vào role Librarian
                await userManager.AddToRoleAsync(librarian, "LIBRARIAN");
            }
        }
    }
}
