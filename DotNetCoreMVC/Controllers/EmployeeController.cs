using DotNetCoreMVC.Data;
using DotNetCoreMVC.Models.EmployeeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;

namespace DotNetCoreMVC.Controllers
{
    [Authorize]
    public class EmployeeController : Controller
    {
        private readonly DataContext _dataContext;
        private readonly ILogger<EmployeeController> _logger;
        private readonly IJwtTokenService _jwtTokenService;

        public EmployeeController(DataContext datacontext, ILogger<EmployeeController> logger, IJwtTokenService jwtTokenService)
        {
            _dataContext = datacontext;
            _logger = logger;
            _jwtTokenService = jwtTokenService;
        }

        public async Task<IActionResult> Index(string value)
        {
            // Get token from the Authorization header
            var token = Request.Cookies["AuthToken"];

            var principal = _jwtTokenService.ValidateToken(token);

            if (principal == null)
            {
                // Handle invalid token or unauthorized access
                return Unauthorized();
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

            var query = _dataContext.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int employeeId))
            {
                query = query.Where(x => x.EmployeeID == employeeId);
            }

            var employees = await query.ToListAsync();
            return View(employees);
        }

        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Create(Employee employee)
        {
            if (ModelState.IsValid)
            {
                await _dataContext.Employees.AddAsync(employee);
                await _dataContext.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _dataContext.Update(employee);
                    await _dataContext.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmployeeExists(employee.EmployeeID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(employee);
        }

        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _dataContext.Employees
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee != null)
            {
                _dataContext.Employees.Remove(employee);
            }

            await _dataContext.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _dataContext.Employees.Any(e => e.EmployeeID == id);
        }
    }
}
