namespace ConsoleApplication.Models;

public class Class
{
    public int? ClassId { get; set; }
    public required string ClassName { get; set; }

    public ICollection<Student?> Students { get; set; } = new List<Student?>();
}
