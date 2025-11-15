using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models
{
    public class ApplicationUser: IdentityUser<int>
    {
        public string FullName { get; set; } = string.Empty;

        //FK
        [ForeignKey("Employee")]
        public int? EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
