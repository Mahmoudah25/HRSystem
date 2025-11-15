namespace HRManagmentSystem.DTOs.Employee
{
    public class EditEmployeeDTO
    {
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }
        public int DepartmentId { get; set; }
        public int PositionId { get; set; }
    }
}
