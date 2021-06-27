using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using TaskTrackerApi.Data;

namespace TaskTrackerApi
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var host = CreateWebHostBuilder(args).Build();

            using (var serviceScope = host.Services.CreateScope())
            {
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DataContext>();

                await dbContext.Database.MigrateAsync();

                var roleManager = serviceScope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

                if (!await roleManager.RoleExistsAsync("Admin"))
                {
                    var adminRole = new IdentityRole("Admin");
                    await roleManager.CreateAsync(adminRole);

                    var admin = new IdentityUser() { UserName = "adam@tasker.com", Email = "adam@tasker.com" };
                    string userPDW = "Adam12345!";

                    var userManager = serviceScope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

                    var checkUser = await userManager.CreateAsync(admin, userPDW);

                    if (checkUser.Succeeded)
                    {
                        await userManager.AddToRoleAsync(admin, "Admin");
                    }
                }

                if (!await roleManager.RoleExistsAsync("Moderator"))
                {
                    var userRole = new IdentityRole("Moderator");
                    await roleManager.CreateAsync(userRole);
                }

                if (!await roleManager.RoleExistsAsync("User"))
                {
                    var userRole = new IdentityRole("User");
                    await roleManager.CreateAsync(userRole);
                }
            }

            await host.RunAsync();
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args) =>
            WebHost.CreateDefaultBuilder(args)
                .UseStartup<Startup>();
    }
}
