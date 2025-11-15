namespace HRManagmentSystem.DTOs.OverTime
{
    public class AddOverTimeDTO
    {
        public DateTime Date { get; set; }
        public double hours { get; set; }
        public decimal Rate { get; set; }
        public int EmployeeId { get; set; }
    }
}
