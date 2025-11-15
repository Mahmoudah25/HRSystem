namespace HRManagmentSystem.Models
{
    public class Department
    {
        public int Id { get; set; }
        public string Name { get; set; } = string.Empty;
        public bool IsDeleted { get; set; } = false;

        //RelationShips
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();
    }
}
