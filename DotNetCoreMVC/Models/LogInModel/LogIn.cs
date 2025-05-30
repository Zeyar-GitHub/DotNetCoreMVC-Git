using System.ComponentModel.DataAnnotations;

namespace DotNetCoreMVC.Models
{
    public class LogIn
    {
        [Key]
        public int LogInID { get; set; }

        [Required]
        [StringLength(50)]
        public string UserName { get; set; }

        [Required]
        [StringLength(100)]
        public string Password { get; set; }

        // Role Management
        public int RoleId { get; set; }
        public Role Role { get; set; }
    }
}