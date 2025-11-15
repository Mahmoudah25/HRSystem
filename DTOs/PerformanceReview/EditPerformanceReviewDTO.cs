namespace HRManagmentSystem.DTOs.PerformanceReview
{
    public class EditPerformanceReviewDTO
    {
        public DateTime ReviewDate { get; set; }
        public String Comments { get; set; } = string.Empty;
        public int Score { get; set; }
    }
}
