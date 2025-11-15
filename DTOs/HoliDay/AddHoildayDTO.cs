namespace HRManagmentSystem.DTOs.HoliDay
{
    public class AddHoildayDTO
    {
        public string Name { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public bool IsRecurring { get; set; }
    }
}
