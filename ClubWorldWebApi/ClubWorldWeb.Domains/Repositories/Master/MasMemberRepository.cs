using ClubWorldWeb.Domains.Models;
using ClubWorldWeb.Domains.Models.Master;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using System.Linq;
using System.Linq.Expressions;
using ClubWorldWeb.Domains.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient;
using Polly;
using ClubWorldWeb.Domains.Common;
using ClubWorldWeb.Domains.Models.Transaction;


namespace ClubWorldWeb.Domains.Repositories.Master
{
    public interface IMasMemberRepository : IRepository<MasMember>
    {
        Task<MasMember> GetMemberByMemCode(string memcode);
        //Task<RPt_MemberDashboard1> GetmemberReportByIdAsync(int membId);
    }
    
    public class MasMemberRepository : Repository<MasMember>, IMasMemberRepository
    {
        DbContext dbContext;
        public MasMemberRepository(IServiceProvider p_provider, ClubWorldWebDbContext p_dataContext) : base(p_provider, p_dataContext)
        {
            dbContext = p_dataContext;
        }

        //public async Task<List<MasMember>> GetMemByMemType(string memtype)
        //{
        //    List<Expression<Func<MasMember, bool>>> filterConditions = new List<Expression<Func<MasMember, bool>>>();
        //    Expression<Func<MasMember, bool>> filters = null;
        //    Int16 isactive = 1;
        //    filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<MasMember>(a => a.MemberType, OperationExpression.Equals, memtype));
        //    filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<MasMember>(a => a.Isactive, OperationExpression.Equals, isactive));

        //    if (filterConditions.Count > 0)
        //    {
        //        foreach (Expression<Func<MasMember, bool>> filterCondition in filterConditions)
        //            filters = (filters == null ? filterCondition : filters.And(filterCondition));
        //    }
        //    if (filters == null)
        //        return default;

        //    return await this.GetManyAsync(filters);


        //}
        public async Task<MasMember> GetMemberByMemCode(string memcode)
        {
            List<Expression<Func<MasMember, bool>>> filterConditions = new List<Expression<Func<MasMember, bool>>>();
            Expression<Func<MasMember, bool>> filters = null;
            Int16 isactive = 1;
            filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<MasMember>(a => a.MemberCode, OperationExpression.Equals, memcode));
            filterConditions.Add(Extensions.ExpressionHelper.GetCriteriaWhere<MasMember>(a => a.ActiveStatus, OperationExpression.Equals,"Active" ));

            if (filterConditions.Count > 0)
            {
                foreach (Expression<Func<MasMember, bool>> filterCondition in filterConditions)
                    filters = (filters == null ? filterCondition : filters.And(filterCondition));
            }
            if (filters == null)
                return default;

            return await this.GetOneAsync(filters);
        }

        

        //public async Task<RPt_MemberDashboard1> GetmemberReportByIdAsync(int membId)
        //{
        //    var param = new SqlParameter[]
        //    {
        //        new SqlParameter(){ ParameterName = "@Ledger", SqlDbType=System.Data.SqlDbType.Int,Value = membId}
        //    };              

        //    return await .FromSqlRaw("EXEC Rpt_DBD_MemberReport1 @Ledger ", param).ToList();
        //}

        //public async Task<List<RPt_MemberDashboard1>> GetmemberReportByIdAsync(int membId)
        //{

        //    var param = new SqlParameter[]
        //    {
        //        new SqlParameter(){ ParameterName = "@Ledger", SqlDbType=System.Data.SqlDbType.Int,Value = membId}

        //    };
        //    return dbContext.RPt_MemberDashboard1.FromSqlRaw("EXEC Rpt_DBD_MemberReport1 @Ledger ", param).ToList();
        //}


    }


}
