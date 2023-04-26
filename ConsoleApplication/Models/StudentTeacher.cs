namespace ConsoleApplication.Models;

public class StudentTeacher
{
    public int StudentTeacherId { get; set; }
    public int StudentId { get; set; }
    public int TeacherID { get; set; }

    public Student? Student { get; set; }

    public Employee? Teacher { get; set; }
}
