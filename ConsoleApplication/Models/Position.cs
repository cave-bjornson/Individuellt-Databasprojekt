namespace ConsoleApplication.Models;

public class Position
{
    public int? PositionId { get; set; }
    public required string Title { get; set; }

    public ICollection<Employee?> Employees { get; set; } = new List<Employee?>();
}
