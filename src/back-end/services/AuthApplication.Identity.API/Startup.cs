using AuthApplication.Identity.API.Configurations;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace AuthApplication.Identity.API
{
    public class Startup
    {

        private static IConfiguration Configuration = new ConfigurationBuilder()
              .SetBasePath(System.IO.Directory.GetCurrentDirectory())
              .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
              .AddJsonFile($"appsettings.{Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT")}.json", optional: true)
              .AddEnvironmentVariables()
              .Build();

        
        public void ConfigureServices(IServiceCollection services) 
            => services.UseAddApiService(Configuration);

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
            => app.UseApiConfiguration(env, Configuration);




    }
}
