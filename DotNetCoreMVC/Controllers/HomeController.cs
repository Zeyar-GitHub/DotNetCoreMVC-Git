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

            return RedirectToAction("Index", "Employee");
        }
    }
}
