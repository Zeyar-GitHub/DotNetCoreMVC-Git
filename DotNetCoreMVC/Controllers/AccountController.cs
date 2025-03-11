using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace DotNetCoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly IJwtTokenService _jwtTokenService;

        public AccountController(IJwtTokenService jwtTokenService)
        {
            _jwtTokenService = jwtTokenService;
        }
        public IActionResult LogIn()
        {
            return View();
        }
        
        [HttpPost]
        public IActionResult LogIn(string username, string password)
        {
            if (username == "admin" && password == "password")
            {
                var token = _jwtTokenService.GenerateToken(username);

                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,  // Prevent JavaScript access
                    Secure = false,   // Localhost မှာ Secure=false (Production မှာ true ပြန်ထားပါ)
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
            // Bangkok time zone ရဲ့ ID ကို သတ်မှတ်ခြင်း
            string bangkokTimeZoneId = "SE Asia Standard Time";

            // TimeZoneInfo object ကို ရယူခြင်း
            TimeZoneInfo bangkokTimeZone = TimeZoneInfo.FindSystemTimeZoneById(bangkokTimeZoneId);

            // UTC time ကို Bangkok time ပြောင်းခြင်း
            DateTime bangkokDateTime = TimeZoneInfo.ConvertTimeFromUtc(utcDateTime, bangkokTimeZone);

            // မိနစ် ၃၀ ထပ်ပေါင်းခြင်း
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
