using Api.Application;
using Api.Application.ClientQueries;
using Api.Infrastructure;
using Api.Infrastructure.CrossCuttingConcerns;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Api
{
    public class Startup
    {
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers();

            services.AddHttpClient<IServicesClient, ServicesClient>();
            services.AddScoped<IServicesService, ServicesService>();

            services.AddMediatR(typeof(ServiceQuery)); 
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ClientsPipelineBehavior<,>));
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(AdditionalPipelineBehavior<,>));
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
