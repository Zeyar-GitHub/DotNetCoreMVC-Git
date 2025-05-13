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

        public IActionResult EmployeeCreate()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeCreate(AddEmployeeViewModel addEmployeeViewModel)
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

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EmployeeEdit(int id)
        {
            var employee = await _dataContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            var viewModel = new AddEmployeeViewModel
            {
                EmployeeID = employee.EmployeeID,
                EmployeeName = employee.EmployeeName,
                Designation = employee.Designation,
                Department = employee.Department
            };

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeEdit(int id, AddEmployeeViewModel model)
        {
            if (id != model.EmployeeID)
            {
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            employee.EmployeeName = model.EmployeeName;
            employee.Designation = model.Designation;
            employee.Department = model.Department;

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
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> EmployeeDeleteConfirm(int id)
        {
            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View(employee);
        }

        [HttpPost, ActionName("EmployeeDelete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EmployeeDeleteConfirmed(int id)
        {
            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _dataContext.Employees.Remove(employee);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _dataContext.Employees.Any(e => e.EmployeeID == id);
        }
    }
}
