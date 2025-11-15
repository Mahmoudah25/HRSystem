using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models
{
    public class Attendance
    {
        [Key]
        public int Id {  get; set; }
        public DateTime Date { get; set; }
        public DateTime? CheckIn {  get; set; }
        public DateTime? CheckOut { get; set; }
        public double? HoursWorked {  get; set; }
        public string Status {  get; set; } = string.Empty;


        //FK
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
