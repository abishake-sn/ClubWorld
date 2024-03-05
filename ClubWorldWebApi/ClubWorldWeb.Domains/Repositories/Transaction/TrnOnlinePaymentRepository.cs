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
    public interface ITrnOnlinePaymentRepository : IRepository<TrnOnlinePayment>
    {
        int GetVoucherNo();
    }

    public class TrnOnlinePaymentRepository : Repository<TrnOnlinePayment>, ITrnOnlinePaymentRepository
    {
        protected ClubWorldWebDbContext Context;
        public TrnOnlinePaymentRepository(IServiceProvider p_provider, ClubWorldWebDbContext p_dataContext) : base(p_provider, p_dataContext)
        {
            Context = p_dataContext;
        }
        public int GetVoucherNo()
        {
            int VouNo = 1;
            var record = Context.TrnOnlinePayment.OrderByDescending(x => x.VouNo).FirstOrDefault();
            if (record != null) { VouNo = (int)record.VouNo + 1; }
            return VouNo;
        }
        //public async Task<TrnOnlinePayment> Getledgerbyledger1(int ledger1)
        //{
        //    List<Expression<Func<TrnOnlinePayment, bool>>> filterConditions = new List<Expression<Func<TrnOnlinePayment, bool>>>();
        //    Expression<Func<TrnOnlinePayment, bool>> filters = null;
        //    Int16 isactive = 1;
        //    filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<TrnOnlinePayment>(a => a.Ledger1, OperationExpression.Equals, ledger1));
        //    filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<TrnOnlinePayment>(a => a.ActiveStatus, OperationExpression.Equals, "Active"));

        //    if (filterConditions.Count > 0)
        //    {
        //        foreach (Expression<Func<TrnOnlinePayment, bool>> filterCondition in filterConditions)
        //            filters = (filters == null ? filterCondition : filters.And(filterCondition));
        //    }
        //    if (filters == null)
        //        return default;

        //    return await this.GetOneAsync(filters);


        //}
    }
}

