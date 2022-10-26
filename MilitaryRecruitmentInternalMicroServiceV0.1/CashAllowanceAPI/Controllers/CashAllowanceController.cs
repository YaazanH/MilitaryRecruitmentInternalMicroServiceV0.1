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

        private void AddCert(int CUserID)
        {
            CashAllowance tra = new CashAllowance { UserID = CUserID, DateOfGiven = DateTime.Now };
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

        public async Task<IActionResult> GetAbleToHaveTravelApproval()
        {
            int CUserID = GetCurrentUserID();
         



            int Age = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/RecordsAdminstration/GetAge"));

            if (Age == 42)
            {
                if (JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40022/Finance/GetUserTransactions")))
                {
                    return Ok("Congratiolations!");
                }
            }

            switch (JsonConvert.DeserializeObject<string>(await APICall("https://host.docker.internal:40018/RecordsAdminstration/GetAge")))
            {
                case "2":
                    if (Age == 43)
                    {

                        return Ok("You Need To Pay Extra 200$");
                    }
                    break;
                case "3":
                    if (Age == 44)
                    {

                        return Ok("You Need To Pay Extra 400$");
                    }
                    break;
                case "4":
                    if (Age == 45)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 600$");
                    }
                    break;
                case "5":
                    if (Age == 46)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 800$");
                    }
                    break;
                case "6":
                    if (Age == 47)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1000$");
                    }
                    break;
                case "7":
                    if (Age == 48)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1200$");
                    }
                    break;
                case "8":
                    if (Age == 49)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1400$");
                    }
                    break;
                case "9":
                    if (Age == 50)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1600$");
                    }
                    break;
                case "10":
                    if (Age == 41)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 1800$");
                    }
                    break;
                case "11":
                    if (Age >= 52)
                    {
                        AddCert(CUserID);
                        return Ok("You Need To Pay Extra 2000$");
                    }
                    break;

                default:
                    return NotFound();







            }

            return NoContent();
        }
    }
}