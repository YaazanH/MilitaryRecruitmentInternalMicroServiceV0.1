using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using AlonePostponement.Data;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using AlonePostponement.Models;

namespace AlonePostponement.Controllers
{
    [ApiController]
    [Route("AlonePostponement")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlonePostponementController : Controller
    {
        private readonly AlonePostponementContext _context;
        public AlonePostponementController(AlonePostponementContext context)
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
            
           Models.AlonePostponement st = new Models.AlonePostponement { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddYears(3) };
            _context.AlonePostponementDBS.Add(st);
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
        [Route("GetIsAlonePostponement/")]
        public async Task<IActionResult> GetIsAlonePostponement()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.AlonePostponementDBS.Where(x => x.UserID == CUserID).FirstOrDefault();
            if (User != null)
            {
                if (User.DateOfEnd.DateTime > DateTime.Now)
                {
                    return Ok("You aready have vaild cert");
                }
            }
            //check have boy brothers
            if(JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40006/University/GetStudyYears")))
            {
                List<int> BrotherNotDied = new List<int>();
                //add brother id 
                List<int> BrothersID = JsonConvert.DeserializeObject<List<int>>(await APICall("https://host.docker.internal:40006/University/GetStudyYears"));
                foreach (var item in BrothersID)
                {
                    //if any brother death
                    if (!JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40006/University/GetStudyYears?id="+item.ToString())))
                    {
                        BrotherNotDied.Add(item);
                    }
                }
                if (BrotherNotDied.Count>0)
                {
                    foreach (var item in BrotherNotDied)
                    {
                        //check if one of the brother not eill
                        if (!JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40006/University/GetStudyYears?id=" + item.ToString())))
                        {
                            return Ok("you cant because you have brothers");
                        }
                    }
                }
                
            }

            AddCert(CUserID);
            return Ok();
        }
    }
}
