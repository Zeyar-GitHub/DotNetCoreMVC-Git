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
        [HttpPost]
        public IActionResult LogIn(string username, string password)
        {
            var token = _jwtTokenService.GenerateToken(username);

            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,  // Prevent JavaScript access
                Secure = false,   // Localhost မှာ Secure=false (Production မှာ true ပြန်ထားပါ)
                SameSite = SameSiteMode.Strict,
                Expires = DateTime.UtcNow.AddMinutes(30)
            };

            Response.Cookies.Append("AuthToken", token, cookieOptions);

            Console.WriteLine($"✅ Set-Cookie: AuthToken={token}"); // Debug Log

            return RedirectToAction("Index", "Employee");
        }


        public IActionResult LogOut()
        {
            Response.Cookies.Delete("AuthToken");
            return RedirectToAction("LogIn");
        }
    }
}
