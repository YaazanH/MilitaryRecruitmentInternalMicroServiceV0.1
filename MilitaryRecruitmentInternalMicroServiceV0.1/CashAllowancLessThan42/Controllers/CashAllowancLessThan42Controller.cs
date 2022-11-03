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
using CashAllowancLessThan42.Data;
using CashAllowancLessThan42.Models;

namespace CashAllowancLessThan42.Controllers
{

    [ApiController]
    [Route("CashAllowanceLessThan42")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CashAllowancLessThan42Cotroller : Controller
    {
        private readonly CashAllowancLessThan42Context _context;
        public CashAllowancLessThan42Cotroller(CashAllowancLessThan42Context context)
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
            CashAllowancLessThan42Model tra = new CashAllowancLessThan42Model { UserID = CUserID, DateOfGiven = DateTime.Now };
            _context.CashAllowancLessThan42Db.Add(tra);
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
        [Route("GetHasCashAllowanceForLessThan42/")]

        public async Task<IActionResult> GetAbleToHaveCashAllowanceFLT42()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.CashAllowancLessThan42Db.Where(x => x.UserID == CUserID).FirstOrDefault();
            if (User != null)
            {

                return Ok("You aready have vaild cert");

            }

            int Age = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/RecordAdminstration/GetAge"));

            if (Age >= 42)
            {
                return Ok("You Cant Postponement");
            }

            else
            {
                if (!JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40011/Passport/GetIstravel")))
                {
                    if (JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40022/Finance/GetUserTransactions")))
                    {
                        AddCert(GetCurrentUserID());
                        return Ok("Congratiolations!");
                    }
                }
            }




            int Days = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40011/Passport/GetNumberOfDaysOutsideCoun"));

          
                    if (Days >= 1460)
                    {

                        return Ok("You Need To Pay  7000$");
                    }
                    
                
                    if (Days > 1095 && Days < 1460)
                    {

                        return Ok("You Need To Pay  8000$");
                    }
              
                    if (Days > 730 && Days < 1095)
                    {

                        return Ok("You Need To Pay 9000$");
                    }
              
           
                    if (Days > 365 && Days < 730)
                    {
                       
                        return Ok("You Need To Pay 10000$");
                    }
                

           

          
            
           

            return NoContent();
        }
    }
}