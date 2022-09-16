using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using AuthApplication.Identity.API.Data;
using AuthApplication.Identity.API.Configurations.Serilog;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System;

namespace AuthApplication.Identity.API.Configurations
{
    public static class ApiConfiguration
    {
        public static IServiceCollection UseAddApiService(this IServiceCollection service, IConfiguration configuration)
        {
        
            service
                .AddDbContext<ApplicationDbContext>(
                    options =>
                        options.UseSqlite(configuration.GetConnectionString("DefaultConnection")
                    )
                );

            service.UseIdentityConfig(configuration);

            service.AddControllers().AddNewtonsoftJson(options =>
                options.SerializerSettings.ReferenceLoopHandling = Newtonsoft.Json.ReferenceLoopHandling.Ignore
            );

            service.Configure<AppSettings>(options => configuration.GetSection("AppSettings").Bind(options));
            service.AddSwagger();

            service.AddCors(options =>
            {

                options.AddDefaultPolicy(policy =>
                {
                    policy.WithOrigins(configuration.GetSection("AllowedOrigins").Get<string[]>()).AllowAnyMethod().AllowAnyHeader();

                });
            });

            service.AddControllers();
            


            return service;
        }

        public static IApplicationBuilder UseApiConfiguration(this IApplicationBuilder app, IWebHostEnvironment env, IConfiguration configuration)
        {
            app.UseSerilog(configuration);
            app.UseHttpsRedirection();

            app.UseCors(options => options.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());


            if (env.IsDevelopment())
                app.UseDeveloperExceptionPage();

            app.UseSwaggerConfig();

            app.UseRouting();
            app.UseEndpoints(endpoints => endpoints.MapControllers());
            

            return app;
        }
    }
}
