using DotNetCoreMVC.Data;
using Microsoft.AspNetCore.Mvc;
using DotNetCoreMVC.Models;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;

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
        public IActionResult LogIn(string UserName, string Password)
        {
            try
            {
                _logger.LogInformation($"Login attempt for username: {UserName}");

                if (string.IsNullOrEmpty(UserName) || string.IsNullOrEmpty(Password))
                {
                    _logger.LogWarning("Username or password is empty");
                    ViewBag.Error = "Please enter both username and password";
                    return View();
                }

                var user = _dataContext.LogIn.FirstOrDefault(x => x.UserName == UserName);
                _logger.LogInformation($"User found: {user != null}");

                if (user == null)
                {
                    _logger.LogWarning($"No user found with username: {UserName}");
                    ViewBag.Error = "Invalid username";
                    return View();
                }

                var role = _dataContext.Roles.FirstOrDefault(r => r.RoleId == user.RoleId);
                _logger.LogInformation($"Role found: {role?.RoleName}");

                if (role == null)
                {
                    _logger.LogWarning($"No role found for user: {user.UserName}");
                    ViewBag.Error = "User role not found";
                    return View();
                }

                bool passwordValid = BCrypt.Net.BCrypt.Verify(Password, user.Password);
                _logger.LogInformation($"Password verification result: {passwordValid}");

                if (!passwordValid)
                {
                    _logger.LogWarning($"Invalid password for user: {user.UserName}");
                    ViewBag.Error = "Invalid password";
                    return View();
                }

                var token = _jwtTokenService.GenerateToken(user.UserName, user.LogInID, role.RoleName);
                Response.Cookies.Append("AuthToken", token, new CookieOptions
                {
                    HttpOnly = true,
                    Secure = true,
                    SameSite = SameSiteMode.Strict
                });

                _logger.LogInformation($"Login successful for user: {user.UserName} with role: {role.RoleName}");

                // Redirect based on role
                if (role.RoleName == "Admin")
                {
                    return RedirectToAction("Index", "Admin");
                }
                return RedirectToAction("Index", "Home");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error during login");
                ViewBag.Error = "An error occurred during login. Please try again.";
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
