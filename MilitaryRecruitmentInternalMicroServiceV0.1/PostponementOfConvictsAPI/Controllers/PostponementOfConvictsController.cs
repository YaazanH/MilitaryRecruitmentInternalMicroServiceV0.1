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
using PostponementOfConvictsAPI.Data;
using PostponementOfConvictsAPI.Models;
using System.Collections.Generic;

namespace PostponementOfConvictsAPI.Controllers
{
    [ApiController]
    [Route("PostponementOfConvicts")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class PostponementOfConvictsController : Controller
    {
        private readonly PostponementOfConvictsContext _context;
        public PostponementOfConvictsController(PostponementOfConvictsContext context)
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


                AsyncEntryDate asyncEntryDate = _context.AsyncEntryDateDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                AsyncInJail asyncInJail = _context.AsyncInJailDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                AsyncYearsRemaning asyncYearsRemaning = _context.AsyncYearsRemaningDb.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                

                

                result.Add("asyncEntryDate", asyncEntryDate.Statues);
                result.Add("asyncInJail", asyncInJail.Statues);
                result.Add("asyncYearsRemaning", asyncYearsRemaning.Statues);
                

                return result;
            }
            else
            {
                return result;
            }
        }

        [HttpGet]
        [Route("GetNumberOfRequests")]
        public int GetNumberOfRequests()
        {
            int result = _context.PostponementOfConvictsDb.Count();
            result += _context.RequestStatuesDBS.Where(x => x.Statues == "wating" || x.Statues == "Faild").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsApproved")]
        public int GetNumberOfRequestsApproved()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Done").Count();

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
        [Route("GetNumberOfRequestsFaild")]
        public int GetNumberOfRequestsFaild()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Faild").Count();

            return result;
        }


        [HttpGet]
        [Route("SearchUserPostponment")]
        public Models.PostponementOfConvicts GetUserPostponment(int id)
        {

            Models.PostponementOfConvicts result = _context.PostponementOfConvictsDb.Where(x => x.UserID == id).FirstOrDefault();


            return result;
        }

        [HttpGet]
        [Route("SearchAllPostponment")]
        public List<Models.PostponementOfConvicts> GetAllUsersPostponments()
        {

            List<Models.PostponementOfConvicts> result = _context.PostponementOfConvictsDb.ToList();


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
                PostponementOfConvicts tra = new PostponementOfConvicts { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddMonths(6) };
                _context.PostponementOfConvictsDb.Add(tra);
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
        [Route("GetIsPrisoner/")]
        public async Task<IActionResult> GetIsPrisoner()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.PostponementOfConvictsDb.Where(x => x.UserID == CUserID).FirstOrDefault();
            bool Prisoner = JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40015/Jail/GetIfInJail"));
            if(Prisoner == true)
            {
                return Ok("Is a Prisoner");
            }
            else 
            {
                return Ok("Isn't a Prisoner");
            }
        }

        [HttpGet]
        [Route("GetIsCondemned/")]
        public async Task<IActionResult> GetIsCondemned()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.PostponementOfConvictsDb.Where(x => x.UserID == CUserID).FirstOrDefault();
            int Condemned = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40012/Court/GetYearsRemaining"));
            if (Condemned > 0)
            {
                return Ok("Is Condemned");
            }
            else
            {
                return Ok("Isn't Condemned");
            }
        }

        [HttpGet]
        [Route("GetIsConfined/")]
        public async Task<IActionResult> GetIsConfined()
        {
            DateTimeOffset x=DateTimeOffset.Now;
            int CUserID = GetCurrentUserID();
            var User = _context.PostponementOfConvictsDb.Where(x => x.UserID == CUserID).FirstOrDefault();
            DateTimeOffset Confined = JsonConvert.DeserializeObject<DateTimeOffset>(await APICall("https://host.docker.internal:40016/CompetentAuthority/GetEntryDate"));
            return Ok(Confined);
        }*/
    }
}

