namespace ConsoleApplication.Models;

public class Employee
{
    public int? EmployeeId { get; init; }

    public required string PersonalIdentityNumber { get; set; }

    public required string FirstName { get; set; }
    public required string LastName { get; set; }

    public Position? Position { get; set; }
    public DateOnly HireDate { get; set; }

    public decimal? Salary { get; set; }

    public Faculty? Faculty { get; set; }

    protected bool Equals(Employee other)
    {
        return EmployeeId == other.EmployeeId;
    }

    /// <inheritdoc />
    public override bool Equals(object? obj)
    {
        if (ReferenceEquals(null, obj))
            return false;
        if (ReferenceEquals(this, obj))
            return true;
        if (obj.GetType() != this.GetType())
            return false;
        return Equals((Employee)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return EmployeeId.GetHashCode();
    }

    public static bool operator ==(Employee? left, Employee? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Employee? left, Employee? right)
    {
        return !Equals(left, right);
    }
}
