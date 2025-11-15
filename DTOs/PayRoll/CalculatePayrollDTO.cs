namespace HRManagmentSystem.DTOs.PayRoll
{
    public class CalculatePayrollDTO
    {
        public int Month { get; set; }
        public int Year { get; set; }
        public decimal BaseSalary { get; set; }
        public decimal Allowances { get; set; }
        public decimal Deductions { get; set; }
        public int EmployeeId { get; set; }
    }
}
