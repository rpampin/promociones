using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence.Models;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<BasePromociones>(options => configuration.GetSection(nameof(BasePromociones)).Bind(options));
            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IPromocionRepositoryAsync, PromocionRepositoryAsync>();
        }
    }
}
