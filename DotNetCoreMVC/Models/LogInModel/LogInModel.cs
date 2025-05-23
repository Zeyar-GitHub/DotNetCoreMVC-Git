using System.ComponentModel.DataAnnotations;

namespace DotNetCoreMVC.Models.LogInModel
{
    public class LogInModel
    {
        public int LogInID { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }


    }
}
