using ClubWorldWeb.Controllers;
using ClubWorldWeb.Domains.Models;
using ClubWorldWeb.Domains.Repositories;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Mvc;
using ClubWorldWeb.OAuth;
using ClubWorldWeb.Web.Controllers;

namespace ClubWorldWeb.WebApi.Controllers
{
    public abstract class ClubWorldWebApiController<TModel, TRepository> : ClubWorldWeb.Controllers.Controller
        where TModel : Model
        where TRepository : IRepository<TModel>
    {
        protected TRepository Repository { get; private set; }
        protected ISecurityContext SecurityContext { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext p_context)
        {
            base.OnActionExecuting(p_context);
            this.Repository = this.Provider.GetService<TRepository>();
            this.SecurityContext = this.Provider.GetService<ISecurityContext>();
            this.SecurityContext.RequestUri = p_context.HttpContext.Request.GetUri();
        }

        [HttpGet("get-all")]
        public virtual Task<List<TModel>> GetAll([FromQuery(Name = "init")]bool p_init = true)
        {
            return Repository.GetAllAsync(p_init: p_init);
        }

        [HttpGet("get-by-id/{p_id}")]
        public virtual Task<TModel> GetById(string p_id, [FromQuery(Name = "init")]bool p_init = true)
        {
            int _id;
            if (!int.TryParse(p_id, out _id))
                return Task.FromResult(default(TModel));

            return Repository.GetByIdAsync(_id, p_init: p_init);
        }

        [HttpPost("get-by-ids")]
        public virtual Task<List<TModel>> GetByIds([FromBody()]string[] p_ids, [FromQuery(Name = "init")]bool p_init = true)
        {
            if (p_ids == null || p_ids.Length == 0)
            {
                return Task.FromResult<List<TModel>>(null);
            }

            List<int> ids = new List<int>();
            foreach (string strId in p_ids)
            {
                int id;
                if (!int.TryParse(strId, out id) || ids.Contains(id) || id <= 0)
                {
                    continue;
                }

                ids.Add(id);
            }

            if (ids.Count == 0)
                return Task.FromResult(default(List<TModel>));

            return Repository.GetByIdsAsync(ids, p_init: p_init);
        }

        [HttpPost("insert")]
        public virtual async Task<TModel> Insert([FromBody()] TModel p_model)
        {
            await Repository.InsertOneAsync(p_model);
            return await Repository.GetByIdAsync(p_model.id);
        }

        [HttpPut("update")]
        public virtual async Task<TModel> Update([FromBody()] TModel p_model)
        {
            await Repository.UpdateOneAsync(p_model);
            return await Repository.GetByIdAsync(p_model.id);
        }

        [HttpDelete("delete/{p_id}")]
        public virtual async Task Delete(string p_id)
        {
            int _id;
            if (!int.TryParse(p_id, out _id))
                return;

            TModel model = await Repository.GetByIdAsync(_id);
            if (model == null)
                return;

            await Repository.RemoveOneAsync(model);
        }
    }
}

