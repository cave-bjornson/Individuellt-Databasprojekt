using ConsoleApplication.Data;
using ConsoleApplication.Models;
using ConsoleApplication.Services;
using Spectre.Console;

namespace ConsoleApplication;

[Command("run-tests")]
public class TestRunner : ConsoleAppBase
{
    private readonly HighSchoolDBContext _dbContext;
    private readonly StudentService _studentService;
    private readonly EmployeeService _employeeService;
    private readonly AdminService _adminService;

    /// <inheritdoc />
    public TestRunner(
        HighSchoolDBContext dbContext,
        StudentService studentService,
        EmployeeService employeeService,
        AdminService adminService
    )
    {
        _dbContext = dbContext;
        _studentService = studentService;
        _employeeService = employeeService;
        _adminService = adminService;
    }

    // Passed test
    [Command("test-insert-student")]
    public void TestInsert()
    {
        Student student = new Student
        {
            PersonalIdentityNumber = "8602071236",
            FirstName = "Yes",
            LastName = "Class",
            Class = new Class() { ClassName = "10B" }
        };
        var result = _studentService.AddStudent(student);
        AnsiConsole.WriteLine(result);
    }

    // Passed test
    [Command("get-student-by-id")]
    public void GetStudentById()
    {
        AnsiConsole.WriteLine(_studentService.GetStudent(2).Dump());
    }

    // Passed test
    [Command("test-insert-grade")]
    public void TestInsertGrade()
    {
        var s = new Student()
        {
            StudentId = 1024,
            PersonalIdentityNumber = "",
            FirstName = "",
            LastName = ""
        };
        var c = new Course()
        {
            CourseId = 1,
            Name = "",
            Active = false
        };
        var t = new Employee()
        {
            EmployeeId = 3,
            PersonalIdentityNumber = "",
            FirstName = "",
            LastName = ""
        };
        Grade g = new Grade()
        {
            GradeValue = 5,
            Course = c,
            Student = s,
            Teacher = t
        };
        AnsiConsole.WriteLine(g.Dump());

        var result = _adminService.AddGrade(g);
        AnsiConsole.WriteLine(result);
    }

    // Passed test
    [Command("test-insert-employee")]
    public void TestInsertEmployee()
    {
        var employee = new Employee
        {
            PersonalIdentityNumber = "8602071236",
            FirstName = "Lol",
            LastName = "Tolhurst",
            Position = new Position() { PositionId = 3, Title = "Teacher" },
            Faculty = new Faculty() { FacultyId = 1, Name = "" },
            Salary = 999
        };
        var result = _employeeService.AddEmployee(employee);
        AnsiConsole.WriteLine(result);
    }

    // Passed test
    [Command("get-all-employees")]
    public void GetAllEmployees()
    {
        IEnumerable<Employee?> employees = _employeeService.GetAllEmployees().ToList();
        foreach (var employee in employees)
        {
            AnsiConsole.WriteLine(employee.Dump());
        }
    }

    [Command("get-all-student-info")]
    public void GetAllStudentInfo()
    {
        AnsiConsole.WriteLine(_studentService.GetAllStudentInfo(2).Dump());
    }

    [Command("get-teachers-faculty")]
    public void RunMiscTests()
    {
        var faculties = _adminService.GetAllFacultiesWithTeachers();
        foreach (var faculty in faculties)
        {
            AnsiConsole.WriteLine(faculty.Employees.Count());
        }
    }

    [Command("get-all-students")]
    public void RunGetAllStudents()
    {
        var students = _studentService.GetAll().ToArray();
        foreach (var student in students)
        {
            AnsiConsole.WriteLine(
                $"{student?.StudentId} {student?.PersonalIdentityNumber} {student?.FirstName} {student?.LastName} {student?.Teachers.FirstOrDefault()?.FirstName}"
            );
        }
    }

    [Command("test-st-view")]
    public void RunStView()
    {
        AnsiConsole.WriteLine(_dbContext.StudentTeachers.ToList().Dump());
    }

    [Command("test-get-active-courses")]
    public void RunGetActiveCourses()
    {
        AnsiConsole.WriteLine(_adminService.GetCourses().Dump());
    }

    [Command("test-get-total-salaries")]
    public void RunGetTotalFacultySalary()
    {
        _adminService
            .GetFacultySalaries()
            .ForEach(
                (
                    tuple =>
                    {
                        var (facultyId, name, salaryValue) = tuple;
                        AnsiConsole.WriteLine("{0}, {1}, {2}", facultyId ?? 0, name, salaryValue);
                    }
                )
            );
    }

    [Command("test-get-average-salaries")]
    public void RunGetAverageFacultySalary()
    {
        _adminService
            .GetFacultySalaries(average: true)
            .ForEach(
                (
                    tuple =>
                    {
                        var (facultyId, name, salaryValue) = tuple;
                        AnsiConsole.WriteLine("{0}, {1}, {2}", facultyId ?? 0, name, salaryValue);
                    }
                )
            );
    }
}
