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
using CashAllowancLessThan42.Data;
using CashAllowancLessThan42.Models;
using System.Collections.Generic;

namespace CashAllowancLessThan42.Controllers
{

    [ApiController]
    [Route("CashAllowanceLessThan42")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class CashAllowancLessThan42Cotroller : Controller
    {
        private readonly CashAllowancLessThan42Context _context;
        public CashAllowancLessThan42Cotroller(CashAllowancLessThan42Context context)
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
        public RequestStatues GetAUserTransactions(int Reqid)
        {
            RequestStatues result = _context.RequestStatuesDBS.Where(x => x.ReqStatuesID == Reqid).FirstOrDefault();
            return result;
        }
        [HttpGet]
        [Route("GetNumberOfRequests")]
        public int GetNumberOfRequests()
        {
            int result = _context.CashAllowancLessThan42Db.Count();

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
        public Models.CashAllowancLessThan42Model GetUserPostponment(int id)
        {

            Models.CashAllowancLessThan42Model result = _context.CashAllowancLessThan42Db.Where(x => x.UserID == id).FirstOrDefault();


            return result;
        }

        [HttpGet]
        [Route("SearchAllPostponment")]
        public List<Models.CashAllowancLessThan42Model> GetAllUsersPostponments()
        {

            List<Models.CashAllowancLessThan42Model> result = _context.CashAllowancLessThan42Db.ToList();


            return result;
        }
    }
}