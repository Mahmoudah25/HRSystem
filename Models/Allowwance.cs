using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models
{
    public class Allowwance
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }


        //FK
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }

    }
}
