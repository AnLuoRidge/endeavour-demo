using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EndeavourDemo.Models;
using EndeavourDemo.Services;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.OpenApi.Models;

namespace EndeavourDemo
{
    public class Startup
    {
        readonly string allowCorsOrigins = "_allowCorsOrigins";

        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            // DB
            var serverVersion = new MySqlServerVersion(new Version(5, 7, 12));

            services.AddDbContext<EndeavourContext>(
                dbContextOptions => dbContextOptions
                    .UseMySql(Configuration.GetConnectionString("Database"), serverVersion)
                    .EnableSensitiveDataLogging()
                    .EnableDetailedErrors()
            );

            // CORS
            services.AddCors(options =>
            {
                options.AddPolicy(name: allowCorsOrigins,
                                  builder =>
                                  {
                                      builder.WithOrigins("http://localhost:3000").AllowAnyMethod().AllowAnyHeader();
                                  });
            });

            services.AddControllers();
            services.AddScoped<TrolleyService>();

            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "EndeavourDemo", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "EndeavourDemo v1"));
            }

            app.UseRouting();

            app.UseCors(allowCorsOrigins);

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
