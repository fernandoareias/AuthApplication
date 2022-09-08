using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace AuthApplication.Identity.API.Configurations
{
    public static class SwaggerConfig
    {
        public static IServiceCollection AddSwagger(this IServiceCollection service)
        {
            service.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new Microsoft.OpenApi.Models.OpenApiInfo()
                {
                    Title = "Auth Application API",
                    Description = "Esta API faz parte do projeto Auth Application",
                    Contact = new Microsoft.OpenApi.Models.OpenApiContact() { Name = "Fernando Areias", Email = "nando.calheirosx@gmail.com" },
                    License = new Microsoft.OpenApi.Models.OpenApiLicense() { Name = "MIT", Url = new Uri("https://opensource.org/license/MIT") }
                });
            });

            return service;
        }

        public static IApplicationBuilder UseSwaggerConfig(this IApplicationBuilder configuration)
        {
            configuration.UseSwagger();
            configuration.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "v1"));

            return configuration;
        }
    }
}
