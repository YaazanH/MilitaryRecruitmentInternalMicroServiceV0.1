using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;
using CashAllowanceAPI.Data;
using CashAllowanceAPI.Models;
using System.Collections.Generic;

namespace CashAllowanceAPI.Controllers
{
    [ApiController]
    [Route("CashAllowance")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CashAllowanceController : Controller
    {
        private readonly CashAllowanceContext _context;
        public CashAllowanceController(CashAllowanceContext context)
        {
            _context = context;
        }
        private int GetCurrentUserID()
        {
            var identity = HttpContext.User.Identity as ClaimsIdentity;
            if (identity != null)
            {
                return Int32.Parse(identity.Claims.FirstOrDefault(o => o.Type == ClaimTypes.PrimarySid)?.Value);
            }
            return 0;
        }

        [HttpGet]
        [Route("GetAllUserTransactions")]
        public List<RequestStatues> GetAllUserTransactions()
        {
            int CUserID = GetCurrentUserID();
            List<RequestStatues> result = _context.RequestStatuesDBS.Where(x => x.UserID == CUserID).OrderByDescending(x => x.DateOfRecive).Take(10).ToList<RequestStatues>();
            return result;
        }

        [HttpGet]
        [Route("GetAUserTransactions")]
        public Dictionary<string, string> GetAUserTransactions(int Reqid)
        {

            RequestStatues requestStatues = _context.RequestStatuesDBS.Where(x => x.ReqStatuesID == Reqid).FirstOrDefault();
            if (requestStatues != null)
            {


                
                AsyncUserTransactions asyncUserTransactions = _context.AsyncUserTransactionsDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();                
                AsyncAge asyncAge = _context.AsyncAgeDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

                Dictionary<string, string> result = new Dictionary<string, string>();

                result.Add("asyncUserTransactions", asyncUserTransactions.Statues);
                result.Add("asyncAge", asyncAge.Statues);

                return result;
            }
            else
            {
                return null;
            }
        }

        [HttpGet]
        [Route("GetNumberOfRequests")]
        public int GetNumberOfRequests()
        {
            int result = _context.CashAllowanceDb.Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsApproved")]
        public int GetNumberOfRequestsApproved()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "wrong").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsProcessing")]
        public int GetNumberOfRequestsProcessing()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "wating").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsDeleted")]
        public int GetNumberOfRequestsDeleted()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "deleted").Count();

            return result;
        }


        [HttpGet]
        [Route("SearchUserPostponment")]
        public Models.CashAllowance GetUserPostponment(int id)
        {

            Models.CashAllowance result = _context.CashAllowanceDb.Where(x => x.UserID == id).FirstOrDefault();


            return result;
        }

        [HttpGet]
        [Route("SearchAllPostponment")]
        public List<Models.CashAllowance> GetAllUsersPostponments()
        {

            List<Models.CashAllowance> result = _context.CashAllowanceDb.ToList();


            return result;
        }
        /*
        [HttpGet]
        [Route("GetAllUserTransactions")]
        public List<RequestStatues> GetAllUserTransactions()
        {
            int CUserID = GetCurrentUserID();
            List<RequestStatues> result = _context.RequestStatuesDBS.Where(x => x.UserID == CUserID).OrderByDescending(x => x.DateOfRecive).Take(10).ToList<RequestStatues>();
            return result;
        }

        [HttpGet]
        [Route("GetAUserTransactions")]
        public RequestStatues GetAUserTransactions(int Reqid)
        {
            RequestStatues result = _context.RequestStatuesDBS.Where(x => x.ReqStatuesID == Reqid).FirstOrDefault();
            return result;
        }
*/



        /*
        private void AddCert(int CUserID)
        {
            Models.CashAllowance tra = new Models.CashAllowance { UserID = CUserID, DateOfGiven = DateTime.Now };
            _context.CashAllowanceDb.Add(tra);
            _context.SaveChanges();

        }

        private async Task<string> APICall(string GURI)
        {
            var authorization = Request.Headers[HeaderNames.Authorization];

            AuthenticationHeaderValue.TryParse(authorization, out var authentication);

            string Token = "";


            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                Token = headerValue.Parameter;
            }

            Uri uri = new Uri(GURI);

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var Client = new HttpClient(clientHandler))
            {
                Client.BaseAddress = uri;
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                using (HttpResponseMessage response = await Client.GetAsync(Client.BaseAddress))
                {
                    if (response.IsSuccessStatusCode)
                    {
                        return response.Content.ReadAsStringAsync().Result;
                    }
                    else
                    {
                        return null;
                    }
                }
            }
        }



        [HttpGet]
        [Route("GetHasCashAllowance/")]

        public async Task<IActionResult> GetAbleToHaveCashAllowance()
        {
            int CUserID = GetCurrentUserID();
          
                var User = _context.CashAllowanceDb.Where(x => x.UserID == CUserID).FirstOrDefault();
                if (User != null)
                {
          
                return Ok("You aready have vaild cert");

                }

                int Age = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/RecordAdminstration/GetAge"));

            if (Age == 42)
            {
                if (JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40022/Finance/GetUserTransactions")))
                {
                    AddCert(GetCurrentUserID());
                    return Ok("Congratiolations!");
                }
            }

                switch (Age)
            {
                case 43:
                 return Ok("You Need To Pay Extra 200$");

                case 44:                  
                return Ok("You Need To Pay Extra 400$");

                case 45:
                   
                    
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 600$");
                   
                    break;
                case 46:
                    
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 800$");
                    
                    break;
                case 47:
                    
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1000$");
                    
                    break;
                case 48:
                   
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1200$");
                    
                    break;
                case 49:
                   
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1400$");
                    
                    break;
                case 50:
                   
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1600$");
                    
                    break;
                case 51:
                      AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1800$");
                    
                    break;
                case 52:
                    
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 2000$");
                    
                    break;

                default:
                    return NotFound();
            }

            return NoContent();
        }*/
    }
}