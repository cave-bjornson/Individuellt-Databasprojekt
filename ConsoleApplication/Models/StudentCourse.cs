namespace ConsoleApplication.Models;

public class StudentCourse
{
    public int? StudentCourseId { get; set; }

    public required Student Student { get; set; }

    public required Course Course { get; set; }
}
