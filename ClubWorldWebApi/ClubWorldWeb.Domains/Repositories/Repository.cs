using ClubWorldWeb.Domains.Extensions;
using ClubWorldWeb.Domains.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ClubWorldWeb.Domains.Repositories
{
    public interface IRepository : IDisposable
    {

    }

    public class Repository : IRepository
    {
        protected IServiceProvider Provider { get; private set; }
        protected ClubWorldWebDbContext DataContext { get; private set; }

        public Repository(IServiceProvider p_provider, ClubWorldWebDbContext p_dataContext)
        {
            this.Provider = p_provider;
            this.DataContext = p_dataContext;
        }

        public void Dispose()
        {
            if (this.DataContext != null)
                this.DataContext.Dispose();
        }
    }

    public interface IRepository<T> : IRepository
        where T : Model
    {
        Task<List<T>> GetAllAsync(FilterOptions p_filterOptions = null, bool p_init = true);
        Task<T> GetByIdAsync<TKey>(TKey p_id, bool p_init = true);
        Task<List<T>> GetByIdsAsync<TKey>(List<TKey> p_ids, bool p_init = true);

        Task<T> GetOneAsync(Expression<Func<T, bool>> p_filters = null, FilterOptions p_filterOptions = null, bool p_init = true);
        Task<List<T>> GetManyAsync(Expression<Func<T, bool>> p_filters = null, FilterOptions p_filterOptions = null, bool p_init = true);

        // Have to add JoinAsync -> Which will return IQueryable or make it a lookup

        Task<PaginationResults<T>> QueryAsync(PaginationQuery p_query, Expression<Func<T, bool>> p_filters = null, FilterOptions p_filterOptions = null, bool p_reversePages = false,
            int p_recordLimit = 10000);
        Task<int> CountAllAsync();
        Task<int> CountManyAsync(Expression<Func<T, bool>> p_filters = null);
        Task InsertOneAsync(T p_entity);
        Task UpdateOneAsync(T p_entity);
        Task RemoveOneAsync(T p_entity);
    }

    public abstract class Repository<T> : Repository, IRepository<T>
        where T : Model
    {
        private DbSet<T> entities;
        public Repository(IServiceProvider p_provider, ClubWorldWebDbContext p_dataContext) : base(p_provider, p_dataContext)
        {
            entities = this.DataContext.Set<T>();
        }

        protected virtual IQueryable<T> GetQueryable(Expression<Func<T, bool>> p_filters = null, FilterOptions p_filterOptions = null)
        {
            IQueryable<T> query = entities.AsQueryable();

            if (p_filters != null)
                query = query.Where(p_filters);

            if (p_filterOptions != null)
            {
                if (p_filterOptions.Skip.HasValue && p_filterOptions.Skip.Value >= 0)
                    query = query.Skip(p_filterOptions.Skip.Value);

                if (p_filterOptions.Limit.HasValue && p_filterOptions.Limit.Value > 0)
                {
                    query = query.Take(p_filterOptions.Limit.Value);
                }
            }

            return query;
        }

        public virtual Task<List<T>> GetAllAsync(FilterOptions p_filterOptions = null, bool p_init = true)
        {
            return GetManyAsync(p_filterOptions: p_filterOptions, p_init: p_init);
        }

        public virtual async Task<T> GetByIdAsync<TKey>(TKey p_id, bool p_init = true)
        {
            try
            {
                T result = await entities.FindAsync(p_id);
                if (result != null && result is Model && p_init)
                {
                    Model model = result as Model;
                    await model.OnInitAsync(this.Provider);
                }
                return result;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        public virtual async Task<List<T>> GetByIdsAsync<TKey>(List<TKey> p_ids, bool p_init = true)
        {
            try
            {
                T[] results = await DataContext.FindAllAsync<T>(p_ids);

                foreach (T result in results)
                {
                    if (result == null)
                        continue;

                    if (p_init && result is Model)
                    {
                        Model model = result as Model;
                        await model.OnInitAsync(this.Provider);
                    }
                }

                return results.ToList();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        public async Task<List<T>> GetManyAsync(Expression<Func<T, bool>> p_filters = null, FilterOptions p_filterOptions = null, bool p_init = true)
        {
            try
            {
                IQueryable<T> query = GetQueryable(p_filters, p_filterOptions);
                List<T> results = await query.ToListAsync();

                foreach (T result in results)
                {
                    if (result == null)
                        continue;

                    if (p_init && result is Model)
                    {
                        Model model = result as Model;
                        await model.OnInitAsync(this.Provider);
                    }
                }

                return results;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        public async Task<T> GetOneAsync(Expression<Func<T, bool>> p_filters = null, FilterOptions p_filterOptions = null, bool p_init = true)
        {
            try
            {
                IQueryable<T> query = GetQueryable(p_filters, p_filterOptions);
                T result = await query.FirstOrDefaultAsync();
                if (result != null && result is Model && p_init)
                {
                    Model model = result as Model;
                    await model.OnInitAsync(this.Provider);
                }
                return result;
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        public virtual async Task<PaginationResults<T>> QueryAsync(PaginationQuery p_query, Expression<Func<T, bool>> p_filters = null, FilterOptions p_filterOptions = null,
            bool p_reversePages = false, int p_recordLimit = 10000)
        {
            PaginationResults<T> results = new PaginationResults<T>()
            {
                RecordsPerPage = p_query.RecordsPerPage
            };

            FilterOptions filterOptions = (p_filterOptions ?? new FilterOptions());

            results.RecordsTotal = await CountManyAsync(p_filters);
            results.PagesTotal = results.RecordsPerPage <= 0 ? 1 : (int)Math.Ceiling((double)results.RecordsTotal / (double)results.RecordsPerPage);
            results.PageIndex = Math.Max(0, Math.Min(p_query.PageIndex, results.PagesTotal - 1));

            filterOptions.Limit = results.RecordsPerPage <= 0 ? 10 : results.RecordsPerPage;
            if (!p_reversePages)
            {
                filterOptions.Skip = results.PageIndex * filterOptions.Limit;
            }
            else
            {
                filterOptions.Skip = results.RecordsTotal - (results.PageIndex + 1) * results.RecordsPerPage;
                if (filterOptions.Skip.Value < 0)
                {
                    filterOptions.Limit = results.RecordsPerPage + filterOptions.Skip.Value;
                    filterOptions.Skip = 0;
                }
            }

            results.PageRecords = await GetManyAsync(p_filters, filterOptions);

            if (p_reversePages)
                results.PageRecords.Reverse();

            return results;
        }

        public async Task<int> CountAllAsync()
        {
            return await CountManyAsync();
        }

        public async Task<int> CountManyAsync(Expression<Func<T, bool>> p_filters = null)
        {
            try
            {
                IQueryable<T> query = GetQueryable(p_filters);
                return await query.CountAsync();
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        public virtual async Task InsertOneAsync(T p_entity)
        {
            try
            {
                if (p_entity == null || !await p_entity.ValidateAsync(this.Provider))
                {
                    throw new Exception("Invalid data");
                }

                if (await p_entity.OnInsertingAsync(this.Provider))
                {
                    if (p_entity is LogModel)
                    {
                        LogModel entity = p_entity as LogModel;
                        // entity.CreatedBy = -> Retrieve security context and get userid
                        entity.CreatedOn = DateTime.UtcNow;
                    }

                    await entities.AddAsync(p_entity);
                    await DataContext.SaveChangesAsync();
                    await p_entity.OnInsertedAsync(this.Provider);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        public virtual async Task RemoveOneAsync(T p_entity)
        {
            try
            {
                if (p_entity == null || !await p_entity.ValidateAsync(this.Provider))
                {
                    throw new Exception("Invalid data");
                }

                if (await p_entity.OnDeletingAsync(this.Provider))
                {
                    entities.Remove(p_entity);
                    await DataContext.SaveChangesAsync();
                    await p_entity.OnDeletedAsync(this.Provider);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        public async Task UpdateOneAsync(T p_entity)
        {
            try
            {
                if (p_entity == null || !await p_entity.ValidateAsync(this.Provider))
                {
                    throw new Exception("Invalid data");
                }

                // make all id field with unique name -> Need to validate whether the model exists based on id before updating or based on key
                // Have to retrieve current model
                //if(await p_entity.OnUpdatingAsync(p_current, this.Provider))
                {
                    entities.Update(p_entity);
                    await DataContext.SaveChangesAsync();
                    //await p_entity.OnUpdatedAsync(p_current, this.Provider);
                }
            }
            catch (Exception ex)
            {
                await HandleExceptionAsync(ex);
                throw;
            }
        }

        protected virtual Task HandleExceptionAsync(Exception ex)
        {
            return Task.CompletedTask;
        }
    }
}
