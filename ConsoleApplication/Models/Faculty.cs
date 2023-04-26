namespace ConsoleApplication.Models;

public class Faculty
{
    public int? FacultyId { get; set; }
    public required string Name { get; set; }

    public ICollection<Employee?> Employees { get; set; } = new List<Employee?>();
}
