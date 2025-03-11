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

        public EmployeeController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }
        
        public async Task<IActionResult> Index(string value)
        {
            //var claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList();
            //foreach (var claim in claims)
            //{
            //    Console.WriteLine($"Claim Type: {claim.Type}, Claim Value: {claim.Value}");
            //}

            var employees = await _dataContext.Employees.ToListAsync();
            if (!string.IsNullOrEmpty(value))
            {
                int employeeId;
                if (Int32.TryParse(value, out employeeId))
                {
                    employees = employees.Where(x => x.EmployeeID == employeeId).ToList();
                }
            }
            return View(employees);
        }


        public IActionResult EmployeeCreate()
        {
            return View();
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeCreate(AddEmployeeViewModel addEmployeeViewModel)
        {
            var employee = new Employee()
            {
                EmployeeName = addEmployeeViewModel.EmployeeName,
                Designation = addEmployeeViewModel.Designation,
                Department = addEmployeeViewModel.Department
            };
            await _dataContext.Employees.AddAsync(employee);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }

        public async Task<IActionResult> EmployeeEdit(int id)
        {
            var employee = await _dataContext.Employees.FirstOrDefaultAsync(x => x.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }
            var addEmployeeViewModel = new AddEmployeeViewModel()
            {
                EmployeeID = employee.EmployeeID,
                EmployeeName = employee.EmployeeName,
                Designation = employee.Designation,
                Department = employee.Department
            };
            return View(addEmployeeViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeEdit(int id, Employee employee)
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
            return RedirectToAction("Index");
        }

        private bool EmployeeExists(int id)
        {
            return _dataContext.Employees.Any(e => e.EmployeeID == id);
        }

        public async Task<IActionResult> EmployeeDeleteConfirm(int id)
        {
            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            return View("EmployeeDeleteConfirm", employee);
        }

        [HttpPost]
        public async Task<IActionResult> EmployeeDelete(int id)
        {
            var employee = await _dataContext.Employees.FindAsync(id);
            if (employee == null)
            {
                return NotFound();
            }

            _dataContext.Employees.Remove(employee);
            await _dataContext.SaveChangesAsync();

            return RedirectToAction("Index");
        }
    }
}
