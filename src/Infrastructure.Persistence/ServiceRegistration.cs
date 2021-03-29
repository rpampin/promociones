using Application.Interfaces;
using Application.Interfaces.Repositories;
using Infrastructure.Persistence.Models;
using Infrastructure.Persistence.Repositories;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

namespace Infrastructure.Persistence
{
    public static class ServiceRegistration
    {
        public static void AddPersistenceInfrastructure(this IServiceCollection services, IConfiguration configuration)
        {
            //services.Configure<BasePromociones>(
            //    configuration.GetSection(nameof(BasePromociones)));
            //services.AddSingleton<IBasePromociones>(sp => sp.GetRequiredService<IOptions<BasePromociones>>().Value);

            services.AddSingleton<IBasePromociones>(sp => sp.GetRequiredService<IOptions<BasePromociones>>().Value);

            services.AddTransient(typeof(IGenericRepositoryAsync<>), typeof(GenericRepositoryAsync<>));
            services.AddTransient<IPromocionRepositoryAsync, PromocionRepositoryAsync>();
        }
    }
}
