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
        [Route("GetAllUserTransactions")]
        public List<RequestStatues> GetAllUserTransactions()
        {
            int CUserID = GetCurrentUserID();
            List<RequestStatues> result = _context.RequestStatuesDBS.Where(x => x.UserID == CUserID).OrderByDescending(x => x.DateOfRecive).Take(10).ToList<RequestStatues>();
            return result;
        }

        [HttpGet]
        [Route("GetAUserTransactions")]
        public IDictionary GetAUserTransactions(int Reqid)
        {
            IDictionary result = new Dictionary<string, object>();
            RequestStatues requestStatues = _context.RequestStatuesDBS.Find(Reqid);
            result.Add("BrotherEill", _context.BrotherEillDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault());

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequests")]
        public int GetNumberOfRequests()
        {
            int result = _context.AlonePostponementDBS.Count();

            return result;
        }

        [HttpGet]
        [Route("GetNumberOfRequestsApproved")]
        public int GetNumberOfRequestsApproved()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "wrong").Count();

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
        [Route("GetNumberOfRequestsDeleted")]
        public int GetNumberOfRequestsDeleted()
        {
            int result = _context.RequestStatuesDBS.Where(x => x.Statues == "deleted").Count();

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
