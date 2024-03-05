using ClubWorldWeb.Domains.Models;
using ClubWorldWeb.Domains.Models.Transaction;

using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq.Expressions;
using ClubWorldWeb.Domains.Extensions;
using ClubWorldWeb.Domains.Models.Master;
using Polly;
using System.Linq;

namespace ClubWorldWeb.Domains.Repositories.Transaction
{
    public interface ITrnMemberCheckInRepository : IRepository<TrnMemberCheckIn>
    {
        Task<TrnMemberCheckIn> GetCheckInbyExternalId(int ExternalId);
    }

    public class TrnMemberCheckInRepository : Repository<TrnMemberCheckIn>, ITrnMemberCheckInRepository
    {
        
        public TrnMemberCheckInRepository(IServiceProvider p_provider, ClubWorldWebDbContext p_dataContext) : base(p_provider, p_dataContext)
        {
           
        }
        public async Task<TrnMemberCheckIn> GetCheckInbyExternalId(int ExternalId)
        {
            List<Expression<Func<TrnMemberCheckIn, bool>>> filterConditions = new List<Expression<Func<TrnMemberCheckIn, bool>>>();
            Expression<Func<TrnMemberCheckIn, bool>> filters = null;
            filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<TrnMemberCheckIn>(a => a.ExternalId, OperationExpression.Equals, ExternalId));

            if (filterConditions.Count > 0)
            {
                foreach (Expression<Func<TrnMemberCheckIn, bool>> filterCondition in filterConditions)
                    filters = (filters == null ? filterCondition : filters.And(filterCondition));
            }
            if (filters == null)
                return default;

            return await this.GetOneAsync(filters);
        }


    }
}

