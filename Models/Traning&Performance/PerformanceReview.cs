using System.ComponentModel.DataAnnotations.Schema;
using System.Globalization;

namespace HRManagmentSystem.Models.Traning_Performance
{
    public class PerformanceReview
    {
        public int Id {  get; set; }
        public DateTime ReviewDate { get; set; }
        public String Comments {  get; set; }  = string.Empty;
        public int Score { get; set; }

        //Fk
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }


    }
}
