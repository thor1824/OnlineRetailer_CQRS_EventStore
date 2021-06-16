using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineRetailer.CustomerApi.BackgroundServices;
using OnlineRetailer.CustomerApi.Command;
using OnlineRetailer.CustomerApi.Command.Facade;
using OnlineRetailer.CustomerApi.Query;
using OnlineRetailer.CustomerApi.Query.Facade;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.Repository;
using OnlineRetailer.Domain.Repository.Facade;

namespace OnlineRetailer.CustomerApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        private IConfiguration Configuration { get; }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddScoped<ICustomerCommand, CustomerCommand>();
            services.AddScoped<ICustomerQuery, CustomerQuery>();
            services.AddScoped<IEventRepository, EventStoreRepository>();

            // Hosted Services
            services.AddHostedService<CustomerListener>();

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
                    c.SwaggerEndpoint("/swagger/v1/swagger.json", "CustomerApi V1");
                    c.RoutePrefix = string.Empty;
                });
            }

            app.UseHttpsRedirection();

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}