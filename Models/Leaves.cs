using System.ComponentModel.DataAnnotations.Schema;

namespace HRManagmentSystem.Models
{
    public enum LeaveStatus
    {
        Pending,
        Approved,
        Rejected

    }
    public class Leave
    {
        public int Id { get; set; }
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public  LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string? Reason { get; set; }

        //FK
        [ForeignKey("Employee")]
        public int EmployeeId { get; set; }
        public Employee Employee { get; set; }
    }
}
