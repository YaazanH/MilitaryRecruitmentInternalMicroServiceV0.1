using AdminDashboard.Models;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;


namespace AdminDashboard.Controllers
{
    
    [ApiController]
    [Route("AdminDashboard")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    [Authorize(Roles = "Admin")]


    public class AdminController : Controller
    {

        public async Task<string> APICall(string GURI,string Type)
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
                try
                {
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
                                case "GetAllPosponment":

                                    jsonString = JsonConvert.SerializeObject(0);
                                    return jsonString;


                                case "GetAllOfPosponment":
                                    jsonString = JsonConvert.SerializeObject(0);
                                    return jsonString;

                                case "Token":
                                    jsonString = JsonConvert.SerializeObject("0");
                                    return jsonString;

                                case "GetAllTransactionsURL":
                                    List<RequestStatues> list = new List<RequestStatues>();
                                    jsonString = JsonConvert.SerializeObject(list);
                                    return jsonString;

                                default:
                                    return null;
                            }
                        }
                    }
                }
                catch (Exception)
                {

                    string jsonString;
                    switch (Type)
                    {
                        case "GetAllPosponment":
                            
                            jsonString = JsonConvert.SerializeObject(0);
                            return jsonString;


                        case "GetAllOfPosponment":
                            jsonString = JsonConvert.SerializeObject(0);
                            return jsonString;

                        case "Token":
                            jsonString = JsonConvert.SerializeObject("0");
                            return jsonString;

                        case "GetAllTransactionsURL":
                            List<RequestStatues> list = new List<RequestStatues>();
                            jsonString = JsonConvert.SerializeObject(list);
                            return jsonString;

                        default:
                            return null;
                    }
                }
                
            }
        }

        public async Task<string> APICall2(string GURI, string Token,HttpContent Data)
        {

            Uri uri = new Uri(GURI);

            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };

            using (var Client = new HttpClient(clientHandler))
            {
                Client.BaseAddress = uri;
                Client.DefaultRequestHeaders.Authorization = new AuthenticationHeaderValue("Bearer", Token);
                try
                {
                    using (HttpResponseMessage response = await Client.PostAsync(Client.BaseAddress, Data))
                    {
                        if (response.IsSuccessStatusCode)
                        {
                            return response.Content.ReadAsStringAsync().Result;
                        }
                        else
                        {
                            return "Faild";
                        }
                    }
                }
                catch (Exception)
                {
                    return "Faild";
                }
                
            }
        }

        [HttpPost]
        [Route("PostRequestPostponmentforUser")]
        public async Task<IActionResult> RequestPostponmentforUser([FromBody] JObject dataObject)
        {
            var UserToken = await APICall("https://host.docker.internal:60050/LoginAPI/AdminToken?id=" + dataObject["userID"].ToString(), "Token");
            if (UserToken!="0")
            {              
                var PID= dataObject["PostponementID"].ToString();    
                var json = JsonConvert.SerializeObject(new { PostponementID = dataObject["PostponementID"].ToString() });
                HttpContent data = new StringContent(json, Encoding.UTF8, "application/json");
                var RequestStatus = await APICall2("https://host.docker.internal:60053/UserRequestHandler/GetUserRequestHandler", UserToken,data);
                return Ok(RequestStatus);

            }
            return Ok("faild");
            
        }

        [HttpGet]
        [Route("GetAllPosponment")]
        public async Task<string> GetAllPosponment()
        {
            Dictionary<string, string> AllPosponment = new Dictionary<string, string>();
            int AloneNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60009/AlonePostponement/GetNumberOfRequests", "GetAllPosponment"));
            int BrotherInServiceNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60001/BrotherInServicePostponement/GetNumberOfRequests", "GetAllPosponment"));
            int CashAllowanceLessThan42Num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60003/CashAllowanceLessThan42/GetNumberOfRequests", "GetAllPosponment"));
            int FixedServiceAllowanceNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60004/FixedServiceAllowance/GetNumberOfRequests", "GetAllPosponment"));
            int ObligatoryServiceNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60005/ObligatoryService/GetNumberOfRequests", "GetAllPosponment"));
            int PostponementOfConvictsNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60006/PostponementOfConvicts/GetNumberOfRequests", "GetAllPosponment"));
            int SchoolPostponementNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60007/SchoolPostponement/GetNumberOfRequests", "GetAllPosponment"));
            int TravelApprovalNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60008/TravelApproval/GetNumberOfRequests", "GetAllPosponment"));
            int CashAllowanceNum = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:60002/CashAllowance/GetNumberOfRequests", "GetAllPosponment"));

            AllPosponment.Add("1", AloneNum.ToString());
            AllPosponment.Add("2", BrotherInServiceNum.ToString());
            AllPosponment.Add("4", CashAllowanceLessThan42Num.ToString());
            AllPosponment.Add("5", FixedServiceAllowanceNum.ToString());
            AllPosponment.Add("6", ObligatoryServiceNum.ToString());
            AllPosponment.Add("7", PostponementOfConvictsNum.ToString());
            AllPosponment.Add("8", SchoolPostponementNum.ToString());
            AllPosponment.Add("9", TravelApprovalNum.ToString());
            AllPosponment.Add("3", CashAllowanceNum.ToString());


            var jsonString = JsonConvert.SerializeObject(AllPosponment);

            return jsonString;
        }


        [HttpPost]
        [Route("GetAllOfPosponment")]
        public async Task<string> GetAllOfPosponment([FromBody] JObject dataObject)
        {
            Dictionary<string, string> GetAllPostDic = new Dictionary<string, string>();
            int GetApproved = 0, GetWating = 0, GetFaild = 0;
            string RequestsApprovedURL = "";
            string RequestsProcessingURL = "";
            string RequestsFaildgURL = "";
            switch (Int32.Parse(dataObject["PostponementID"].ToString()))
            {
                case 1:
                     RequestsApprovedURL="https://host.docker.internal:60009/AlonePostponement/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60009/AlonePostponement/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60009/AlonePostponement/GetNumberOfRequestsFaild";


                    break;

                case 2:
                     RequestsApprovedURL="https://host.docker.internal:60001/BrotherInServicePostponement/GetNumberOfRequestsApproved";
                    RequestsProcessingURL = "https://host.docker.internal:60001/BrotherInServicePostponement/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60001/BrotherInServicePostponement/GetNumberOfRequestsFaild";


                    break;

                case 3:
                    RequestsApprovedURL = "https://host.docker.internal:60002/CashAllowance/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60002/CashAllowance/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60002/CashAllowance/GetNumberOfRequestsFaild";


                    break;

                case 4:
                    RequestsApprovedURL = "https://host.docker.internal:60003/CashAllowanceLessThan42/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60003/CashAllowanceLessThan42/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60003/CashAllowanceLessThan42/GetNumberOfRequestsFaild";


                    break;

                case 5:
                     RequestsApprovedURL = "https://host.docker.internal:60004/FixedServiceAllowance/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60004/FixedServiceAllowance/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60004/FixedServiceAllowance/GetNumberOfRequestsFaild";

                    break;

                case 6:
                    RequestsApprovedURL = "https://host.docker.internal:60005/ObligatoryService/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60005/ObligatoryService/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60005/ObligatoryService/GetNumberOfRequestsFaild";

                    break;

                case 7:
                    RequestsApprovedURL = "https://host.docker.internal:60006/PostponementOfConvicts/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60006/PostponementOfConvicts/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60006/PostponementOfConvicts/GetNumberOfRequestsFaild";

                    break;

                case 8:
                    RequestsApprovedURL = "https://host.docker.internal:60007/SchoolPostponement/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60007/SchoolPostponement/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60007/SchoolPostponement/GetNumberOfRequestsFaild";

                    break;

                case 9:
                    RequestsApprovedURL = "https://host.docker.internal:60008/TravelApproval/GetNumberOfRequestsApproved";
                     RequestsProcessingURL = "https://host.docker.internal:60008/TravelApproval/GetNumberOfRequestsProcessing";
                     RequestsFaildgURL = "https://host.docker.internal:60008/TravelApproval/GetNumberOfRequestsFaild";

                    break;

            }

            GetApproved = JsonConvert.DeserializeObject<int>(await APICall(RequestsApprovedURL, "GetAllOfPosponment"));
            GetWating = JsonConvert.DeserializeObject<int>(await APICall(RequestsProcessingURL, "GetAllOfPosponment"));
            GetFaild = JsonConvert.DeserializeObject<int>(await APICall(RequestsFaildgURL, "GetAllOfPosponment"));

            GetAllPostDic.Add("proccessingPostponments", GetWating.ToString());
            GetAllPostDic.Add("completedPostponments", GetApproved.ToString());
            GetAllPostDic.Add("failedPostponments", GetFaild.ToString());

            var jsonString = JsonConvert.SerializeObject(GetAllPostDic);

            return jsonString;
        }



        [HttpPost]
        [Route("GetAllTransactions")]
        public async Task<string> GetAllTransactions([FromBody] JObject dataObject)
        {
            string GetAllTransactionsURL = "";

            switch (Int32.Parse(dataObject["PostponementID"].ToString()))
            {
                case 1:
                    GetAllTransactionsURL = "https://host.docker.internal:60009/AlonePostponement/GetAllTransactions";
                   

                    break;

                case 2:
                    GetAllTransactionsURL = "https://host.docker.internal:60001/BrotherInServicePostponement/GetAllTransactions";
                   
                    break;

                case 3:
                    GetAllTransactionsURL = "https://host.docker.internal:60002/CashAllowance/GetAllTransactions";
                    

                    break;

                case 4:
                    GetAllTransactionsURL = "https://host.docker.internal:60003/CashAllowanceLessThan42/GetAllTransactions";
                    

                    break;

                case 5:
                    GetAllTransactionsURL = "https://host.docker.internal:60004/FixedServiceAllowance/GetAllTransactions";
                    
                    break;

                case 6:
                    GetAllTransactionsURL = "https://host.docker.internal:60005/ObligatoryService/GetAllTransactions";
                    
                    break;

                case 7:
                    GetAllTransactionsURL = "https://host.docker.internal:60006/PostponementOfConvicts/GetAllTransactions";
                     
                    break;

                case 8:
                    GetAllTransactionsURL = "https://host.docker.internal:60007/SchoolPostponement/GetAllTransactions";
                    
                    break;

                case 9:
                    GetAllTransactionsURL = "https://host.docker.internal:60008/TravelApproval/GetAllTransactions";
                    
                    break;

            }

            List<RequestStatues> result = JsonConvert.DeserializeObject<List<RequestStatues>>(await APICall(GetAllTransactionsURL, "GetAllTransactions"));
            
           
            var jsonString = JsonConvert.SerializeObject(result);

            return jsonString;
        }

        
    }

}