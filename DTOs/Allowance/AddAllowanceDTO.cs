namespace HRManagmentSystem.DTOs.Allowance
{
    public class AddAllowanceDTO
    {
        public string Type { get; set; } = string.Empty;
        public decimal Amount { get; set; }
        public int EmployeeId { get; set; }

    }
}
