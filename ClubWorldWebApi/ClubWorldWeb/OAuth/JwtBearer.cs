using Microsoft.AspNetCore.Authentication.JwtBearer;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace ClubWorldWeb.OAuth
{
    public static class JwtBearer
    {
        public const string Audience = "BearerAuthentication:Audience";
        public const string Authority = "BearerAuthentication:Authority";
        public const string JwtKey = "BearerAuthentication:JwtKey";
        public const string NameClaimType = "sub";
        public const string RoleClaimType = "role";
        public static readonly JwtBearerEvents Events = new JwtBearerEvents()
        {
            OnAuthenticationFailed = context =>
            {
                return Task.FromResult(0);
            },
            OnMessageReceived = context =>
            {
                return Task.FromResult(0);
            },
            OnChallenge = context =>
            {
                return Task.FromResult(0);
            },
            OnTokenValidated = context =>
            {
                SecurityContext securityContext = (SecurityContext)context.HttpContext.RequestServices.GetService(typeof(ISecurityContext));
                if (securityContext != null)
                    securityContext.Principal = context.Principal;

                return Task.FromResult(0);
            }
        };
    }
}
