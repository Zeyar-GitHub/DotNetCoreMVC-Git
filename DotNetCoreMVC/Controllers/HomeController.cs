using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using DotNetCoreMVC.Data;

namespace DotNetCoreMVC.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataContext _dataContext;

        public HomeController(DataContext dataContext)
        {
            _dataContext = dataContext;
        }

        public IActionResult Index()
        {
            var token = Request.Cookies["AuthToken"];

            if (string.IsNullOrEmpty(token))
            {
                return RedirectToAction("LogIn", "Account");
            }

            try
            {
                var handler = new JwtSecurityTokenHandler();
                var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

                if (jsonToken != null)
                {
                    // Get username from ClaimTypes.Name
                    var username = jsonToken.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Name)?.Value;

                    if (!string.IsNullOrEmpty(username))
                    {
                        ViewBag.Username = username;
                    }
                    else
                    {
                        ViewBag.Username = "Guest";
                    }
                }
                else
                {
                    ViewBag.Username = "Guest";
                }
            }
            catch
            {
                ViewBag.Username = "Guest";
            }
            
            // Get employee list
            var employees = _dataContext.Employees.ToList();
            return View(employees);
        }
    }
}
