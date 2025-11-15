namespace HRManagmentSystem.DTOs.Deduction
{
    public class AddDeductionDTO
    {
        public string Reason { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int EmployeeId { get; set; }

    }
}
