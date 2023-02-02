using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http;
using System.Security.Claims;
using System.Threading.Tasks;
using FixedServiceAllowanceAPI.Models;
using FixedServiceAllowanceAPI.Data;

namespace FixedServiceAllowanceAPI.Controllers
{
    [ApiController]
    [Route("FixedServiceAllowance")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FixedServiceAllowanceController : Controller
    {
        private readonly FixedServiceAllowanceContext _context;
        public FixedServiceAllowanceController(FixedServiceAllowanceContext context)
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
        [Route("GetAllTransactions/")]
        [Authorize(Roles = "Admin")]
        public List<RequestStatues> GetAllTransactions()
        {
            List<RequestStatues> result = _context.RequestStatuesDBS.OrderByDescending(x => x.DateOfRecive).ToList<RequestStatues>();
            return result;
        }

        [HttpGet]
        [Route("GetAllUserTransactions/")]
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
            Dictionary<string, string> result = new Dictionary<string, string>();
            RequestStatues requestStatues = _context.RequestStatuesDBS.Where(x => x.ReqStatuesID == Reqid).FirstOrDefault();
            if (requestStatues != null)
            {


                AsyncFixedService asyncFixedService = _context.AsyncFixedServiceDB.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();


                

                result.Add("asyncFixedService", asyncFixedService.Statues);
                return result;
            }
            else
            {
                return result;
            }
        }


        [HttpGet]
        [Route("GetNumberOfRequests")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequests()
        {
            int result = _context.FixedServiceAllowanceContextDBS.Count();
            result += _context.RequestStatuesDBS.Where(x => x.Statues == "wating" || x.Statues == "Faild").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsApproved")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequestsApproved()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Done").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsProcessing")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequestsProcessing()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "wating").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsFaild")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequestsFaild()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Faild").Count();

            return result;
        }


        [HttpGet]
        [Route("SearchUserPostponment")]
        public Models.FixedServiceAllowance GetUserPostponment(int id)
        {

            Models.FixedServiceAllowance result = _context.FixedServiceAllowanceContextDBS.Where(x => x.UserId == id).FirstOrDefault();


            return result;
        }

        [HttpGet]
        [Route("SearchAllPostponment")]
        public List<Models.FixedServiceAllowance> GetAllUsersPostponments()
        {

            List<Models.FixedServiceAllowance> result = _context.FixedServiceAllowanceContextDBS.ToList();


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

        private void AddCert(int CUserID)
        {
            FixedServiceAllowance st = new FixedServiceAllowance { id = CUserID, DateOfGiven = DateTime.Now };
            _context.FixedServiceAllowanceContextDBS.Add(st);
            _context.SaveChanges();

        }
        [HttpGet]
        [Route("GetIsHasBrotherInService")]
        public async Task<IActionResult> GetIsHasBrotherInService()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.FixedServiceAllowanceContextDBS.Where(x => x.id == CUserID).FirstOrDefault();
            if (User != null)
            {
                if (User!=null)
                {
                    return Ok("You aready have vaild cert");
                }
            }

       
            if (JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40013/DefenseAPI/GetIsFixed/?id=" + CUserID.ToString())))
            {
                AddCert(CUserID);
                return Ok("cert added");
            }
            else 
            {
                return Ok("can't give you a cert :(");
            }
           
                    

        }*/
    }
}
