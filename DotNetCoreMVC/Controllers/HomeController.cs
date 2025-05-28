using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using System.IdentityModel.Tokens.Jwt;
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
                return RedirectToAction("Login", "Account");
            }

            var handler = new JwtSecurityTokenHandler();
            var jsonToken = handler.ReadToken(token) as JwtSecurityToken;

            ViewBag.Username = jsonToken?.Claims.FirstOrDefault(c => c.Type == "unique_name")?.Value ?? "Guest";
            
            // Get employee list
            var employees = _dataContext.Employees.ToList();
            return View(employees);
        }
    }
}
