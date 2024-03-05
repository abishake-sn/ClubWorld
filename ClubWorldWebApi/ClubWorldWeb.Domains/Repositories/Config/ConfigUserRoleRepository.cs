using ClubWorldWeb.Domains.Models;
using ClubWorldWeb.Domains.Models.Config;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using ClubWorldWeb.Domains.Extensions;

namespace ClubWorldWeb.Domains.Repositories.Config
{
    public interface IConfigUserRoleRepository : IRepository<ConfigUserRoles>
    {
        Task<ConfigUserRoles> GetByUserId(int p_userId);
        Task<ConfigRole> GetRoleByUserId(int p_userId);
        Task MapMember(int p_userId);
        Task MapAdmin(int p_userId);

    }

    public class ConfigUserRoleRepository : Repository<ConfigUserRoles>, IConfigUserRoleRepository
    {
        public ConfigUserRoleRepository(IServiceProvider p_provider, ClubWorldWebDbContext p_dataContext) : base(p_provider, p_dataContext)
        {

        }

        public async Task<ConfigUserRoles> GetByUserId(int p_userId)
        {
            Expression<Func<ConfigUserRoles, bool>> filters =
                Extensions.ExpressionHelper.GetCriteriaWhere<ConfigUserRoles>(a => a.UserId, OperationExpression.Equals, p_userId);

            return await this.GetOneAsync(filters);
        }

        public async Task<ConfigRole> GetRoleByUserId(int p_userId)
        {
            ConfigUserRoles userRoles = await GetByUserId(p_userId);

            if (userRoles == null)
                return default;

            IConfigRoleRepository roleRepo = Provider.GetService<IConfigRoleRepository>();
            return await roleRepo.GetByIdAsync(userRoles.RoleId);
        }

        public async Task MapMember(int p_userId)
        {
            ConfigUserRoles userRoles = await GetByUserId(p_userId);
            if (userRoles == null)
            {
                IConfigRoleRepository roleRepo = Provider.GetService<IConfigRoleRepository>();
                ConfigRole role = await roleRepo.GetByParamsAsync("Member", "", true);

                if (role != null)
                {
                    userRoles = new ConfigUserRoles()
                    {
                        RoleId = role.id,
                        UserId = p_userId
                    };

                    await InsertOneAsync(userRoles);
                }
            }
        }

        public async Task MapAdmin(int p_userId)
        {
            ConfigUserRoles userRoles = await GetByUserId(p_userId);
            if (userRoles == null)
            {
                IConfigRoleRepository roleRepo = Provider.GetService<IConfigRoleRepository>();
                ConfigRole role = await roleRepo.GetByParamsAsync("Admin", "", true);

                if (role != null)
                {
                    userRoles = new ConfigUserRoles()
                    {
                        RoleId = role.id,
                        UserId = p_userId
                    };

                    await InsertOneAsync(userRoles);
                }
            }


        }
    }
}