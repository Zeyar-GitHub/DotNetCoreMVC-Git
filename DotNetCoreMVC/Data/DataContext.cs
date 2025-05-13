using DotNetCoreMVC.Models.EmployeeModel;
using DotNetCoreMVC.Models.LogInModel;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreMVC.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }
        public DbSet<LogIn> LogIn { get; set; }

    }
}
