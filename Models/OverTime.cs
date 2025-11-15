using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models
{
    public class OverTime
    {
        public int Id {  get; set; }
        public DateTime Date { get; set; }
        public double hours {  get; set; }
        public decimal Rate { get; set; }


        //FK
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
