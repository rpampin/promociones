using FluentValidation;
using FluentValidation.AspNetCore;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Promociones.Model;
using Promociones.Services.Promociones;
using Promociones.Services.Promociones.Net;
using Promociones.Services.Repository;
using Promociones.Services.Repository.Net;
using Promociones.Validators;

namespace Promociones
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));

            services.Configure<BasePromociones>(
                Configuration.GetSection(nameof(BasePromociones)));

            services.AddSingleton<IBasePromociones>(sp =>
                sp.GetRequiredService<IOptions<BasePromociones>>().Value);

            services.AddSingleton<IPromocionesService, PromocionesService>();
            services.AddSingleton<IPromocionesRepository, PromocionesRepository>();
            services.AddTransient<IValidator<Promocion>, PromocionValidator>();

            services.AddControllers()
                .AddFluentValidation()
                .AddNewtonsoftJson(options => options.UseMemberCasing());
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Promociones", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Promociones v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
