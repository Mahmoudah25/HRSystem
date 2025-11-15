using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models
{
    public class Payrolls
    {
        public int Id { get; set; }
        public int Month {  get; set; }
        public int Year {  get; set; }
        public decimal BaseSalary {  get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public decimal NetSalary { get; set; }


        //FK
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
