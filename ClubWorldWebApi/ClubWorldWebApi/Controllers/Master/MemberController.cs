using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ClubWorldWeb.Domains;
using ClubWorldWeb.Domains.Common;
using ClubWorldWeb.Domains.Models.Master;
using ClubWorldWeb.Domains.Repositories.Master;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.DependencyInjection;
using IO = System.IO;
using Microsoft.Extensions.Options;
using ClubWorldWeb.Domains.Models.Config;
using Microsoft.AspNetCore.Identity;
using ClubWorldWeb.Domains.Repositories.Config;
using ClubWorldWeb.Domains.Models.Transaction;
using ClubWorldWeb.Domains.Repositories.Transaction;

namespace ClubWorldWeb.WebApi.Controllers.Master
{
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",Query")]
    [Route("api/member")]

    public class MemberController : ClubWorldWebApiController<MasMember, IMasMemberRepository>
    {
        private readonly IHostingEnvironment m_hostingEnvironment;

        public MemberController(IHostingEnvironment p_hostingEnvironment)
        {
            m_hostingEnvironment = p_hostingEnvironment;
        }

        [HttpPost("upload-MemberCheckIn")]
        public async Task<ClubWorldWebActionResult> SaveMemberCheckIn([FromBody] TrnMemberCheckIn body_trnMemCheckIn)
        {
            ClubWorldWebActionResult authResult = new ClubWorldWebActionResult();
            try
            {
                List<string> validationErrors = await IsValidCheckIn(body_trnMemCheckIn);
                if (validationErrors.Count == 0)
                {
                    if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
                    {
                        ITrnMemberCheckInRepository memberCheckInRepository = this.Provider.GetService<ITrnMemberCheckInRepository>();
                        TrnMemberCheckIn trnMemberCheck = await memberCheckInRepository.GetCheckInbyExternalId(body_trnMemCheckIn.ExternalId);
                        if (trnMemberCheck == null)
                        {
                            
                            trnMemberCheck.Isactive = TEMDictionary.Isactive;
                            trnMemberCheck.CreatedBy = 1;
                            trnMemberCheck.CreatedOn = System.DateTime.Now;
                            await memberCheckInRepository.InsertOneAsync(trnMemberCheck);
                        }
                        else
                        {                           
                            trnMemberCheck.AcYear = body_trnMemCheckIn.AcYear;
                            trnMemberCheck.MemberId = body_trnMemCheckIn.MemberId;
                            trnMemberCheck.DependantId = body_trnMemCheckIn.DependantId;
                            trnMemberCheck.CheckInTime = body_trnMemCheckIn.CheckInTime;
                            trnMemberCheck.CheckOutTime = body_trnMemCheckIn.CheckOutTime;
                            trnMemberCheck.EntryType = body_trnMemCheckIn.EntryType;
                            trnMemberCheck.FacilityId = body_trnMemCheckIn.FacilityId;
                            trnMemberCheck.Isactive = body_trnMemCheckIn.Isactive;
                            trnMemberCheck.ExternalId = body_trnMemCheckIn.ExternalId;                            
                            trnMemberCheck.UpdatedOn = System.DateTime.Now;
                            trnMemberCheck.UpdatedBy = 1;
                            await memberCheckInRepository.UpdateOneAsync(trnMemberCheck);
                        }
                        authResult.result = TEMDictionary.ResultSuccess;
                    }
                    else
                    {
                        authResult.result = TEMDictionary.Error.ResultFailed;
                        authResult.ErrorMsgs.Add("Invalid Authorization");
                    }
                }
                else
                {
                    authResult.result = TEMDictionary.Error.ResultFailed;
                    authResult.ErrorMsgs = validationErrors;
                }
            }
            catch (Exception ex)
            {
                authResult.result = TEMDictionary.Error.ResultFailed;
                authResult.ErrorMsgs.Add(ex.Message);
            }

            return authResult;
        }

        private async Task<List<string>> IsValidCheckIn(TrnMemberCheckIn p_Trn, bool isNew = true)
        {
            List<string> ErrorMsgs = new List<string>();
            if (p_Trn == null)
                ErrorMsgs.Add("Invalid data");
            else
            {
                if (p_Trn.ExternalId == 0)
                    ErrorMsgs.Add("ExternalId  is required");

                if (p_Trn.MemberId == 0)
                    ErrorMsgs.Add("MemberId is required");

                if (int.Equals(p_Trn.AcYear, 0))
                    ErrorMsgs.Add("AcYear is required");

            }
            return ErrorMsgs;
        }

        //    [HttpGet("get-member-info/{p_membId}")]
        //    public async Task<MasMember> GetmemberByIdAsync(string p_membId)
        //    {
        //        if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
        //        {
        //            int membId = int.Parse(p_membId);
        //            MasMember inst = await this.Repository.GetByIdAsync(membId);
        //            return inst;
        //        }
        //        return null;
        //    }

        //    [HttpGet("get-member-bymemcode/{p_memCode")]
        //    public async Task<List<MasMember>> GetMemberByMemCode(string p_memcode)
        //    {
        //        if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
        //        {

        //            List<MasMember> inst = await this.Repository.GetMemberByMemCode(p_memcode);
        //            return inst;
        //        }
        //        return null;
        //    }
        //    [HttpGet("get-member-bymemType/{p_memType")]

        //    public async Task<List<MasMember>> GetMemByMemType(string p_memtype)
        //    {
        //        if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
        //        {

        //            List<MasMember> inst = await this.Repository.GetMemByMemType(p_memtype);
        //            return inst;
        //        }
        //        return null;
        //    }



        //    [HttpPost("remove/{p_Id}")]
        //    public async Task<AuthResult>RemoveUnit(string param_Id)
        //    {
        //        int _id = int.Parse(param_Id);
        //        AuthResult result = new AuthResult() { ErrorMsgs = new List<string>() };
        //        try
        //        {
        //            if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
        //            {
        //                Int16 isactive = 0;
        //                MasMember MasMem = await this.Repository.GetByIdAsync(_id);
        //                MasMem.Isactive = isactive;
        //                MasMem.UpdatedOn = System.DateTime.Now;
        //                MasMem.UpdatedBy = this.SecurityContext.GetUsername();

        //                await this.Repository.UpdateOneAsync(MasMem);
        //                result.result = "Sucess";
        //            }
        //            else
        //            {
        //                result.result = "Fail";
        //                result.ErrorMsgs.Add("Invalid Authorization");
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            result.result = "Fail";
        //            result.ErrorMsgs[0] = ex.Message;
        //        }
        //        return result;
        //    ///
        //    }
        //    [HttpPost("save-member")]
        //    public async Task<ClubWorldWebActionResult> SaveMember([FromBody] MasMember p_MasMem)
        //    {
        //        ClubWorldWebActionResult authResult = new ClubWorldWebActionResult();
        //        try
        //        {
        //            List<string> validationErrors = await IsValid(p_MasMem);
        //            if (validationErrors.Count == 0)
        //            {
        //                if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
        //                {
        //                    IMasMemberRepository MasMemRepo = this.Provider.GetService<IMasMemberRepository>();
        //                    if (p_MasMem.id == 0)
        //                    {
        //                        p_MasMem.ActiveStatus = TEMDictionary.ActiveStaus;
        //                        p_MasMem.CreatedBy = TEMDictionary.AdminUser;
        //                        p_MasMem.CreatedOn = System.DateTime.Now;
        //                        await MasMemRepo.InsertOneAsync(p_MasMem);
        //                    }
        //                    else
        //                    {
        //                        MasMember Mem = await MasMemRepo.GetByIdAsync(p_MasMem.id);
        //                        Mem.MemberCode = p_MasMem.MemberCode;
        //                        Mem.MemberName = p_MasMem.MemberName;
        //                        Mem.MemberType = p_MasMem.MemberType;
        //                        Mem.SmartCardNo = p_MasMem.SmartCardNo;
        //                        Mem.EmailId = p_MasMem.EmailId;
        //                        Mem.MobileNo = p_MasMem.MobileNo;
        //                        Mem.AltMobileNo = p_MasMem.AltMobileNo;
        //                        Mem.DOJ = p_MasMem.DOJ;
        //                        Mem.DOB = p_MasMem.DOB;
        //                        Mem.DOM = p_MasMem.DOM;
        //                        Mem.ValidTo = p_MasMem.ValidTo;
        //                        Mem.TallyId = p_MasMem.TallyId;
        //                        Mem.AdhaarNo = p_MasMem.AdhaarNo;
        //                        Mem.PAN = p_MasMem.PAN;
        //                        Mem.Passport = p_MasMem.Passport;
        //                        Mem.CarNo = p_MasMem.CarNo;
        //                        Mem.BloodGroup = p_MasMem.BloodGroup;
        //                        Mem.Refferer1 = p_MasMem.Refferer1;
        //                        Mem.Refferer2 = p_MasMem.Refferer2;
        //                        Mem.CreditLimit = p_MasMem.CreditLimit;
        //                        Mem.MonthlyBill = p_MasMem.MonthlyBill;
        //                        Mem.BioMacId = p_MasMem.BioMacId;
        //                        Mem.BioMacCode = p_MasMem.BioMacCode;
        //                        Mem.Education = p_MasMem.Education;
        //                        Mem.CompanyName = p_MasMem.CompanyName;
        //                        Mem.Occupation = p_MasMem.Occupation;
        //                        Mem.Designation = p_MasMem.Designation;
        //                        Mem.Gender = p_MasMem.Gender;
        //                        Mem.MaritalStatus = p_MasMem.MaritalStatus;
        //                        Mem.ActiveStatus = p_MasMem.ActiveStatus;
        //                        Mem.Isactive = p_MasMem.Isactive;
        //                        Mem.WhatsAppNo = p_MasMem.WhatsAppNo;///////////////////////////////////////////////////////////////////////////
        //                        Mem.BillingEmailId = p_MasMem.BillingEmailId;

        //                        Mem.GSTNo = p_MasMem.GSTNo;
        //                        Mem.Remarks = p_MasMem.Remarks;
        //                        Mem.IsSMS = p_MasMem.IsSMS;
        //                        Mem.Grade = p_MasMem.Grade;
        //                        Mem.AffliatedClub = p_MasMem.AffliatedClub;
        //                      ///////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////////
        //                        Mem.UpdatedOn = System.DateTime.Now;
        //                        Mem.UpdatedBy = this.SecurityContext.GetUsername();
        //                        await MasMemRepo.UpdateOneAsync(Mem);///
        //                    }
        //                    authResult.result = TEMDictionary.ResultSuccess;
        //                }
        //                else
        //                {
        //                    authResult.result = TEMDictionary.Error.ResultFailed;
        //                    authResult.ErrorMsgs.Add("Invalid Authorization");
        //                }
        //            }
        //            else
        //            {
        //                authResult.result = TEMDictionary.Error.ResultFailed;
        //                authResult.ErrorMsgs = validationErrors;
        //            }
        //        }
        //        catch (Exception ex)
        //        {
        //            authResult.result = TEMDictionary.Error.ResultFailed;
        //            authResult.ErrorMsgs.Add(ex.Message);
        //        }
        //        return authResult;
        //    }


        //private async Task<List<string>> IsValid(MasMember p_fund, bool isNew = true)
        //{
        //    List<string> ErrorMsgs = new List<string>();


        //    if (p_fund == null)
        //        ErrorMsgs.Add("Invalid data");
        //    else
        //    {
        //        if (string.IsNullOrEmpty(p_fund.MemberCode))
        //            ErrorMsgs.Add("MemberCode is required");

        //        if (string.IsNullOrEmpty(p_fund.MemberName))
        //            ErrorMsgs.Add("Member Name is required");
        //    }

        //    //    return ErrorMsgs;
        //    //}

    }
}


