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
        [Route("GetNumberOfRequests")]
        public int GetNumberOfRequests()
        {
            int result = _context.CashAllowancLessThan42Db.Count();
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


                Asynctravel asynctravel = _context.AsynctravelDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();                
                AsyncDaysOutsideCoun asyncDaysOutsideCoun = _context.AsyncDaysOutsideCounDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();
                AsyncAge asyncAge = _context.AsyncAgeDBS.Where(x => x.RequestStatuesID == requestStatues).FirstOrDefault();

                

                result.Add("asynctravel", asynctravel.statuse);       
                result.Add("asynLabor", asyncDaysOutsideCoun.statuse);
                result.Add("asyncAge", asyncAge.statuse);

                return result;
            }
            else
            {
                return result;
            }
        }

    }
}