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
        }
    }
}

