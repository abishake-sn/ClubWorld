using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Text;
using ClubWorldWeb.Domains.Models;
using ClubWorldWeb.Domains.Models.Master;
using ClubWorldWeb.Domains.Repositories.Config;
using ClubWorldWeb.Domains.Repositories.Master;
using ClubWorldWeb.Domains.Repositories.Transaction;
using ClubWorldWeb.Domains.Repositories.Transaction;

namespace ClubWorldWeb.Domains
{
    public static class Startup
    {
        public static void ConfigureRepositoryServices(IServiceCollection p_services)
        {
            ConfigureUserServices(p_services);
            ConfigureMasterServices(p_services);
            ConfigurTransactionServices(p_services);
        }
        public static void ConfigureUserServices(IServiceCollection p_services)
        {
            p_services.AddScoped<IConfigRoleRepository>(p_provider => new ConfigRoleRepository(p_provider,
                p_provider.GetService<ClubWorldWebDbContext>()));
            p_services.AddScoped<IConfigUserRoleRepository>(p_provider => new ConfigUserRoleRepository(p_provider,
                p_provider.GetService<ClubWorldWebDbContext>()));
        }
        public static void ConfigureMasterServices(IServiceCollection p_services)
        {
            p_services.AddScoped<IMasMemberRepository>(p_provider => new MasMemberRepository(p_provider,
              p_provider.GetService<ClubWorldWebDbContext>()));
        }
        public static void ConfigurTransactionServices(IServiceCollection p_services)
        {
            p_services.AddScoped<ITrnLedgerRepository>(p_provider => new TrnLedgerRepository(p_provider,
              p_provider.GetService<ClubWorldWebDbContext>()));

            p_services.AddScoped<ITrnOnlinePaymentRepository>(p_provider => new TrnOnlinePaymentRepository(p_provider,
           p_provider.GetService<ClubWorldWebDbContext>()));

            p_services.AddScoped<ITrnMemberCheckInRepository>(p_provider => new TrnMemberCheckInRepository(p_provider,
          p_provider.GetService<ClubWorldWebDbContext>()));

        }

    }
}
