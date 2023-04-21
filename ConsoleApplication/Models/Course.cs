namespace ConsoleApplication.Models;

public class Course
{
    public int? CourseId { get; set; }
    public required string Name { get; set; }

    public required bool Active { get; set; }

    public Employee? Teacher { get; set; }

    public Faculty? Faculty { get; set; }

    public IEnumerable<Student?> Students { get; set; } = new List<Student?>();
}
