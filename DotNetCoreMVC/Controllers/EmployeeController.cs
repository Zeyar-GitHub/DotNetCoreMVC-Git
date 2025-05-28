using DotNetCoreMVC.Data;
using DotNetCoreMVC.Models.EmployeeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

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


            var query = _dataContext.Employees.AsQueryable();

            if (!string.IsNullOrEmpty(value) && int.TryParse(value, out int employeeId))
            {
                query = query.Where(x => x.EmployeeID == employeeId);
            }

            var employees = await query.ToListAsync();
            return View(employees);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(AddEmployeeViewModel addEmployeeViewModel)
        {
            if (!ModelState.IsValid)
            {
                return View(addEmployeeViewModel);
            }

            var employee = new Employee
            {
                EmployeeName = addEmployeeViewModel.EmployeeName,
                Designation = addEmployeeViewModel.Designation,
                Department = addEmployeeViewModel.Department
            };

            await _dataContext.Employees.AddAsync(employee);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        public IActionResult Edit(int id)
        {
            var employee = _dataContext.Employees.Find(id);
            if (employee == null)
            {
                return NotFound();
            }
            return View(employee);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public IActionResult Edit(int id, Employee employee)
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
                    _dataContext.SaveChanges();
                    return RedirectToAction("Index", "Home");
                }
                catch (Exception)
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
            }
            return View(employee);
        }

        public async Task<IActionResult> DeleteConfirm(int id)
        {
            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _dataContext.Employees.Remove(employee);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index", "Home");
        }

        private bool EmployeeExists(int id)
        {
            return _dataContext.Employees.Any(e => e.EmployeeID == id);
        }
    }
}
