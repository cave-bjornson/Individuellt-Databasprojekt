namespace ConsoleApplication.Models;

public class Faculty
{
    public int? FacultyId { get; set; }
    public required string Name { get; set; }

    //public IEnumerable<Employee?> Employees { get; set; } = new List<Employee>();

    //public IEnumerable<Course?> Courses { get; set; } = new List<Course>();
}
