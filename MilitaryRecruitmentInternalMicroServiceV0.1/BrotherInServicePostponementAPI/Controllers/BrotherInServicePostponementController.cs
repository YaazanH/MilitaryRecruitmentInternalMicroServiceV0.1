using BrotherInServicePostponementAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using BrotherInServicePostponementAPI.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;

namespace BrotherInServicePostponementAPI.Controllers
{
    [ApiController]
    [Route("BrotherInServicePostponement")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class BrotherInServicePostponementController : Controller
    {
        private readonly BrotherInServicePostponementContext _context;
        public BrotherInServicePostponementController(BrotherInServicePostponementContext context)
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
        [Route("GetNumberOfRequests")]
        public int GetNumberOfRequests()
        {
            int result = _context.BrotherInServicePostponementDBS.Count();

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
        public Models.BrotherInServicePostponement GetUserPostponment(int id)
        {

            Models.BrotherInServicePostponement result = _context.BrotherInServicePostponementDBS.Where(x => x.UserID == id).FirstOrDefault();


            return result;
        }

        [HttpGet]
        [Route("SearchAllPostponment")]
        public List<Models.BrotherInServicePostponement> GetAllUsersPostponments()
        {

            List<Models.BrotherInServicePostponement> result = _context.BrotherInServicePostponementDBS.ToList();


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
            BrotherInServicePostponement st = new BrotherInServicePostponement { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddYears(1) };
            _context.BrotherInServicePostponementDBS.Add(st);
            _context.SaveChanges();

        }
        public async Task<IActionResult> GetIsAStudent()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.BrotherInServicePostponementDBS.Where(x => x.UserID == CUserID).FirstOrDefault();
            if (User != null)
            {
                if (User.DateOfEnd.DateTime > DateTime.Now)
                {
                    return Ok("You aready have vaild cert");
                }
            }

            List<int> id = JsonConvert.DeserializeObject<List<int>>(await APICall("https://host.docker.internal:40018/RecordsAdminstration/GetStudyYears"));

            foreach (int i in id)
            {
                if (JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40013/DefenseAPI/GetIsInService/?id=" + i.ToString())))
                {
                    AddCert(CUserID);
                    return Ok("cert added");

                    
                }
            }

            return Ok("can't give you a cert :(");

        }*/
    }
}
