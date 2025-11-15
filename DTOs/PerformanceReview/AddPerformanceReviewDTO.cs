namespace HRManagmentSystem.DTOs.PerformanceReview
{
    public class AddPerformanceReviewDTO
    {
        public DateTime ReviewDate { get; set; }
        public String Comments { get; set; } = string.Empty;
        public int Score { get; set; }
        public int EmployeeId { get; set; }

    }
}
