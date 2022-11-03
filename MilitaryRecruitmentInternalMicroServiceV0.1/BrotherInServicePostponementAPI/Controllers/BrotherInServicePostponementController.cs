using BrotherInServicePostponementAPI.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using SchoolPostponementAPI.Data;
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
    [Route("SchoolPostponement")]
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

        }
    }
}
