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
using ObligatoryServiceAPI.Data;
using ObligatoryServiceAPI.Models;


namespace ObligatoryServiceAPI.Controllers
{
    [ApiController]
    [Route("ObligatoryService")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class ObligatoryServiceController : Controller
    {
        private readonly ObligatoryServiceContext _context;
        public ObligatoryServiceController(ObligatoryServiceContext context)
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
            ObligatoryService tra = new ObligatoryService { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddMonths(6) };
            _context.ObligatoryServiceDB.Add(tra);
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
        [Route("HasObliatoryService/")]
        public async Task<IActionResult> HasObliatoryService()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.ObligatoryServiceDB.Where(x => x.UserID == CUserID).FirstOrDefault();
            bool person = JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40005/BloodBank/HasDonated"));
            if (person == false)
            {
                return Ok("You need to donate blood");
            }

            AddCert(CUserID);
            return Ok("You Have Obligatory Service");
        }*/
    }
}
