﻿using System.ComponentModel.DataAnnotations;

namespace DotNetCoreMVC.Models
{
    public class LogInCreateModel
    {
        [Required(ErrorMessage = "Username is required")]
        [Display(Name = "Username")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Username must be between 3 and 50 characters")]
        public string UserName { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [DataType(DataType.Password)]
        [Display(Name = "Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters long")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Role")]
        public int RoleId { get; set; }
    }
}