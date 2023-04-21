namespace ConsoleApplication.Models;

public class Employee : Person
{
    public Position? Position { get; set; }
    public DateOnly HireDate { get; set; }

    public decimal? Salary { get; set; }

    public Faculty? Faculty { get; set; }

    public IEnumerable<Course?> Courses { get; set; } = new List<Course?>();

    /// <inheritdoc />
    public Employee() { }

    /// <inheritdoc />
    public Employee(int personId, string personalIdentityNumber, string firstName, string lastName)
        : base(personId, personalIdentityNumber, firstName, lastName) { }
}
