using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Net;
using FinanceAPI.Controllers;
using FinanceAPI.Data;

namespace FinanceAPI.Controllers
{
    [ApiController]
    [Route("PoliticalParties")]
    [Authorize(AuthenticationSchemes = JwtBearerDefaults.AuthenticationScheme)]
    public class FinanceController : Controller
    {
        private readonly FinanceContext _context;
        public FinanceController(FinanceContext context)
        {
            _context = context;
        }

       

    }
}
