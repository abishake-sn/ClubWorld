using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClubWorldWeb.Controllers
{
    public abstract class Controller : Microsoft.AspNetCore.Mvc.Controller
    {
        public IServiceProvider Provider { get; private set; }

        public override void OnActionExecuting(ActionExecutingContext p_context)
        {
            base.OnActionExecuting(p_context);
            this.Provider = p_context.HttpContext.RequestServices;
        }

        public override async Task OnActionExecutionAsync(ActionExecutingContext p_context, ActionExecutionDelegate p_next)
        {
            await base.OnActionExecutionAsync(p_context, p_next);
        }
    }
}
