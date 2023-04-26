namespace ConsoleApplication.Models;

public class Grade
{
    public int? GradeId { get; init; }

    public Student Student { get; set; } = null!;

    public Course Course { get; set; } = null!;

    public required int GradeValue { get; set; }

    public DateOnly DateGraded { get; set; }

    public required Employee Teacher { get; set; }

    protected bool Equals(Grade other)
    {
        return GradeId == other.GradeId;
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
        return Equals((Grade)obj);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return GradeId.GetHashCode();
    }

    public static bool operator ==(Grade? left, Grade? right)
    {
        return Equals(left, right);
    }

    public static bool operator !=(Grade? left, Grade? right)
    {
        return !Equals(left, right);
    }
}
