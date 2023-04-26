namespace ConsoleApplication.Models;

public class Student
{
    public int? StudentId { get; set; }

    public required string PersonalIdentityNumber { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public Class? Class { get; set; }

    public ICollection<Course?> Courses { get; init; } = new List<Course?>();

    public ICollection<Grade?> Grades { get; set; } = new List<Grade?>();

    public ICollection<Employee?> Teachers { get; set; } = new List<Employee?>();

    public ICollection<StudentTeacher?> StudentTeachers { get; set; } = new List<StudentTeacher?>();
}
