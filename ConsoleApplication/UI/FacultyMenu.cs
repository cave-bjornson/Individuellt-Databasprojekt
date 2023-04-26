using ConsoleApplication.Models;
using Spectre.Console;

namespace ConsoleApplication.UI;

public class FacultyMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Faculty Administration";

    /// <inheritdoc />
    public void Show()
    {
        // AnsiConsole.Clear();
#pragma warning disable CS8714
        var menuSelection = AnsiConsole.Prompt(
            new SelectionPrompt<IMenu>()
                .Title(Name)!
                .AddChoices(
                    MenuService?.GetMenus(
                        nameof(GradeMenu),
                        nameof(ActiveCoursesMenu),
                        nameof(SalaryMenu),
                        "BackMenu"
                    )!
                )
                // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
                .UseConverter(menu => menu is null ? "Back" : menu.Name)
        );
#pragma warning restore CS8714
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (menuSelection is null)
        {
            return;
        }
        MenuService!.Navigate(this, menuSelection);
    }
}

public class GradeMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Grade Students";

    /// <inheritdoc />
    public void Show()
    {
        var teachers = MenuService!.EmployeeService
            .GetAllEmployees()
            .Where(e => e?.Position?.Title == "Teacher")
            .ToList();
        teachers.Add(null);

#pragma warning disable CS8714
        var teacher = AnsiConsole.Prompt(
            new SelectionPrompt<Employee>()
                .Title("Login as teacher")!
                .AddChoices(teachers)
                .UseConverter(
                    t => t is null ? "Cancel" : $"{t.FirstName} {t.LastName} {t.Faculty?.Name}"
                )
        );
#pragma warning restore CS8714

        if (teacher is null)
        {
            return;
        }

        var teacherCourses = MenuService.AdminService.GetCourseByTeacher(teacher).ToList();

        if (!teacherCourses.Any())
        {
            AnsiConsole.WriteLine("You don't teach any courses.");
            return;
        }

        teacherCourses.Add(null);
#pragma warning disable CS8714
        var course = AnsiConsole.Prompt(
            new SelectionPrompt<Course>()
                .Title("Select a Course")!
                .AddChoices(teacherCourses)
                .UseConverter(tC => tC is null ? "Cancel" : tC.Name)
        );
#pragma warning restore CS8714

        if (course is null)
        {
            return;
        }

        var courseStudents = MenuService.StudentService
            .GetUngradedStudentsInCourse(course)
            .ToList();

        if (!courseStudents.Any())
        {
            AnsiConsole.WriteLine("No Ungraded Students in Course");
            return;
        }

        courseStudents.Add(null);

#pragma warning disable CS8714
        var student = AnsiConsole.Prompt(
            new SelectionPrompt<Student>()
                .Title("Select a Student")!
                .AddChoices(courseStudents)
                .UseConverter(
                    cS =>
                        cS is null
                            ? "Cancel"
                            : $"{cS.PersonalIdentityNumber} {cS.FirstName} {cS.LastName}"
                )
        );
#pragma warning restore CS8714

        if (student is null)
        {
            return;
        }

        int? gradeInput = AnsiConsole.Prompt(
            new TextPrompt<int?>("Enter grade, Leave blank to cancel")
                .AllowEmpty()
                .DefaultValue(null)
                .ShowDefaultValue(false)
                .ValidationErrorMessage("Enter grade between 1 and 5")
#pragma warning disable CS8629
                .Validate(grade => Enumerable.Range(1, 5).Contains((int)grade))
#pragma warning restore CS8629
        );

        if (gradeInput is null)
        {
            return;
        }

        var gradeValue = (int)gradeInput;

        var grid = new Grid();
        grid.AddColumns(2);
        grid.AddRow(
            "Student:",
            $"{student.PersonalIdentityNumber} {student.FirstName} {student.LastName}"
        );
        grid.AddRow("Course:", course.Name);
        grid.AddRow("Grade:", gradeValue.ToString());
        AnsiConsole.Write(grid);
        if (AnsiConsole.Confirm("Save this grade?") == false)
        {
            return;
        }

        var grade = new Grade()
        {
            Course = course,
            Student = student,
            GradeValue = gradeValue,
            DateGraded = DateOnly.FromDateTime(DateTime.Today),
            Teacher = teacher
        };

        var success = MenuService.AdminService.AddGrade(grade);
        AnsiConsole.MarkupLine(success ? $"[green]Grade added[/]" : "[red]Failed to add Grade[/]");
    }
}

public class ActiveCoursesMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Active Courses";

    /// <inheritdoc />
    public void Show()
    {
        var courses = MenuService?.AdminService.GetCourses().ToList();
        var courseTable = new Table()
            .Title("Active Courses")
            .AddColumns(
                new TableColumn(new Markup("[green]Course[/]")),
                new TableColumn(new Markup("[green]Faculty[/]")),
                new TableColumn(new Markup("[green]Teacher[/]"))
            );
        if (courses!.Any())
        {
            foreach (Course? c in courses!)
            {
                courseTable.AddRow(
                    $"{c.Name}",
                    $"{c.Faculty?.Name}",
                    $"{(c.Teacher is null ? "Vacant" : c.Teacher?.FirstName + " " + c.Teacher?.LastName)} "
                );
            }
        }
        else
        {
            courseTable.AddRow("No active courses", "", "");
        }

        AnsiConsole.Write(courseTable);
    }
}

public class SalaryMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Salary Statistics";

    /// <inheritdoc />
    public void Show()
    {
        var totalSalaries = MenuService!.AdminService.GetFacultySalaries().ToList();
        var avgSalaries = MenuService.AdminService.GetFacultySalaries(average: true).ToList();

        var totalChart = new BarChart().Label("Total Salary per Faculty").CenterLabel();
        totalSalaries.ForEach(
            tuple =>
                totalChart.AddItem(
                    tuple.Name,
                    decimal.ToDouble(tuple.SalaryValue),
                    Color.FromInt32(Random.Shared.Next(0, 256))
                )
        );

        var averageChart = new BarChart().Label("Average Salary per Faculty").CenterLabel();
        avgSalaries.ForEach(
            tuple =>
                averageChart.AddItem(
                    tuple.Name,
                    decimal.ToDouble(tuple.SalaryValue),
                    Color.FromInt32(Random.Shared.Next(0, 256))
                )
        );

        AnsiConsole.Write(totalChart);
        AnsiConsole.Write(averageChart);
    }
}
