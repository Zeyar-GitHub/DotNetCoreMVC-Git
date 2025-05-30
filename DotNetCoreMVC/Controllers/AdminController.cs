using DotNetCoreMVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using DotNetCoreMVC.Models;
using BCrypt.Net;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreMVC.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(DataContext dataContext, ILogger<AdminController> logger)
        {
            _dataContext = dataContext;
            _logger = logger;
        }

        public IActionResult Index()
        {
            try
            {
                // Check if Roles table has data
                if (!_dataContext.Roles.Any())
                {
                    // Add roles if they don't exist
                    _dataContext.Roles.AddRange(
                        new Role { RoleId = 1, RoleName = "Admin" },
                        new Role { RoleId = 2, RoleName = "User" }
                    );
                    _dataContext.SaveChanges();
                }

                // Check if any users exist
                if (!_dataContext.LogIn.Any())
                {
                    // Create initial admin user
                    var hashedPassword = BCrypt.Net.BCrypt.HashPassword("Admin123");
                    var adminUser = new LogIn
                    {
                        UserName = "Michael",
                        Password = hashedPassword,
                        RoleId = 1 // Admin role
                    };
                    _dataContext.LogIn.Add(adminUser);
                    _dataContext.SaveChanges();
                    _logger.LogInformation("Initial admin user created successfully");
                }

                var users = _dataContext.LogIn
                    .Include(u => u.Role)
                    .Where(u => u.UserName != User.Identity.Name)
                    .ToList();

                return View(users);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Index action: {ex.Message}");
                ViewBag.Error = "An error occurred while loading users. Please try again.";
                return View(new List<LogIn>());
            }
        }

        public IActionResult Create()
        {
            try
            {
                // Check if Roles table has data
                if (!_dataContext.Roles.Any())
                {
                    // Add roles if they don't exist
                    _dataContext.Roles.AddRange(
                        new Role { RoleId = 1, RoleName = "Admin" },
                        new Role { RoleId = 2, RoleName = "User" }
                    );
                    _dataContext.SaveChanges();
                }

                // Get roles for dropdown
                ViewBag.Roles = new SelectList(_dataContext.Roles, "RoleId", "RoleName");
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error in Create action: {ex.Message}");
                ViewBag.Error = "An error occurred. Please try again.";
                return View();
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Create(LogInCreateModel model)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    // Check if username already exists
                    if (_dataContext.LogIn.Any(u => u.UserName == model.UserName))
                    {
                        ModelState.AddModelError("UserName", "Username already exists");
                        ViewBag.Roles = new SelectList(_dataContext.Roles, "RoleId", "RoleName");
                        return View(model);
                    }

                    // Hash the password
                    string hashedPassword = BCrypt.Net.BCrypt.HashPassword(model.Password);

                    var newUser = new LogIn
                    {
                        UserName = model.UserName,
                        Password = hashedPassword,
                        RoleId = model.RoleId
                    };

                    _dataContext.LogIn.Add(newUser);
                    _dataContext.SaveChanges();

                    return RedirectToAction("Index");
                }

                ViewBag.Roles = new SelectList(_dataContext.Roles, "RoleId", "RoleName");
                return View(model);
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error creating user: {ex.Message}");
                ViewBag.Error = "An error occurred while creating the user. Please try again.";
                ViewBag.Roles = new SelectList(_dataContext.Roles, "RoleId", "RoleName");
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Delete(int id)
        {
            try
            {
                var user = _dataContext.LogIn.Find(id);
                if (user == null)
                {
                    return NotFound();
                }

                // Don't allow deleting the current admin
                if (user.UserName == User.Identity.Name)
                {
                    ViewBag.Error = "You cannot delete your own account.";
                    return View("Index", _dataContext.LogIn.Include(u => u.Role).ToList());
                }

                _dataContext.LogIn.Remove(user);
                _dataContext.SaveChanges();

                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError($"Error deleting user: {ex.Message}");
                ViewBag.Error = "An error occurred while deleting the user. Please try again.";
                return View("Index", _dataContext.LogIn.Include(u => u.Role).ToList());
            }
        }
    }
} 