using Materials.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System.Data;

namespace Materials.Data
{
    public static class SeedData
    {
        public static async Task Initialize(IServiceProvider serviceProvider)
        {
            var context = serviceProvider.GetRequiredService<ApplicationDbContext>();

            if (context.Roles.Count() == 0)
            {
                var role = new Role
                {
                    Name = "Admin"
                };

                await context.AddAsync(role);
                await context.SaveChangesAsync();
            }

            if (context.Users.Count() == 0)
            {
                var user = new User
                {
                    UserName = "admin",
                    Password = "admin",
                    Role = await context.Roles.FirstOrDefaultAsync()
                };

                await context.AddAsync(user);
                await context.SaveChangesAsync();
            }
        }
    }
}
