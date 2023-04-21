using ConsoleApplication.Data;
using ConsoleApplication.Models;
using ConsoleApplication.Repositories;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Spectre.Console;

namespace ConsoleApplication;

[Command("run-tests")]
public class TestRunner : ConsoleAppBase
{
    private readonly HighSchoolDBContext _dbContext;
    private readonly StudentRepository _studentRepository;
    private readonly EmployeeRepository _employeeRepository;
    private readonly FacultyRepository _facultyRepository;

    /// <inheritdoc />
    public TestRunner(
        HighSchoolDBContext dbContext,
        StudentRepository studentRepository,
        EmployeeRepository employeeRepository,
        FacultyRepository facultyRepository
    )
    {
        _dbContext = dbContext;
        _studentRepository = studentRepository;
        _employeeRepository = employeeRepository;
        _facultyRepository = facultyRepository;
    }

    // Passed test
    [Command("test-insert-student")]
    public void TestInsert()
    {
        Student student = new Student(0, "8602071236", "Yes", "Class");
        student.Class = new Class() { ClassName = "10B" };
        var result = _studentRepository.AddStudent(student);
        AnsiConsole.WriteLine(result);
    }

    // Passed test
    [Command("get-student-by-id")]
    public void GetStudentById()
    {
        AnsiConsole.WriteLine(_studentRepository.GetStudent(2).Dump());
    }

    // Passed test
    [Command("test-insert-grade")]
    public void TestInsertGrade()
    {
        var s = new Student(1024, "", "", "");
        var c = new Course()
        {
            CourseId = 1,
            Name = "",
            Active = false
        };
        var t = new Employee(3, "", "", "");
        Grade g = new Grade()
        {
            GradeValue = 5,
            Course = c,
            Student = s,
            Teacher = t
        };
        AnsiConsole.WriteLine(g.Dump());

        var result = _studentRepository.AddGrade(g);
        AnsiConsole.WriteLine(result);
    }

    // Passed test
    [Command("test-insert-employee")]
    public void TestInsertEmployee()
    {
        var employee = new Employee(0, "8602071236", "Lol", "Tolhurst");
        employee.Position = new Position() { PositionId = 3, Title = "Teacher" };
        employee.Faculty = new Faculty() { FacultyId = 1, Name = "" };
        employee.Salary = 999;
        var result = _employeeRepository.AddEmployee(employee);
        AnsiConsole.WriteLine(result);
    }

    // Passed test
    [Command("get-all-employees")]
    public void GetAllEmployees()
    {
        IEnumerable<Employee?> employees = _employeeRepository.GetAllEmployees().ToList();
        foreach (var employee in employees)
        {
            AnsiConsole.WriteLine(employee.Dump());
        }
    }

    [Command("get-all-student-info")]
    public void GetAllStudentInfo()
    {
        AnsiConsole.WriteLine(_studentRepository.GetAllStudentInfo(2).Dump());
    }

    [Command("misc")]
    public void RunMiscTests()
    {
        AnsiConsole.WriteLine(_facultyRepository.GetAllFaculties().Dump());
    }
}
