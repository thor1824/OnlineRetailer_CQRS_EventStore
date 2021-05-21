using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using OnlineRetailer.Domain.EventStore;
using OnlineRetailer.Domain.EventStore.Repository;
using OnlineRetailer.Domain.EventStore.Repository.Facade;
using OnlineRetailer.ProductsApi.Command;
using OnlineRetailer.ProductsApi.Command.Facades;
using OnlineRetailer.ProductsApi.Query;
using OnlineRetailer.ProductsApi.Query.Facades;

namespace OnlineRetailer.ProductsApi
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
            services.AddScoped<IProductCommand, ProductCommand>();
            services.AddScoped<IProductQuery, ProductQuery>();
            services.AddScoped<IEventRepository, EventStoreRepository>();

            services.AddSingleton(_ => new EventClient(Configuration.GetConnectionString("EventStore")));

            services.AddControllers();
            services.AddSwaggerGen();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

            app.UseSwagger();

            // Enable middleware to serve swagger-ui (HTML, JS, CSS, etc.),
            // specifying the Swagger JSON endpoint.
            app.UseSwaggerUI(c =>
            {
                c.SwaggerEndpoint("/swagger/v1/swagger.json", "ProductApi V1");
                c.RoutePrefix = string.Empty;
            });

            app.UseRouting();

            app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
        }
    }
}