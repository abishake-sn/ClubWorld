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
using ClubWorldWeb.Domains.Common;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Linq;

namespace ClubWorldWeb.Domains.Repositories.Transaction
{
    public interface ITrnLedgerRepository : IRepository<TrnLedger>
    {
        Task<TrnLedger> Getledgerbyledger1(string ledger1);
        Task<TrnLedger> GetledgerbyExternalId(int ExternalId);
        Task<List<TrnLedgerRptInfo>> Rpt_TrnLedgerReport(int LedgerNo1, DateTime FromDate, DateTime ToDate);
        Task<List<RPt_MemberDashboard1>> GetmemberReportByIdAsync(int membId);

    }

    public class TrnLedgerRepository : Repository<TrnLedger>, ITrnLedgerRepository
    {
        ClubWorldWebDbContext dbContext;
        public TrnLedgerRepository(IServiceProvider p_provider, ClubWorldWebDbContext p_dataContext) : base(p_provider, p_dataContext)
        {
            dbContext=p_dataContext;
        }

        public async Task<TrnLedger> Getledgerbyledger1(string ledger1)
        {
            List<Expression<Func<TrnLedger, bool>>> filterConditions = new List<Expression<Func<TrnLedger, bool>>>();
            Expression<Func<TrnLedger, bool>> filters = null;
            Int16 isactive = 1;
            filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<TrnLedger>(a => a.Ledger1, OperationExpression.Equals, ledger1));
            filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<TrnLedger>(a => a.ActiveStatus, OperationExpression.Equals, "Active"));

            if (filterConditions.Count > 0)
            {
                foreach (Expression<Func<TrnLedger, bool>> filterCondition in filterConditions)
                    filters = (filters == null ? filterCondition : filters.And(filterCondition));
            }
            if (filters == null)
                return default;

            return await this.GetOneAsync(filters);

        }

        public async Task<TrnLedger> GetledgerbyExternalId(int ExternalId)
        {
            List<Expression<Func<TrnLedger, bool>>> filterConditions = new List<Expression<Func<TrnLedger, bool>>>();
            Expression<Func<TrnLedger, bool>> filters = null;
            filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<TrnLedger>(a => a.ExternalId, OperationExpression.Equals, ExternalId));
            if (filterConditions.Count > 0)
            {
                foreach (Expression<Func<TrnLedger, bool>> filterCondition in filterConditions)
                    filters = (filters == null ? filterCondition : filters.And(filterCondition));
            }
            if (filters == null)
                return default;

            return await this.GetOneAsync(filters);
        }
        public async Task<List<TrnLedgerRptInfo>> Rpt_TrnLedgerReport(int LedgerNo1, DateTime FromDate, DateTime ToDate)
        {

            var param = new SqlParameter[]
            {
                new SqlParameter(){ ParameterName = "@LedgerNo", SqlDbType=System.Data.SqlDbType.Int,Value = LedgerNo1},
                new SqlParameter(){ ParameterName = "@FromDate", SqlDbType=System.Data.SqlDbType.DateTime,Value = FromDate},
                new SqlParameter(){ ParameterName = "@ToDate", SqlDbType=System.Data.SqlDbType.DateTime,Value = ToDate},
            };
            return  dbContext.TrnLedgerRptInfo.FromSqlRaw("EXEC Rpt_ACC_Ledger @LedgerNo,@FromDate,@ToDate", param).ToList();
        }
        public async Task<List<RPt_MemberDashboard1>> GetmemberReportByIdAsync(int membId)
        {
            var param = new SqlParameter[]
            {
                new SqlParameter(){ ParameterName = "@Ledger", SqlDbType=System.Data.SqlDbType.Int,Value = membId}

            };
            return dbContext.RPt_MemberDashboard1.FromSqlRaw("EXEC Rpt_DBD_MemberReport1 @Ledger ", param).ToList();
        }
    }
}

