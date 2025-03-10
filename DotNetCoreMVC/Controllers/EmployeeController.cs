using DotNetCoreMVC.Data;
using DotNetCoreMVC.Models.EmployeeModel;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreMVC.Controllers
{
    [Authorize] 
    public class EmployeeController : Controller
    {
        private readonly DataContext _dataContext;

        public EmployeeController(DataContext datacontext)
        {
            _dataContext = datacontext;
        }

        //public async Task<IActionResult> Index(string? searchValue = null)
        //{
        //    Console.WriteLine($"Authenticated User: {User.Identity?.Name}"); // Debugging Purpose

        //    var employees = await _dataContext.Employees.ToListAsync();

        //    if (!string.IsNullOrEmpty(searchValue) && int.TryParse(searchValue, out int employeeId))
        //    {
        //        employees = employees.Where(e => e.EmployeeID == employeeId).ToList();
        //    }

        //    return View(employees);
        //}
        public async Task<IActionResult> Index(string value)
        {
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
