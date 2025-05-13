using DotNetCoreMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace DotNetCoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(DataContext dataContext, IJwtTokenService jwtTokenService)
        {
            _dataContext = dataContext;
            _jwtTokenService = jwtTokenService;
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string username, string password)
        {
            var user = _dataContext.LogIn.SingleOrDefault(u=>u.UserName == username && u.Password == password);
            
            if (user != null)
            {
                var token = _jwtTokenService.GenerateToken(username);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,  // Prevent JavaScript access
                    Secure = false,   // In Production, set to true
                    SameSite = SameSiteMode.Strict,
                    Expires = ConvertUtcToBangkokWithOffset(DateTime.UtcNow)
                };

                Response.Cookies.Append("AuthToken", token, cookieOptions);

                return RedirectToAction("Index", "Employee");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }


        }

        public static DateTime ConvertUtcToBangkokWithOffset(DateTime utcDateTime)
        {
            // Bangkok time zone id
            string bangkokTimeZoneId = "SE Asia Standard Time";

            // Timezone info
            TimeZoneInfo bangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById(bangkokTimeZoneId);

            // Bangkok date to utc date
            DateTime bangkokDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, bangkokTimeZone);

            // Add 30 minutes to the bangkok date
            DateTime bangkokDateTimeWithOffset = bangkokDateTime.AddMinutes(30);

            return bangkokDateTimeWithOffset;
        }
        public IActionResult LogOut()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("LogIn");
        }
    }
}
