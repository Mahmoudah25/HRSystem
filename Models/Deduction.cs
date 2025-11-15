using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models
{
    public class Deduction
    {
        public int Id { get; set; }
        public string Reason { get; set; } = string.Empty;
        [Range(0, double.MaxValue)]
        public decimal Amount {  get; set; }
        //FK
        [ForeignKey("Employee")]
        public int EmployeeId {  get; set; }    
        public Employee Employee { get; set; }
    }
}
