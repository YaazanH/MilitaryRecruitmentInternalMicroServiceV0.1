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
using UserTransactions.Models;


namespace UserTransactions.Controllers
{
    [ApiController]
    [Route("UserTransactions")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserTransactionsController : Controller
    {
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
        [HttpGet]
        [Route("GetAllUserTransactions")]
        public async Task<List<RequestStatues>> GetAllUserTransactions()
        {
            List<RequestStatues> requestStatues = new List<RequestStatues>();
            List<RequestStatues> AlonePostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> BrotherInServicePostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> CashAllowancetponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> CashAllowancLessThan42ponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> FixedServiceAllowanceponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> ObligatoryServiceponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> SchoolPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> TravelApprovalPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));
            List<RequestStatues> ConvictsPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:600"));

            requestStatues.Concat(AlonePostponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(BrotherInServicePostponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(CashAllowancetponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(CashAllowancLessThan42ponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(FixedServiceAllowanceponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(ObligatoryServiceponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(SchoolPostponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(TravelApprovalPostponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);
            requestStatues.Concat(ConvictsPostponementRequestStatues).OrderByDescending(x=>x.DateOfRecive);



            return requestStatues;
        }

        [HttpGet]
        [Route("GetAUserTransactions")]
        public async Task<Dictionary<string, string>> GetAUserTransactions(int Reqid, string postponmentname)
        {
             Dictionary<string, string> result = new Dictionary<string, string>();
            switch (postponmentname)
            {
                case "AlonePostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "BrotherInServicePostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "CashAllowancLessThan42ponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "FixedServiceAllowanceponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "ObligatoryServiceponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "SchoolPostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "TravelApprovalPostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "ConvictsPostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;
                case "CashAllowancetponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:600" + Reqid));
                    break;

                default:
                    result = null;
                    break;
            }
            

            return result;
        }
    }
}
