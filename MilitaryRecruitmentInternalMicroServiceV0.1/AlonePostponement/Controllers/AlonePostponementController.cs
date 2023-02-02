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
using AlonePostponement.Data;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using AlonePostponement.Models;
using System.Collections;

namespace AlonePostponement.Controllers
{
    [ApiController]
    [Route("AlonePostponement")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class AlonePostponementController : Controller
    {
        private readonly AlonePostponementContext _context;
        public AlonePostponementController(AlonePostponementContext context)
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

        [HttpGet]
        [Route("GetAllTransactions/")]
        [Authorize(Roles = "Admin")]
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
        [Route("GetAUserTransactions/")]
        public Dictionary<string, string> GetAUserTransactions(int Reqid)
        {
            Dictionary<string, string> result = new Dictionary<string, string>();
            RequestStatues requestStatues = _context.RequestStatuesDBS.Where(x => x.ReqStatuesID == Reqid).FirstOrDefault();
            if (requestStatues != null)
            {


                BrotherEill brotherEill = _context.BrotherEillDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                BrothersID brothersID = _context.BrothersIDDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                DeadBrothers deadBrothers = _context.DeadBrothersDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();


                result.Add("asynctravel", brotherEill.Statues);
                result.Add("asynLabor", brothersID.Statues);
                result.Add("asyncAge", deadBrothers.Statues);

                return result;
            }
            else
            {
                return result;
            }
        }

 
        [HttpGet]
        [Route("GetNumberOfRequests")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequests()
        {
            int result = _context.AlonePostponementDBS.Count();
            result += _context.RequestStatuesDBS.Where(x=>x.Statues=="wating"|| x.Statues == "Faild").Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsApproved")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequestsApproved()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Done").Count();

            return result;
        }


        [HttpGet]
        [Route("GetNumberOfRequestsProcessing")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequestsProcessing()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "wating").Count();

            return result;
        }


        [HttpGet]
        [Route("GetNumberOfRequestsFaild")]
        [Authorize(Roles = "Admin")]
        public int GetNumberOfRequestsFaild()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "Faild").Count();

            return result;
        }


        [HttpGet]
        [Route("SearchUserPostponment")]
        public Models.AlonePostponement GetUserPostponment(int id)
        {
            
            Models.AlonePostponement result = _context.AlonePostponementDBS.Where(x => x.UserID == id).FirstOrDefault();
           

            return result;
        }

        [HttpGet]
        [Route("SearchAllPostponment")]
        public List<Models.AlonePostponement> GetAllUsersPostponments()
        {

            List<Models.AlonePostponement> result = _context.AlonePostponementDBS.ToList();


            return result;
        }
    }
}
