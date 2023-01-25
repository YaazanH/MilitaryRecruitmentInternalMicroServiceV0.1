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
        private async Task<string> APICall(string GURI, string Type)
        {
            try
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
                            string jsonString;
                            switch (Type)
                            {
                                case "List":
                                    List<RequestStatues> list = new List<RequestStatues>();
                                    jsonString = JsonConvert.SerializeObject(list);
                                    return jsonString;


                                case "Dic":
                                    Dictionary<string, string> result = new Dictionary<string, string>();
                                    jsonString = JsonConvert.SerializeObject(result);
                                    return jsonString;

                                default:
                                    return null;
                            }
                        }
                    }
                }
            }
            catch (Exception)
            {
                string jsonString;
                switch (Type)
                {
                    case "List":
                        List<RequestStatues> list = new List<RequestStatues>();
                        jsonString = JsonConvert.SerializeObject(list);
                        return jsonString;


                    case "Dic":
                        Dictionary<string, string> result = new Dictionary<string, string>();
                         jsonString = JsonConvert.SerializeObject(result);
                        return jsonString;

                    default:
                        return null;
                }
            }
            
        }
        [HttpGet]
        [Route("GetAllUserTransactions")]
        public async Task<string> GetAllUserTransactions()
        {
            List<RequestStatues> requestStatues = new List<RequestStatues>();
            List<RequestStatues> AlonePostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60009/AlonePostponement/GetAllUserTransactions", "List"));
            List<RequestStatues> BrotherInServicePostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60001/BrotherInServicePostponement/GetAllUserTransactions", "List"));
            List<RequestStatues> CashAllowancetponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60002/CashAllowance/GetAllUserTransactions", "List"));
            List<RequestStatues> CashAllowancLessThan42ponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60003/CashAllowanceLessThan42/GetAllUserTransactions", "List"));
            List<RequestStatues> FixedServiceAllowanceponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60004/FixedServiceAllowance/GetAllUserTransactions", "List"));
            List<RequestStatues> ObligatoryServiceponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60005/ObligatoryService/GetAllUserTransactions", "List"));
            List<RequestStatues> SchoolPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60007/SchoolPostponement/GetAllUserTransactions", "List"));
            List<RequestStatues> TravelApprovalPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60008/TravelApproval/GetAllUserTransactions", "List"));
            List<RequestStatues> ConvictsPostponementRequestStatues = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall("https://host.docker.internal:60006/PostponementOfConvicts/GetAllUserTransactions", "List"));

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
            var jsonString = JsonConvert.SerializeObject(result);

            return jsonString;
        }

        [HttpPost]
        [Route("GetAUserTransactions")]
        public async Task<string> GetAUserTransactions([FromBody] PostponmentInfo postponmentInfo)
        {
             Dictionary<string, string> result = new Dictionary<string, string>();
            switch (postponmentInfo.PosponmentName)
            {
                case "AlonePostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60009/AlonePostponement/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "BrotherInServicePostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60001/BrotherInServicePostponement/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "CashAllowancLessThan42":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60003/CashAllowanceLessThan42/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "FixedServiceAllowance":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60004/FixedServiceAllowance/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "ObligatoryService":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60005/ObligatoryService/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "SchoolPostponement":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60007/SchoolPostponement/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "TravelApproval":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60008/TravelApproval/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "PostponementOfConvicts":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60006/PostponementOfConvicts/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;
                case "CashAllowance":
                    result = JsonConvert.DeserializeObject<Dictionary<string, string>>(await APICall("https://host.docker.internal:60002/CashAllowance/GetAUserTransactions?Reqid=" + postponmentInfo.PosponmentID.ToString(), "Dic"));
                    break;

                default:
                    result = null;
                    break;
            }
            var jsonString = JsonConvert.SerializeObject(result);

            return jsonString;
        }
    }
}
