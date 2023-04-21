namespace ConsoleApplication.Models;

public class Grade
{
    public int? GradeId { get; set; }
    public required Course Course { get; set; }
    public required Student Student { get; set; }
    public required short GradeValue { get; set; }

    public DateOnly DateGraded { get; set; }

    public required Employee Teacher { get; set; }

    /// <inheritdoc />
    public override string ToString()
    {
        return $"{nameof(Course)}: {Course.Name}, {nameof(Student)}: {Student.PersonalIdentityNumber}, {nameof(GradeValue)}: {GradeValue}, {nameof(DateGraded)}: {DateGraded}, {nameof(Teacher)}: {Teacher.PersonalIdentityNumber}";
    }
}
