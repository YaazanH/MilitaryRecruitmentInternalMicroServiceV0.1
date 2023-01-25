using Microsoft.AspNetCore.Mvc;
using System;
using System.Net.Http;
using SchoolPostponementAPI.Models;
using Newtonsoft.Json;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using System.Text;
using System.Threading.Tasks;
using System.Net.Http.Headers;
using Microsoft.Net.Http.Headers;
using SchoolPostponementAPI.Data;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;

namespace SchoolPostponementAPI.Controllers
{
    [ApiController]
    [Route("SchoolPostponement")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class SchoolPostponementController : Controller
    {

        private readonly SchoolPostponementContext _context;
        public SchoolPostponementController(SchoolPostponementContext context)
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
            SchoolPostponement st = new SchoolPostponement { UserID = CUserID, DateOfGiven = DateTime.Now, DateOfEnd = DateTime.Now.AddYears(1) };
            _context.schoolDBS.Add(st);
            _context.SaveChanges();

        }
       

        [HttpGet]
        [Route("GetNumberOfRequests")]
        public int GetNumberOfRequests()
        {
            int result = _context.schoolDBS.Count();
            result += _context.RequestStatuesDBS.Where(x => x.Statues == "wating" || x.Statues == "Faild").Count();

            return result;
        }

        [HttpGet]
        [Route("GetAllTransactions/")]
        public List<RequestStatues> GetAllTransactions()
        {
            List<RequestStatues> result = _context.RequestStatuesDBS.OrderByDescending(x => x.DateOfRecive).ToList<RequestStatues>();
            return result;
        }

        [HttpGet]
        [Route("GetAllUserTransactions/")]
        public List<RequestStatues> GetAllUserTransactions()
        {
            int CUserID = GetCurrentUserID();
            List<RequestStatues> result = _context.RequestStatuesDBS.Where(x => x.UserID == CUserID).OrderByDescending(x => x.DateOfRecive).Take(10).ToList<RequestStatues>();
            return result;
        }



        [HttpGet]
        [Route("GetNumberOfRequestsApproved")]
        public int GetNumberOfRequestsApproved()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Done").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsProcessing")]
        public int GetNumberOfRequestsProcessing()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "wating").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsFaild")]
        public int GetNumberOfRequestsFaild()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Faild").Count();

            return result;
        }

        [HttpGet]
        [Route("GetAUserTransactions")]
        public Dictionary<string, string> GetAUserTransactions(int Reqid)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            RequestStatues requestStatues = _context.RequestStatuesDBS.Where(x => x.ReqStatuesID == Reqid).FirstOrDefault();
            if (requestStatues != null)
            {


                AsyncDroppedOut asyncDroppedOut = _context.AsyncDroppedOutDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                AsyncStudyingNow asyncStudyingNow = _context.AsyncStudyingNowDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                AsyncStudyYears asyncStudyYears = _context.AsyncStudyYearsDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                AsyncAge asyncAge = _context.AsyncAgeDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

                

                result.Add("asyncDroppedOut", asyncDroppedOut.statuse);
                result.Add("asyncStudyingNow", asyncStudyingNow.statuse);
                result.Add("asyncStudyYears", asyncStudyYears.statuse);
                result.Add("asyncAge", asyncAge.statuse);

                return result;
            }
            else
            {
                return result;
            }
        }


        /*
        [HttpGet]
        [Route("GetIsAStudent/")]
        public async Task<IActionResult> GetIsAStudent()
        {
            int CUserID = GetCurrentUserID();
            var User = _context.schoolDBS.Where(x => x.UserID == CUserID).FirstOrDefault();
            if (User != null)
            {
                if (User.DateOfEnd.DateTime > DateTime.Now)
                {
                    return Ok("You aready have vaild cert");
                }
            }

            int Age = JsonConvert.DeserializeObject<int>(await APICall("https://host.docker.internal:40006/RecordAdminstration/GetAge"));

            if (Age > 37)
            {
                return Ok("You cant cert because of age");
            }

            if (!JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40006/University/GetIsStudyingNow")))
            {
                return Ok("You cant cert because of Cut in UNI");
            }

            if (!JsonConvert.DeserializeObject<bool>(await APICall("https://host.docker.internal:40006/EduMinAPI/GetIsDroppedOut")))
            {
                return Ok("You cant cert because of Cut in School");
            }

            switch (JsonConvert.DeserializeObject<string>(await APICall("https://host.docker.internal:40006/University/GetStudyYears")))
            {
                case "2":
                    if (Age < 24)
                    {
                        AddCert(CUserID);
                        return Ok("You cert has been added ssuc");
                    }
                    break;
                case "3":
                    if (Age < 25)
                    {
                        AddCert(CUserID);
                        return Ok("You cert has been added ssuc");
                    }
                    break;
                case "4":
                    if (Age < 26)
                    {
                        AddCert(CUserID);
                        return Ok("You cert has been added ssuc");
                    }
                    break;
                case "5":
                    if (Age < 27)
                    {
                        AddCert(CUserID);
                        return Ok("You cert has been added ssuc");
                    }
                    break;
                case "6":
                    if (Age < 29)
                    {
                        AddCert(CUserID);
                        return Ok("You cert has been added ssuc");
                    }
                    break;
                default:
                    return NotFound();
            }
            return NoContent();
        }*/
    }
}
