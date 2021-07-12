using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using TaskTrackerApi.Contracts.V1.Responses;
using TaskTrackerApi.Data;
using TaskTrackerApi.Repositories;
using TaskTrackerApi.Services;

namespace TaskTrackerApi.Installers
{
    public class DbInstaller : IInstaller
    {
        public void InstallService(IServiceCollection services, IConfiguration configuration)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("DefaultConnection")));
            services.AddDefaultIdentity<IdentityUser>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();

            services.AddScoped<IBoardService, BoardService>();
            services.AddSingleton<IBoardRepository, BoardRepository>();
        }
    }
}
