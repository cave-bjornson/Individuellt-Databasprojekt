namespace ConsoleApplication.Models;

public class Student : Person
{
    public Class? Class { get; set; }

    /// <inheritdoc />
    public Student() { }

    public IEnumerable<Course?> Courses { get; set; } = new List<Course?>();

    public IEnumerable<Grade?> Grades { get; set; } = new List<Grade?>();

    public IEnumerable<Employee?> Teachers { get; set; } = new List<Employee?>();

    /// <inheritdoc />
    public Student(int personId, string personalIdentityNumber, string firstName, string lastName)
        : base(personId, personalIdentityNumber, firstName, lastName) { }
}
