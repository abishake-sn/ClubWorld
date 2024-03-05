using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ClubWorldWeb.Domains.Models;
using ClubWorldWeb.Domains.Models.Config;
using ClubWorldWeb.OAuth;
using ClubWorldWeb.Web.Controllers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Infrastructure;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Logging;
using Microsoft.IdentityModel.Tokens;
using Newtonsoft.Json.Serialization;
namespace ClubWorldWeb.WebApi
{
  public class Startup
  {
    public static IConfigurationRoot ConfigurationRoot { get; set; }
    public IConfiguration Configuration { get; }

    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      // Add Cors
      services.AddCors(o => o.AddPolicy("DotePolicy", builder =>
      {
        builder.AllowAnyOrigin()
               .AllowAnyMethod()
               .AllowAnyHeader();
      }));

      services.Configure<ApiBehaviorOptions>(options =>
            {
              options.SuppressModelStateInvalidFilter = true;
            });

      ConfigurationRoot = ClubWorldWeb.Startup.GetConfigurationRoot(services);

            // ===== Add DbContext ========
            services.AddDbContext<ClubWorldWebDbContext>(options =>
            {
            options.UseSqlServer(Configuration["Data:DefaultConnection:ConnectionString"]);
      });



      ClubWorldWeb.Startup.ConfigureDomainServices(services);
      ClubWorldWeb.Domains.Startup.ConfigureRepositoryServices(services);
      services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();
      services.AddSingleton<IActionContextAccessor, ActionContextAccessor>();

      //services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_1)
      //    .AddJsonOptions(options =>
      //    {
      //        options.SerializerSettings.ContractResolver = new CamelCasePropertyNamesContractResolver();
      //    });

      services.Configure<FormOptions>(x =>
      {
        x.ValueLengthLimit = 1 * 1024 * 1024 * 1024;
        x.MultipartBodyLengthLimit = 1 * 1024 * 1024 * 1024;
        x.MultipartHeadersCountLimit = 1 * 1024 * 1024 * 1024;
      });


      // ===== Add Identity ========
      services.AddIdentity<ConfigUser, Role>()
          .AddEntityFrameworkStores<ClubWorldWebDbContext>()
          .AddDefaultTokenProviders();

      services.Configure<IdentityOptions>(options =>
      {
        options.User.AllowedUserNameCharacters = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789.@_";
        options.Password.RequiredLength = 6;
        options.Password.RequireDigit = false;
        options.Password.RequireLowercase = false;
        options.Password.RequireUppercase = false;
        options.Password.RequireNonAlphanumeric = false;
      });

      // Authentication
      IdentityModelEventSource.ShowPII = true;
      services.AddAuthentication(options =>
      {
        options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
        options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;

      })

      .AddJwtBearer(options =>
      {
        options.RequireHttpsMetadata = false;
        options.SaveToken = true;
        options.TokenValidationParameters = new TokenValidationParameters()
        {
          NameClaimType = JwtBearer.NameClaimType,
          RoleClaimType = JwtBearer.RoleClaimType,
          ValidateActor = false,
          ValidateLifetime = false,
          ValidIssuer = Configuration[JwtBearer.Authority],
          ValidAudience = Configuration[JwtBearer.Audience],
          IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(Configuration[JwtBearer.JwtKey])),
          ClockSkew = TimeSpan.Zero
        };
        options.Events = JwtBearer.Events;
      }).AddScheme<QueryAuthenticationOptions, QueryAuthenticationHandler>("Query", p_options =>
      {
        p_options.Audience = ConfigurationRoot[JwtBearer.Audience];
        p_options.Authority = ConfigurationRoot[JwtBearer.Authority];
        p_options.TokenKey = "t";
      });
      services.AddMvc().AddSessionStateTempDataProvider();
      services.AddSession();
      services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Latest);
      services.AddControllers(options => options.EnableEndpointRouting = false);

    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.

    public void Configure(IApplicationBuilder app, IHostingEnvironment env, ClubWorldWebDbContext dbContext)
    {
      if (env.IsDevelopment())
      {
        app.UseDeveloperExceptionPage();
      }
      else
      {
        app.UseExceptionHandler("/Error");
        app.UseHsts();
      }

      app.UseHttpsRedirection();
      app.UseFileServer();
      app.UseCors("DotePolicy");
      app.UseMvc();

      // ===== Create tables ======
      dbContext.Database.EnsureCreated();
    }
  }
}
