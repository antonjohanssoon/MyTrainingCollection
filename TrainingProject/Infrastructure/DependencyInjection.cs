using Application.Interfaces.RepositoryInterface;
using Application.Interfaces.RepositoryInterfaces;
using Infrastructure.Databases;
using Infrastructure.Repositories;
using Infrastructure.Repositories.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services, string connectionstring)
        {
            services.AddDbContext<Database>(options =>
            {
                options.UseSqlServer(connectionstring);
            });

            services.AddScoped(typeof(IRepository<>), typeof(GenericRepository<>));
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IWorkoutSessionRepository, WorkoutSessionRepository>();


            return services;
        }
    }
}
