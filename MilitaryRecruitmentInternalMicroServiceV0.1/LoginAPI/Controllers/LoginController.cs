using LoginAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using LoginAPI.Data;

namespace LoginAPI.Controller
{
    [ApiController]
    [Route("Login")]
    public class LoginController : ControllerBase
    {
        private IConfiguration _config;
        private readonly LoginContext _context;

        public LoginController(IConfiguration config, LoginContext context)
        {
            _config = config;
            _context = context;
        }
        [AllowAnonymous]
        [HttpGet]
        public IActionResult LogIn([FromBody] Login userLogin)
        {
            var User = Authenticate(userLogin);
            if (User != null)
            {
                var token = Generate(User);
                return Ok(token);


            }
            return NotFound("User not found");
        }
        [AllowAnonymous]
        [HttpPost]
        public IActionResult SignUp([FromBody] Login userLogin)
        {
            var User = _context.LoginDBS.FirstOrDefault(o=>o.Username==userLogin.Username||o.UserID==userLogin.UserID);
            if (User == null)
            {
                _context.LoginDBS.Add(userLogin);
                _context.SaveChanges();
                var token = Generate(userLogin);
                return Ok(token);

            }
            return NotFound("UserName already exsist");
        }

        private string Generate(Login user)
        {
            var securityKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_config["jwt:key"]));
            var credentials = new SigningCredentials(securityKey, SecurityAlgorithms.HmacSha256);
            var claims = new[]
            {
                new Claim(ClaimTypes.PrimarySid,user.UserID.ToString()),

            };
            var token = new JwtSecurityToken(_config["jwt:Issuer"],
                _config["jwt:Audience"],
                claims,
                expires: DateTime.Now.AddDays(1),
                signingCredentials: credentials);
            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private Login Authenticate(Login userLogin)
        {
            //mistake in writing LoginDBS 
            var currentUser = _context.LoginDBS.FirstOrDefault(o => o.Username.ToLower() == userLogin.Username.ToLower() && o.Password == userLogin.Password);

            if (currentUser != null)
            {
                return currentUser;
            }
            return null;
        }

        [Authorize(Roles = "Admin")]
        [HttpGet]
        public string GetUserToken(int UserID)
        {
            Login CurrentUser = new Login();
            CurrentUser.UserID = UserID;
            return Generate(CurrentUser);
        }
    }
}
