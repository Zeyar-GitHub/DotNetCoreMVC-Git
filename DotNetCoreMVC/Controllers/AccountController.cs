using Microsoft.AspNetCore.Mvc;

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
            if (username == "admin" && password == "password") // Replace with real authentication
            {
                var token = _jwtTokenService.GenerateToken(username);

                // Save JWT Token in Cookie
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true, // Set true in production
                    Expires = DateTime.UtcNow.AddMinutes(30)
                });
                return RedirectToAction("Index", "Employee");
            }
            else
            {
                ViewBag.Error = "Invalid username or password";
                return View();
            }
        }

        public IActionResult LogOut()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("LogIn");
        }
    }
}
