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
using System.Security.Claims;
using System.Text.Json;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using RabbitMQ.Client;
using Newtonsoft.Json.Linq;
using UserRequestHandler.Models;
using System.Linq;

namespace UserRequestHandler.Controllers
{
    [ApiController]
    [Route("UserRequestHandler")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class UserRequestHandlerController : ControllerBase
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
        private String GetCurrentUserToken()
        {
            var authorization = Request.Headers[HeaderNames.Authorization];

            AuthenticationHeaderValue.TryParse(authorization, out var authentication);

            string Token = "";


            if (AuthenticationHeaderValue.TryParse(authorization, out var headerValue))
            {
                Token = headerValue.Parameter;
                return Token;
            }
            return null;
        }


        [HttpPost]
        [Route("GetUserRequestHandler/")]
        public IActionResult GetUserRequestHandler([FromBody] JObject dataObject)
        {
            var factory = new ConnectionFactory() { HostName = "host.docker.internal" };

            using var connection = factory.CreateConnection();

            using var channel = connection.CreateModel();

            channel.ExchangeDeclare(exchange: "UserRequestExch", type: ExchangeType.Direct);

            UserInfo userInfo = new UserInfo();
            userInfo.UserID = GetCurrentUserID();
            userInfo.JWT=GetCurrentUserToken();
            var mess = System.Text.Json.JsonSerializer.Serialize(userInfo);
            var body = Encoding.UTF8.GetBytes(mess);

            string RoueKey = "";

                switch (Int32.Parse(dataObject["PostponementID"].ToString()))
                {
                case 1:
                    RoueKey = "AlonePostponement";
                    break;

                case 2:
                    RoueKey = "BrotherInServicePostponement";
                    break;

                case 3:
                    RoueKey = "CashAllowance";
                    break;

                case 4:
                    RoueKey = "CashAllowancLessThan42";
                    break;

                case 5:
                    RoueKey = "FixedServiceAllowance";
                    break;

                case 6:
                    RoueKey = "ObligatoryService";
                    break;

                case 7:
                    RoueKey = "PostponementOfConvicts";
                    break;

                case 8:
                    RoueKey = "SchoolPostponement";
                    break;

                case 9:
                    RoueKey = "TravelApproval";
                    break;

                default:
                    return NotFound();

                }
                channel.BasicPublish("UserRequestExch", RoueKey, null, body);

                return Ok("The request has been received and is now being processed");
        }
    }
}