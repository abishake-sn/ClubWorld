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
using Microsoft.Extensions.Logging;

namespace ClubWorldWeb.WebApi.Controllers.Transaction
{

    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme + ",Query")]
    [Route("api/ledger")]

    public class LedgerController : ClubWorldWebApiController<TrnLedger, ITrnLedgerRepository>
    {
        private readonly IHostingEnvironment m_hostingEnvironment;

        public LedgerController(IHostingEnvironment p_hostingEnvironment)
        {
            m_hostingEnvironment = p_hostingEnvironment;
        }

        [HttpGet("get-ledger/{p_membercode}")]
        public async Task<TrnLedger> GetLedgerByIdAsync(string p_ledgerId)
        {
            if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
            {
                int ledgerId = int.Parse(p_ledgerId);
                TrnLedger led = await this.Repository.GetByIdAsync(ledgerId);
                return led;
            }
            return null;
        }

        
        [HttpPost("save-payment")]
        public async Task<ClubWorldWebActionResult> SavePayment([FromBody] TrnOnlinePayment p_payment)
        {
            ClubWorldWebActionResult authResult = new ClubWorldWebActionResult();
            try
            {
                List<string> validationErrors = await IsValidPayment(p_payment);
                if (validationErrors.Count == 0)
                {
                    int VouNo = 0;
                    ITrnOnlinePaymentRepository paymentRepo = this.Provider.GetService<ITrnOnlinePaymentRepository>();
                    VouNo = paymentRepo.GetVoucherNo();

                    p_payment.VouNo = VouNo;
                    p_payment.VouDate = System.DateTime.Now;
                    p_payment.ActiveStatus = TEMDictionary.ActiveStaus;
                    p_payment.SyncStatus = "";
                    p_payment.CreatedBy = 1;
                    p_payment.CreatedOn = System.DateTime.Now;
                    await paymentRepo.InsertOneAsync(p_payment);

                    if (p_payment.TransactionStatus.ToLower() == "success")
                    {
                        ITrnLedgerRepository TrnLedgerRepo = this.Provider.GetService<ITrnLedgerRepository>();
                        TrnLedger led1 = new TrnLedger();
                        led1.AcYear = p_payment.AcYear;
                        led1.VouType = "ONLINEPAY";
                        led1.VouNo = VouNo;
                        led1.SNo = 1;
                        led1.VouDate = System.DateTime.Now;
                        led1.Ledger1 = p_payment.MemberId;
                        led1.Credit = p_payment.Credit;
                        led1.Debit = p_payment.Debit;
                        led1.Notes = p_payment.Notes;
                        led1.SyncStatus = "";
                       
                        led1.ActiveStatus = TEMDictionary.ActiveStaus;
                        led1.CreatedBy = 1;
                        led1.CreatedOn = System.DateTime.Now;
                        await TrnLedgerRepo.InsertOneAsync(led1);

                        TrnLedger led2 = new TrnLedger();
                        led2.AcYear = p_payment.AcYear;
                        led2.VouType = "ONLINEPAY";
                        led2.VouNo = VouNo;
                        led2.SNo = 1;
                        led2.VouDate = System.DateTime.Now;
                        led2.Ledger1 = p_payment.MemberId;
                        led2.Credit = p_payment.Credit;
                        led2.Debit = p_payment.Debit;
                        led2.Notes = p_payment.Notes;
                        led2.SyncStatus = "";
                        led2.ActiveStatus = TEMDictionary.ActiveStaus;
                        led2.CreatedBy = 1;
                        led2.CreatedOn = System.DateTime.Now;
                        await TrnLedgerRepo.InsertOneAsync(led2);

                    }

                    authResult.result = TEMDictionary.ResultSuccess;
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

        [HttpPost("ledger-upload/{p_ledgerId}")]
        public async Task<AuthResult> LedgerUpload(string p_ledgerId)
        {
            int _id=int.Parse(p_ledgerId);
            AuthResult result = new AuthResult() { ErrorMsgs = new List<string>() };
            try
            {
                if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
                {
                    TrnLedger led = await this.Repository.GetledgerbyExternalId(_id);
                    if(led != null)
                    {
                        led.SyncStatus = "Downloaded";
                        await this.Repository.UpdateOneAsync(led);
                    }
                    result.result = "Sucess";
                }
                else
                {
                    result.result = "Fail";
                    result.ErrorMsgs.Add("Invalid Authorization");
                }
            }
            catch (Exception ex)
            {
                result.result = "Fail";
                result.ErrorMsgs[0] = ex.Message;
            }
            return result;            
        }

        [HttpPost("syncstaus-update")]
        public async Task<AuthResult> SyncStatusUpdate([FromBody] TrnLedger ledger)
        {
            AuthResult result = new AuthResult() { ErrorMsgs = new List<string>() };
            try
            {
                if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
                {
                    TrnLedger led = await this.Repository.GetledgerbyExternalId(ledger.ExternalId);
                    if (led != null)
                    {
                        led.Ledger1 = ledger.Ledger1;
                        led.Ledger2 = ledger.Ledger2;
                        led.Ledger1Name = "";
                        led.Ledger2Name = "";
                        led.Credit = ledger.Credit;
                        led.Balance = ledger.Balance;
                        led.Debit = ledger.Debit;
                        led.Notes = ledger.Notes;
                        led.ActiveStatus = ledger.ActiveStatus;
                        led.PaymentType = ledger.PaymentType;
                        led.UpdatedBy = ledger.UpdatedBy;
                        led.UpdatedOn = System.DateTime.Now;
                        await this.Repository.UpdateOneAsync(led);
                    }
                    else
                    {
                        await this.Repository.InsertOneAsync(led);
                    }

                    result.result = "Sucess";
                }
                else
                {
                    result.result = "Fail";
                    result.ErrorMsgs.Add("Invalid Authorization");
                }
            }
            catch (Exception ex)
            {
                result.result = "Fail";
                result.ErrorMsgs[0] = ex.Message;
            }
            return result;
           
        }

        [HttpPost("get-trnledger-Info")]
        public async Task<List<TrnLedgerRptInfo>> TrnLedgerRpts([FromBody] TrnLedgerRpt p_Rpt)
        {
            if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
            {
                ITrnLedgerRepository trnLedger = this.Provider.GetService<ITrnLedgerRepository>();
                List<TrnLedgerRptInfo> inst = await trnLedger.Rpt_TrnLedgerReport(p_Rpt.LedgerNo1, p_Rpt.FromDate, p_Rpt.ToDate);
                return inst;
            }
            return null;
        }


        private async Task<List<string>> IsValidPayment(TrnOnlinePayment p_Trn, bool isNew = true)
        {
            List<string> ErrorMsgs = new List<string>();
            if (p_Trn == null)
                ErrorMsgs.Add("Invalid data");
            else
            {
                if (p_Trn.MemberId==0)
                    ErrorMsgs.Add("Member Id is required");

                if (p_Trn.Credit == 0)
                    ErrorMsgs.Add("Amount is required");
            }
            return ErrorMsgs;
        }

        [HttpPost("upload-ledger")]
        public async Task<ClubWorldWebActionResult> SaveLedger([FromBody] TrnLedger body_trnLedger)
        {
            ClubWorldWebActionResult authResult = new ClubWorldWebActionResult();
            try
            {
                List<string> validationErrors = await IsValidLedger(body_trnLedger);
                if (validationErrors.Count == 0)
                {
                    if (!string.IsNullOrEmpty(this.SecurityContext.GetUsername()))
                    {
                        TrnLedger led = await this.Repository.GetledgerbyExternalId(body_trnLedger.ExternalId);
                        if (led == null)
                        {

                            body_trnLedger.ActiveStatus = TEMDictionary.ActiveStaus;
                            body_trnLedger.CreatedBy = 1;
                            body_trnLedger.CreatedOn = System.DateTime.Now;
                            await this.Repository.InsertOneAsync(body_trnLedger);
                        }
                        else                         
                        {
                            //TrnLedger trnLedger = await Repository.GetByIdAsync(p_MasMem.id);
                            led.ExternalId = body_trnLedger.ExternalId;
                            led.AcYear = body_trnLedger.AcYear;
                            led.VouType = body_trnLedger.VouType;
                            led.VouNo = body_trnLedger.VouNo;
                            led.VouDate = body_trnLedger.VouDate;
                            led.SNo = body_trnLedger.SNo;
                            led.Ledger1 = body_trnLedger.Ledger1;
                            led.Ledger2 = body_trnLedger.Ledger2;
                            led.Ledger1Name = body_trnLedger.Ledger1Name;
                            led.Ledger2Name = body_trnLedger.Ledger2Name;
                            led.Credit = body_trnLedger.Credit;
                            led.Debit = body_trnLedger.Debit;
                            led.Balance = body_trnLedger.Balance;
                            led.Notes = body_trnLedger.Notes;
                            led.ActiveStatus = body_trnLedger.ActiveStatus;
                            led.SyncStatus = body_trnLedger.SyncStatus;
                            led.PaymentType = body_trnLedger.PaymentType;                        
                            led.UpdatedOn = System.DateTime.Now;
                            led.UpdatedBy = 1;
                            await Repository.UpdateOneAsync(led);
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
        private async Task<List<string>> IsValidLedger(TrnLedger p_Trn, bool isNew = true)
        {
            List<string> ErrorMsgs = new List<string>();
            if (p_Trn == null)
                ErrorMsgs.Add("Invalid data");
            else
            {
                if (p_Trn.ExternalId == 0)
                    ErrorMsgs.Add("ExternalId  is required");

                if (p_Trn.AcYear == 0)
                    ErrorMsgs.Add("AcYear is required");                    

                if (decimal.Equals(p_Trn.Ledger1Name, 0))                
                    ErrorMsgs.Add("Ledger1Name is required");
                
            }
            return ErrorMsgs;
        }


    }
}