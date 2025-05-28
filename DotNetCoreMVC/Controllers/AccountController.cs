using DotNetCoreMVC.Data;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreMVC.Models;
using BCrypt.Net;
using Microsoft.Extensions.Logging;

namespace DotNetCoreMVC.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly IJwtTokenService _jwtTokenService;
        private readonly ILogger<AccountController> _logger;

        public AccountController(DataContext dataContext, IJwtTokenService jwtTokenService, ILogger<AccountController> logger)
        {
            _dataContext = dataContext;
            _jwtTokenService = jwtTokenService;
            _logger = logger;
        }

        public IActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public IActionResult LogIn(string username, string password)
        {
            var user = _dataContext.LogIn.SingleOrDefault(u => u.UserName == username);
            
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                var token = _jwtTokenService.GenerateToken(user.UserName, user.LogInID);
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict,
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });

                return RedirectToAction("Index", "Home");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LogInCreateModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if username already exists
                if (_dataContext.LogIn.Any(u => u.UserName == model.UserName))
                {
                    ModelState.AddModelError("UserName", "Username already exists");
                    return View(model);
                }

                // Hash the password
                string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                var newUser = new LogIn
                {
                    UserName = model.UserName,
                    Password = hashedPassword
                };

                _dataContext.LogIn.Add(newUser);
                _dataContext.SaveChanges();

                return RedirectToAction("LogIn");
            }

            return View(model);
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("LogIn");
        }

        public IActionResult Delete()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(string username, string password)
        {
            var user = _dataContext.LogIn.SingleOrDefault(u => u.UserName == username);
            
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.Password))
            {
                _dataContext.LogIn.Remove(user);
                _dataContext.SaveChanges();

                // Clear the authentication cookie
                Response.Cookies.Delete("AuthToken");

                return RedirectToAction("LogIn");
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
    }
}
