using HRManagmentSystem.Models;

namespace HRManagmentSystem.DTOs.Leave
{
    public class UpdateLeaveDTO
    {
        public string LeaveType { get; set; } = string.Empty;
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public LeaveStatus Status { get; set; } = LeaveStatus.Pending;
        public string? Reason { get; set; }
    }
}
