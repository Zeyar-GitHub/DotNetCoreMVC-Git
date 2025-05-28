using System.ComponentModel.DataAnnotations;

namespace DotNetCoreMVC.Models.EmployeeModel
{
    public class AddEmployeeViewModel
    {
        public int EmployeeID { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, ErrorMessage = "Name cannot be longer than 100 characters")]
        [Display(Name = "Name")]
        public string EmployeeName { get; set; }

        [Required(ErrorMessage = "Designation is required")]
        [StringLength(50, ErrorMessage = "Designation cannot be longer than 50 characters")]
        [Display(Name = "Designation")]
        public string Designation { get; set; }

        [Required(ErrorMessage = "Department is required")]
        [StringLength(50, ErrorMessage = "Department cannot be longer than 50 characters")]
        [Display(Name = "Department")]
        public string Department { get; set; }
    }
}
