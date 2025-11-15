using System.Text.Json.Serialization;

namespace HRManagmentSystem.Models
{
    public class Position
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string Level { get; set; } = string.Empty;

        //RelationShips
        [JsonIgnore]
        public ICollection<Employee> Employees { get; set; } = new List<Employee>();

    }
}
