namespace HRManagmentSystem.DTOs.Department
{
    public class EmpLoyeeDTO
    {
        public int Id { get; set; }
        public string FullName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public decimal Salary { get; set; }
        public DateTime HireDate { get; set; }


    }
    public class DeptWithEmpDTO
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public int TotalEmployee { set; get; }
        public List<EmpLoyeeDTO> EmpLoyees { get; set; } = new();
    }
}
