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
            List<RequestStatues> AlonePostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60009/AlonePostponement/GetAllUserTransactions"));
            List<RequestStatues> BrotherInServicePostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60001/BrotherInServicePostponement/GetAllUserTransactions"));
            List<RequestStatues> CashAllowancetponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60002/CashAllowance/GetAllUserTransactions"));
            List<RequestStatues> CashAllowancLessThan42ponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60003/CashAllowanceLessThan42/GetAllUserTransactions"));
            List<RequestStatues> FixedServiceAllowanceponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60004/FixedServiceAllowance/GetAllUserTransactions"));
            List<RequestStatues> ObligatoryServiceponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60005/ObligatoryService/GetAllUserTransactions"));
            List<RequestStatues> SchoolPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60007/SchoolPostponement/GetAllUserTransactions"));
            List<RequestStatues> TravelApprovalPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60008/TravelApproval/GetAllUserTransactions"));
            List<RequestStatues> ConvictsPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60006/PostponementOfConvicts/GetAllUserTransactions"));

            requestStatues.AddRange(AlonePostponementRequestStatues);
            requestStatues.AddRange(BrotherInServicePostponementRequestStatues);
            requestStatues.AddRange(CashAllowancetponementRequestStatues);
            requestStatues.AddRange(CashAllowancLessThan42ponementRequestStatues);
            requestStatues.AddRange(FixedServiceAllowanceponementRequestStatues);
            requestStatues.AddRange(ObligatoryServiceponementRequestStatues);
            requestStatues.AddRange(SchoolPostponementRequestStatues);
            requestStatues.AddRange(TravelApprovalPostponementRequestStatues);
            requestStatues.AddRange(ConvictsPostponementRequestStatues);

            List<RequestStatues>result = requestStatues.OrderByDescending(x => x.DateOfRecive).ToList<RequestStatues>();
            return result;
        }

        [HttpPost]
        [Route("GetAUserTransactions")]
        public async Task<Dictionary<string, string>> GetAUserTransactions([FromBody] PostponmentInfo postponmentInfo)
        {
             Dictionary<string, string> result = new Dictionary<string, string>();
            switch (postponmentInfo.PosponmentName)
            {
                case "AlonePostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60009/AlonePostponement/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "BrotherInServicePostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60001/BrotherInServicePostponement/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "CashAllowancLessThan42":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60003/CashAllowanceLessThan42/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "FixedServiceAllowance":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60004/FixedServiceAllowance/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "ObligatoryService":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60005/ObligatoryService/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "SchoolPostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60007/SchoolPostponement/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "TravelApproval":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60008/TravelApproval/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "PostponementOfConvicts":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60006/PostponementOfConvicts/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;
                case "CashAllowance":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60002/CashAllowance/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString()));
                    break;

                default:
                    result = null;
                    break;
            }
            

            return result;
        }
    }
}
