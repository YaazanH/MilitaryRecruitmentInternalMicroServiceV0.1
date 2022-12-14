using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Threading.Tasks;


namespace AdminDashboard.Controllers
{
    [ApiController]
    [Route("AdminDashboard")]
    [Authorize(Roles ="admin")]


    public class AdminController : Controller
    {

        public async Task<string> APICall(string GURI)
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

        public async Task<string> APICall2(string GURI,string Token)
        {

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

        public async Task<IActionResult> RequestPostponmentforUser (int RequestUserId)
        {
            string UserToken= JsonConvert.DeserializeObject<string>(await APICall("https://host.docker.internal:60050/LoginAPI/AdminToken" + RequestUserId));
            string RequestStatus = JsonConvert.DeserializeObject<string>(await APICall2("https://192.168.168.103:60053/UserRequestHandler/GetUserRequestHandler", UserToken));
            return Ok(RequestStatus);
        }

        public async Task<int>  AloneAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/AlonePostponement/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> AloneApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/AlonePostponement/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> AlonePreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/AlonePostponement/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> AloneDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/AlonePostponement/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.AlonePostponement> GetAloneUserPostponment()
        {
            Models.AlonePostponement Pos = JsonConvert.DeserializeObject<Models.AlonePostponement>(await APICall("https://host.docker.internal:40018/AlonePostponement/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.AlonePostponement>> GetAllAloneUserPostponment()
        {
            List<Models.AlonePostponement> Pos = JsonConvert.DeserializeObject<List<Models.AlonePostponement>>(await APICall("https://host.docker.internal:40018/AlonePostponement/SearchAllPostponment"));
            return Pos;
        }

        public async Task<int> BrotherInServiceAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/BrotherInServicePostponement/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> BrotherInServiceApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/BrotherInServicePostponement/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> BrotherInServicePreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/BrotherInServicePostponement/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> BrotherInServiceDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/BrotherInServicePostponement/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.BrotherInServicePostponement> GetBrotherInServicePostponment()
        {
            Models.BrotherInServicePostponement Pos = JsonConvert.DeserializeObject<Models.BrotherInServicePostponement>(await APICall("https://host.docker.internal:40018/BrotherInServicePostponement/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.BrotherInServicePostponement>> GetAllBrotherInServicePostponment()
        {
            List<Models.BrotherInServicePostponement> Pos = JsonConvert.DeserializeObject<List<Models.BrotherInServicePostponement>>(await APICall("https://host.docker.internal:40018/BrotherInServicePostponement/SearchAllPostponment"));
            return Pos;
        }

        public async Task<int> CashLessAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowanceLessThan42/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> CashLessApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowanceLessThan42/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> CashLessPreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowanceLessThan42/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> CashLessDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowanceLessThan42/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.CashAllowancLessThan42Model> GetCashLessPostponment()
        {
            Models.CashAllowancLessThan42Model Pos = JsonConvert.DeserializeObject<Models.CashAllowancLessThan42Model>(await APICall("https://host.docker.internal:40018/CashAllowanceLessThan42/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.CashAllowancLessThan42Model>> GetAllCashLessPostponment()
        {
            List<Models.CashAllowancLessThan42Model> Pos = JsonConvert.DeserializeObject<List<Models.CashAllowancLessThan42Model>>(await APICall("https://host.docker.internal:40018/CashAllowanceLessThan42/SearchAllPostponment"));
            return Pos;
        }



        public async Task<int> FixedServiceAllowanceAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/FixedServiceAllowance/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> FixedServiceAllowanceApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/FixedServiceAllowance/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> FixedServiceAllowancePreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/FixedServiceAllowance/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> FixedServiceAllowanceDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/FixedServiceAllowance/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.FixedServiceAllowance> GetFixedServiceAllowancePostponment()
        {
            Models.FixedServiceAllowance Pos = JsonConvert.DeserializeObject<Models.FixedServiceAllowance>(await APICall("https://host.docker.internal:40018/FixedServiceAllowance/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.FixedServiceAllowance>> GetAllFixedServiceAllowancePostponment()
        {
            List<Models.FixedServiceAllowance> Pos = JsonConvert.DeserializeObject<List<Models.FixedServiceAllowance>>(await APICall("https://host.docker.internal:40018/FixedServiceAllowance/SearchAllPostponment"));
            return Pos;
        }
        public async Task<int> ObligatoryServiceAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/ObligatoryService/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> ObligatoryServiceApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/ObligatoryService/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> ObligatoryServicePreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/ObligatoryService/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> ObligatoryServiceDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/ObligatoryService/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.ObligatoryService> GetObligatoryServicePostponment()
        {
            Models.ObligatoryService Pos = JsonConvert.DeserializeObject<Models.ObligatoryService>(await APICall("https://host.docker.internal:40018/ObligatoryService/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.ObligatoryService>> GetAllObligatoryServicePostponment()
        {
            List<Models.ObligatoryService> Pos = JsonConvert.DeserializeObject<List<Models.ObligatoryService>>(await APICall("https://host.docker.internal:40018/ObligatoryService/SearchAllPostponment"));
            return Pos;
        }

        public async Task<int> PostponementOfConvictsAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/PostponementOfConvicts/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> PostponementOfConvictsApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/PostponementOfConvicts/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> PostponementOfConvictsPreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/PostponementOfConvicts/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> PostponementOfConvictsDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/PostponementOfConvicts/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.PostponementOfConvicts> GetPostponementOfConvictsPostponment()
        {
            Models.PostponementOfConvicts Pos = JsonConvert.DeserializeObject<Models.PostponementOfConvicts>(await APICall("https://host.docker.internal:40018/PostponementOfConvicts/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.PostponementOfConvicts>> GetAllPostponementOfConvictsPostponment()
        {
            List<Models.PostponementOfConvicts> Pos = JsonConvert.DeserializeObject<List<Models.PostponementOfConvicts>>(await APICall("https://host.docker.internal:40018/PostponementOfConvicts/SearchAllPostponment"));
            return Pos;
        }



        public async Task<int> SchoolPostponementAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/SchoolPostponement/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> SchoolPostponementApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/SchoolPostponement/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> SchoolPostponementPreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/SchoolPostponement/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> SchoolPostponementDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/SchoolPostponement/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.SchoolPostponement> GetSchoolPostponementPostponment()
        {
            Models.SchoolPostponement Pos = JsonConvert.DeserializeObject<Models.SchoolPostponement>(await APICall("https://host.docker.internal:40018/SchoolPostponement/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.SchoolPostponement>> GetAllSchoolPostponementPostponment()
        {
            List<Models.SchoolPostponement> Pos = JsonConvert.DeserializeObject<List<Models.SchoolPostponement>>(await APICall("https://host.docker.internal:40018/SchoolPostponement/SearchAllPostponment"));
            return Pos;
        }



        public async Task<int> TravelApprovalAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/TravelApproval/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> TravelApprovalApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/TravelApproval/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> TravelApprovalPreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/TravelApproval/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> TravelApprovalDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/TravelApproval/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.TravelApproval> GetTravelApprovalPostponment()
        {
            Models.TravelApproval Pos = JsonConvert.DeserializeObject<Models.TravelApproval>(await APICall("https://host.docker.internal:40018/TravelApproval/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.TravelApproval>> GetAllTravelApprovalPostponment()
        {
            List<Models.TravelApproval> Pos = JsonConvert.DeserializeObject<List<Models.TravelApproval>>(await APICall("https://host.docker.internal:40018/TravelApproval/SearchAllPostponment"));
            return Pos;
        }



        public async Task<int> CashAllowancelAll()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowance/GetNumberOfRequests"));
            return num;
        }

        public async Task<int> CashAllowanceApproved()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowance/GetNumberOfRequestsApproved"));
            return num;
        }

        public async Task<int> CashAllowancePreocessing()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowance/GetNumberOfRequestsProcessing"));
            return num;
        }

        public async Task<int> CashAllowanceDeleted()
        {
            int num = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40018/CashAllowance/GetNumberOfRequestsDeleted"));
            return num;
        }

        public async Task<Models.CashAllowance> GetCashAllowancePostponment()
        {
            Models.CashAllowance Pos = JsonConvert.DeserializeObject<Models.CashAllowance>(await APICall("https://host.docker.internal:40018/CashAllowance/SearchUserPostponment"));
            return Pos;
        }

        public async Task<List<Models.CashAllowance>> GetAllCashAllowancePostponment()
        {
            List<Models.CashAllowance> Pos = JsonConvert.DeserializeObject<List<Models.CashAllowance>>(await APICall("https://host.docker.internal:40018/CashAllowance/SearchAllPostponment"));
            return Pos;
        }
    }
      
}
