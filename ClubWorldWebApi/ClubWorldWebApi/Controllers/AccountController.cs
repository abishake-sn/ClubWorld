using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using OpenIddict.Abstractions;
using OpenIddict.Server.AspNetCore;
using ClubWorldWeb.Domains.Common;
using ClubWorldWeb.Domains.Models.Config;
using ClubWorldWeb.Domains.Models.Master;
using ClubWorldWeb.Domains.Repositories.Config;
using ClubWorldWeb.Domains.Repositories.Master;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using Microsoft.Extensions.DependencyInjection;
using ClubWorldWeb.OAuth;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using ClubWorldWeb.Domains;
using Azure.Core;
using System.Security.Cryptography;

namespace ClubWorldWeb.WebApi.Controllers
{
    [Route("api/auth")]
    public class AccountController : ClubWorldWeb.Controllers.Controller
    {
        private readonly SignInManager<ConfigUser> _signInManager;
        private readonly UserManager<ConfigUser> _userManager;
        private readonly IConfiguration _configuration;

        public AccountController(
            UserManager<ConfigUser> userManager,
            SignInManager<ConfigUser> signInManager,
            IConfiguration configuration
            )
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
        }
        [HttpPost("Register-Admin")]
        public async Task<AuthResult> RegisterAdmin([FromBody] Signupmodel p_user)
        {
            AuthResult authResult = new AuthResult();
            try
            {
                authResult.ErrorMsgs = await CanRegisterAdmin(p_user);
                if (authResult.ErrorMsgs.Count == 0)
                {
                    try
                    {
                        ConfigUser user = new ConfigUser()
                        {
                            Email = p_user.EmailId,
                            Name = p_user.Name,
                            PhoneNumber = p_user.MobileNo,
                            UserName = p_user.MobileNo,
                        };
                        IdentityResult result = await _userManager.CreateAsync(user, p_user.Password);
                        if (result.Succeeded)
                        {
                            await _signInManager.SignInAsync(user, false);
                            IConfigUserRoleRepository userRoleRepo = this.Provider.GetService<IConfigUserRoleRepository>();
                            await userRoleRepo.MapAdmin(user.Id);
                            authResult.UserId = user.Id;
                            authResult.Token = await GenerateJwtToken(user);
                            authResult.UserType = TEMDictionary.AdminUser;
                            authResult.result = TEMDictionary.ResultSuccess;
                        }
                        else
                            authResult.ErrorMsgs.Add(result.Errors.First().Description);
                    }
                    catch (Exception ex) { }
                }
            }
            catch (Exception ex)
            {
                authResult.result = TEMDictionary.Error.ResultFailed;
                authResult.ErrorMsgs.Add(ex.Message);
            }
            return authResult;
        }

        //[Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",Query")]
        [HttpPost("Register-Member")]
        public async Task<AuthResult> RegisterMember([FromBody] MasMember p_memb)
        {
            AuthResult authResult = new AuthResult();

            try
            {
                authResult.ErrorMsgs = await CanRegisterMember(p_memb);
                if (authResult.ErrorMsgs.Count == 0)
                {
                    try
                    {
                        IMasMemberRepository masrepo = this.Provider.GetService<IMasMemberRepository>();
                        ConfigUser user = new ConfigUser()
                        {
                            Email = p_memb.EmailId,
                            Name = p_memb.MemberName,
                            PhoneNumber = p_memb.MobileNo,
                            UserName = p_memb.MemberCode.ToLower()
                        };


                        IdentityResult result = await _userManager.CreateAsync(user, p_memb.Password);
                        if (result.Succeeded)
                        {
                            p_memb.MemberCode = p_memb.MemberCode.ToLower();
                            p_memb.ActiveStatus = "Active";
                            p_memb.CreatedBy = 1;
                            await masrepo.InsertOneAsync(p_memb);

                            await _signInManager.SignInAsync(user, false);
                            IConfigUserRoleRepository userRoleRepo = this.Provider.GetService<IConfigUserRoleRepository>();
                            await userRoleRepo.MapMember(user.Id);
                            authResult.Token = await GenerateJwtToken(user);
                            authResult.UserType = TEMDictionary.MemberUser;
                            authResult.result = TEMDictionary.ResultSuccess;
                        }
                        else
                            authResult.ErrorMsgs.Add("Registration failed");
                    }
                    catch (Exception ex) { }
                }
            }
            catch (Exception ex)
            {
                authResult.result = TEMDictionary.Error.ResultFailed;
                authResult.ErrorMsgs.Add(ex.Message);
            }
            return authResult;
        }

        [HttpPost("connect")]
        public async Task<AuthResult> Connect([FromBody] OpenIddictRequest p_request)
        {
            AuthResult authResult = new AuthResult()
            {
                ErrorMsgs = new List<string>()
            };

            #region Validation

            if (p_request == null)
                authResult.ErrorMsgs.Add("Invalid request");
            else
            {
                if (string.IsNullOrEmpty(p_request.Username))
                    authResult.ErrorMsgs.Add("Username is required");

                if (string.IsNullOrEmpty(p_request.Password))
                    authResult.ErrorMsgs.Add("Password is required");
            }
            //IMasMemberRepository masrepo = this.Provider.GetService<IMasMemberRepository>();
            //MasMember Masmemb = await masrepo.GetMemberInfoByMemberCode(p_request.Username);
            //if (Masmemb.FirstLogin == "Yes")
            //{
            //    authResult.result = TEMDictionary.ResetPassword;
            //    return authResult;
            //}



            #endregion

            if (authResult.ErrorMsgs.Count == 0)
            {
                ConfigUser user = await _userManager.FindByNameAsync(p_request.Username);

                if (user != null)
                {
                    var result = await _signInManager.PasswordSignInAsync(p_request.Username, p_request.Password, false, false);

                    if (result.Succeeded)
                    {
                        authResult.Token = await GenerateJwtToken(user);
                    }
                }

                if (string.IsNullOrEmpty(authResult.Token))
                {
                    authResult.result = "Fail";
                    authResult.ErrorMsgs.Add("Invalid username/password");
                }
                else
                {
                    ISecurityContext securityContext = this.Provider.GetService<ISecurityContext>();
                    ConfigRole role = null;
                    if (user != null)
                    {
                        IConfigUserRoleRepository userRoleRepo = this.Provider.GetService<IConfigUserRoleRepository>();
                        role = await userRoleRepo.GetRoleByUserId(user.Id);
                        authResult.UserType = role.RoleName;

                        IMasMemberRepository masMembrepo = this.Provider.GetService<IMasMemberRepository>();
                        MasMember mm = await masMembrepo.GetMemberByMemCode(p_request.Username.ToLower());
                        if (mm != null)
                        {
                            authResult.UserId = mm.id;
                            authResult.UserCode = mm.MemberCode;
                            authResult.UserName = mm.MemberName;
                        }
                        authResult.result = TEMDictionary.ResultSuccess;
                    }
                }
            }
            return authResult;
        }





        private async Task<List<string>> CanRegisterMember(MasMember p_memb)
        {
            List<string> ErrorMsgs = new List<string>();

            if (p_memb == null)
                ErrorMsgs.Add("Invalid data");
            else
            {
                if (string.IsNullOrEmpty(p_memb.MemberCode))
                    ErrorMsgs.Add("MemberCode No is required");
                else
                {
                    ConfigUser user = await _userManager.FindByNameAsync(p_memb.MemberCode);
                    if (user != null)
                        ErrorMsgs.Add("MemberCode already exists");
                }

                //if (string.IsNullOrEmpty(p_memb.password))
                //    ErrorMsgs.Add("Password is required");
            }
            return ErrorMsgs;
        }

        private async Task<List<string>> CanRegisterAdmin(Signupmodel Signupmodel)
        {
            List<string> ErrorMsgs = new List<string>();

            if (Signupmodel == null)
                ErrorMsgs.Add("Invalid data");
            else
            {
                if (string.IsNullOrEmpty(Signupmodel.MobileNo))
                    ErrorMsgs.Add("Mobile No is required");
                else
                {
                    ConfigUser user = await _userManager.FindByNameAsync(Signupmodel.MobileNo);
                    if (user != null)
                        ErrorMsgs.Add("An account is already registered with your Mobile No, Please use another one");
                }
                if (string.IsNullOrEmpty(Signupmodel.Name))
                    ErrorMsgs.Add("Name is required");

                if (string.IsNullOrEmpty(Signupmodel.Password))
                    ErrorMsgs.Add("Password is required");
            }
            return ErrorMsgs;
        }

        private async Task<string> GenerateJwtToken(ConfigUser p_user)
        {
            var claims = new List<Claim>(){

            new Claim(OpenIddictConstants.Claims.Subject, p_user.Id.ToString()),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(OpenIddictConstants.Claims.Name, p_user.Name),
                new Claim(OpenIddictConstants.Claims.ClientId, p_user.Id.ToString()),
                new Claim(OpenIddictConstants.Claims.Username,p_user.Email),
                new Claim(OpenIddictConstants.Claims.PhoneNumber,p_user.PhoneNumber)
            };

            IConfigUserRoleRepository userRoleRepo = this.Provider.GetService<IConfigUserRoleRepository>();
            ConfigRole role = await userRoleRepo.GetRoleByUserId(p_user.Id);

            if (role != null)
            {
                string roleName = role.RoleName.ToLower();

                claims.Add(new Claim("Role", roleName));
                switch (roleName)
                {
                    case "admin":
                        claims.Add(new Claim("IsAdmin", true.ToString()));
                        break;
                    case "ClubAdmin":
                        claims.Add(new Claim("IsMemberAdmin", true.ToString()));
                        break;
                    case "Member":
                        claims.Add(new Claim("IsMember", true.ToString()));
                        break;

                }
            }

            //IMasMemberRepository MasMemRepo = this.Provider.GetService<IMasMemberRepository>();
            //MasMember Mem = await MasMemRepo.GetMemberInfoByMemberCode(p_user.UserName);
            //if (Mem != null)
            //{
            //    claims.Add(new Claim("UID", Mem.id.ToString()));
            //    claims.Add(new Claim("MobileNo", Mem.MobileNo.ToString()));

            //}
            //if (role != null)
            //{
            //    string roleName = role.RoleName.ToLower();
            //    claims.Add(new Claim("Role", roleName));
            //    switch (roleName)
            //    {
            //        case "member":
            //            claims.Add(new Claim("IsMember", true.ToString()));
            //            break;
            //    }
            //}

            // Audience
            claims.Add(new Claim(OpenIddictConstants.Claims.Audience, _configuration[JwtBearer.Audience]));

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration[JwtBearer.JwtKey]));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(Convert.ToDouble(_configuration["JwtExpireDays"]));

            var token = new JwtSecurityToken(
                _configuration[JwtBearer.Authority],
                _configuration[JwtBearer.Audience],
                claims,
                expires: expires,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }
    }
}

