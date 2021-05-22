using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.Repository;
using OnlineRetailer.Domain.Repository.Facade;
using OnlineRetailer.OrderApi.BackgroundServices;
using OnlineRetailer.OrderApi.Command;
using OnlineRetailer.OrderApi.Command.Facade;
using OnlineRetailer.OrderApi.Query;
using OnlineRetailer.OrderApi.Query.Facade;

namespace OnlineRetailer.OrderApi
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
            services.AddScoped<IOrderCommand, OrderCommand>();
            services.AddScoped<IOrderQuery, OrderQuery>();
            services.AddScoped<IEventRepository, EventStoreRepository>();

            // Hosted Services
            services.AddHostedService<OrderListener>();

            services.AddSingleton(_ => new EventClient(Configuration.GetConnectionString("EventStore")));

            services.AddControllers();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
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