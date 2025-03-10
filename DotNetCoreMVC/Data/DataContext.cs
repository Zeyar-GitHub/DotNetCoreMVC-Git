using DotNetCoreMVC.Models.EmployeeModel;
using Microsoft.EntityFrameworkCore;

namespace DotNetCoreMVC.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions<DataContext> options) : base(options) { }

        public DbSet<Employee> Employees { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            // Map Employee entity to tblEmployee table
            modelBuilder.Entity<Employee>()
                        .ToTable("tblEmployee");
        }
    }
}
