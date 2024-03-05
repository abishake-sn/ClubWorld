using ClubWorldWeb.OAuth;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace ClubWorldWeb
{
    public static class Startup
    {
        public static IConfigurationRoot GetConfigurationRoot(IServiceCollection p_services)
        {
            IConfigurationBuilder builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json")
                .AddEnvironmentVariables();

            IConfigurationRoot result = builder.Build();
            p_services.AddOptions();
            p_services.Configure<AppSettings>(options => result.GetSection("AppSettings").Bind(options));
            return result;
        }
        public static void ConfigureDomainServices(IServiceCollection p_services)
        {
            p_services.AddScoped<ISecurityContext>(p_provider => new SecurityContext(p_provider));
        }
    }
}
