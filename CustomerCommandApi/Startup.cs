using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using OnlineRetailer.CustomerCommandApi.Command;
using OnlineRetailer.CustomerCommandApi.Command.Facade;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.Repository;
using OnlineRetailer.Domain.Repository.Facade;

namespace OnlineRetailer.CustomerCommandApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerCommand, CustomerCommand>();
            services.AddScoped<IEventRepository, EventStoreRepository>();


            services.AddSingleton(_ => new EventClient(Configuration.GetConnectionString("EventStore")));

            services.AddControllers();
            services.AddSwaggerGen();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c =>
                {
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerCommandApi V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}