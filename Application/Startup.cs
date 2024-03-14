using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using SimpleApi.Controllers;
using SimpleApi.Service;

namespace SimpleApi.Application
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
            // Add services to the container.

            // Use this line to add MVC services to the application
            // For API projects, you might only need controllers, so use AddControllers() instead
            services.AddControllers();
            services.AddSingleton<IAuthorizationHandler, SignCheckHandler>();
            services.AddSingleton<ILogger, Logger<NextCloudApiController>>();
            services.AddSingleton<OcsService>();
            services.AddDbContext<SimpleApi.Persistence.DbContext>(options => options.UseSqlite("Data Source=GroupFinder.db"));
            services.AddAuthorization(options =>
            {
                options.AddPolicy("SignCheckPolicy", policy =>
                    policy.Requirements.Add(new SignCheckRequirement()));
            });
            // Add Swagger/OpenAPI support
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Your API", Version = "v1" });
            });

            // Configure other services like DbContext, Identity, etc., here
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Your API v1"));
            }
            else
            {
                // Add middleware to handle errors in production
            }

          //  app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthorization() ;
            app.UseStaticFiles();
            app.UseEndpoints(endpoints =>
            {
                // Map controllers to the API endpoints
                endpoints.MapControllers();
            });
        }
    }
}
