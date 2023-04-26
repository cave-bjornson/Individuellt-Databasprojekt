namespace ConsoleApplication.Models;

public class Course
{
    public int? CourseId { get; init; }
    public required string Name { get; set; }

    public required bool Active { get; set; }

    public Employee? Teacher { get; set; }

    public Faculty? Faculty { get; set; }

    public ICollection<Student?> Students { get; set; } = new List<Student?>();

    protected bool Equals(Course other)
    {
        return CourseId == other.CourseId;
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
        return Equals((Course)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return CourseId.GetHashCode();
    }

    public static bool operator ==(Course? left, Course? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Course? left, Course? right)
    {
        return !Equals(left, right);
    }
}
