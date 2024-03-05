using ClubWorldWeb.OAuth;
using Microsoft.AspNetCore.Authentication;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;

namespace ClubWorldWeb.Web.Controllers
{
    public class QueryAuthenticationOptions : AuthenticationSchemeOptions
    {
        public string Audience { get; set; }
        public string Authority { get; set; }
        public string TokenKey { get; set; }
    }

    public class QueryAuthenticationHandler : AuthenticationHandler<QueryAuthenticationOptions>
    {
        /// <summary>
        /// 
        /// </summary>
        /// <param name="options"></param>
        /// <param name="logger"></param>
        /// <param name="encoder"></param>
        /// <param name="clock"></param>
        public QueryAuthenticationHandler(IOptionsMonitor<QueryAuthenticationOptions> options, ILoggerFactory logger, UrlEncoder encoder, ISystemClock clock) : base(options, logger, encoder, clock)
        {
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override Task<AuthenticateResult> HandleAuthenticateAsync()
        {
            OAuth.SecurityContext securityContext = (OAuth.SecurityContext)this.Context.RequestServices.GetService(typeof(ISecurityContext));
            if (securityContext != null && securityContext.Principal != null)
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            string tokenKey = !string.IsNullOrEmpty(this.Options.TokenKey) ? this.Options.TokenKey : "t";
            string token;
            if (this.Context.User.Identity.IsAuthenticated
                || !this.Context.Request.Query.ContainsKey(tokenKey)
                || string.IsNullOrEmpty(token = this.Context.Request.Query["t"])
                )
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            JwtSecurityTokenHandler handler = new JwtSecurityTokenHandler();
            JwtSecurityToken jwtToken = handler.ReadToken(token) as JwtSecurityToken;
            if (jwtToken == null
                // Validate valid to
                || jwtToken.ValidTo < DateTime.UtcNow
                // Validate audience
                || (!string.IsNullOrEmpty(this.Options.Audience)
                && !jwtToken.Audiences.Any(i => i.Equals(this.Options.Audience, StringComparison.OrdinalIgnoreCase))
                )
                // Validate issuer
                || (!string.IsNullOrEmpty(this.Options.Authority)
                && !this.Options.Authority.Equals(jwtToken.Issuer, StringComparison.OrdinalIgnoreCase)
                )
                )
            {
                return Task.FromResult(AuthenticateResult.NoResult());
            }

            ClaimsIdentity identity = new ClaimsIdentity(jwtToken.Claims, this.Scheme.Name);
            ClaimsPrincipal principal = securityContext.Principal = new ClaimsPrincipal(identity);
            AuthenticationTicket ticket = new AuthenticationTicket(principal, this.Scheme.Name);
            return Task.FromResult(AuthenticateResult.Success(ticket));
        }
    }
}
