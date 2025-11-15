using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models.Traning_Performance
{
    public class EmployeeTraning
    {
        //FKs
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }


        //FK
        [ForeignKey("traning")]
        public int TraningId { get; set; }
        public Traning traning { get; set; }
    }
}
