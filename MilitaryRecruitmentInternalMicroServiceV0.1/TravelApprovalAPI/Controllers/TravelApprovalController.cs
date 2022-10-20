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
using TravelApprovalAPI.Data;
using TravelApprovalAPI.Models;

namespace TravelApprovalAPI.Controllers
{

    [ApiController]
    [Route("TravelApproval")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class TravelApprovalController : Controller
    {
        private readonly TravelApprovalContext _context;
        public TravelApprovalController(TravelApprovalContext context)
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
            TravelApproval tra = new TravelApproval { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddMonths(6) };
            _context.TravelApprovalDb.Add(tra);
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
        [Route("GetHasTravelApproval/")]

        public async Task<IActionResult> GetAbleToHaveTravelApproval()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.TravelApprovalDb.Where(x => x.UserID == CUserID).FirstOrDefault();
            if (User != null)
            {
                if (User.DateOfEnd.DateTime > DateTime.Now)
                {
                    return Ok("You aready have vaild cert");
                }
            }



            int Age = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/RecordsAdminstration/GetAge"));

            if (Age >= 42)
            {
                return Ok("You dont need cert");
            }
            else
            {
                //is worker
                if (JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40024/LaborMinAPI/GetIsAWorker")))
                {
                    return Ok("You cant cert because of Your Current Job");
                }
                else
                {

                    //Is clear financially
                    if (!JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40022/Finance/GetUserTransactions")))
                    {
                        return Ok("You cant cert because of Financial Probelms");
                    }
                    else

                    //is in country
                    if (JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40011/Passport/GetIstravel")))
                    {
                        return Ok("You Have Succesfully Postponement!");
                    }
                   
                }
            }

            return NoContent();





        }
    }
}