using ConsoleApplication.Models;
using Spectre.Console;
using static ConsoleApplication.UI.UIHelpers;

namespace ConsoleApplication.UI;

public class StudentMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Student Administration";

    /// <inheritdoc />
    public void Show()
    {
        AnsiConsole.Write(new Rule($"[yellow]{this.Name}[/]"));
#pragma warning disable CS8714
        var menuSelection = AnsiConsole.Prompt(
            new SelectionPrompt<IMenu>()
                .Title(Name)!
                .AddChoices(
                    MenuService?.GetMenus(
                        "StudentTableMenu",
                        "SingleStudentInfoMenu",
                        "AddStudentMenu",
                        "BackMenu"
                    )!
                )
                .UseConverter(menu => menu is null ? "Back" : menu.Name)
#pragma warning restore CS8714
        );
        if (menuSelection is null)
        {
            return;
        }
        MenuService?.Navigate(this, menuSelection);
    }

    public class StudentTableMenu : IMenu
    {
        /// <inheritdoc />
        public MenuService? MenuService { get; set; }

        /// <inheritdoc />
        public string Name { get; init; } = "Student Table";

        /// <inheritdoc />
        public void Show()
        {
            var students = MenuService?.StudentService.GetAll().ToList();

            if (students is not null && students.Any())
            {
                var studentTable = new Table()
                    .Title("Students")
                    .AddColumns("Student ID", "SSN", "First Name", "Last Name", "Class");
                foreach (Student? s in students)
                {
                    studentTable.AddRow(
                        s!.StudentId?.ToString() ?? "Not set",
                        s.PersonalIdentityNumber,
                        s.FirstName,
                        s.LastName,
                        s.Class?.ClassName ?? "None"
                    );
                }
                AnsiConsole.Write(studentTable);
            }
        }
    }

    public class AddStudentMenu : IMenu
    {
        /// <inheritdoc />
        public MenuService? MenuService { get; set; }

        /// <inheritdoc />
        public string Name { get; init; } = "Add Student";

        /// <inheritdoc />
        public void Show()
        {
            var (firstName, lastName, pin) = AskNamesAndPIN();
            var selectedClass = AnsiConsole.Prompt(
                new SelectionPrompt<Class>()
                    .Title("Select a class to enroll Student in")
                    .AddChoices(MenuService?.AdminService.GetAllClasses().ToList()!)
                    .UseConverter(c => c.ClassName)
            );

            var grid = new Grid();
            grid.AddColumns(2);
            grid.AddRow("First name:", firstName);
            grid.AddRow("Last name:", lastName);
            grid.AddRow("PIN:", pin);
            grid.AddRow("Class:", selectedClass.ClassName);
            AnsiConsole.Write(grid);
            if (AnsiConsole.Confirm("Save this student?") == false)
            {
                return;
            }

            var student = new Student()
            {
                FirstName = firstName,
                LastName = lastName,
                PersonalIdentityNumber = pin,
                Class = selectedClass
            };

            var success = MenuService?.StudentService.AddStudent(student);
            AnsiConsole.MarkupLine(
                success is not null && (bool)success
                    ? $"[green]Student {student.FirstName} {student.LastName} added[/]"
                    : "[red]Failed to add student[/]"
            );
        }
    }

    public class SingleStudentInfoMenu : IMenu
    {
        /// <inheritdoc />
        public MenuService? MenuService { get; set; }

        /// <inheritdoc />
        public string Name { get; init; } = "Student Info";

        /// <inheritdoc />
        public void Show()
        {
            // TODO Add validation.
            int? selectedStudentID = AnsiConsole.Prompt(
                (
                    new TextPrompt<int?>("Enter a students ID, Leave blank to cancel")
                        .AllowEmpty()
                        .DefaultValue(null)
                        .ShowDefaultValue(false)
                )
            );

            if (selectedStudentID is null)
            {
                return;
            }

            int unNullStudentId = (int)selectedStudentID;

            Student? student = MenuService?.StudentService.GetAllStudentInfo(unNullStudentId);

            if (student is null)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]No student with ID {unNullStudentId}[/]");
                return;
            }

            var studentGrid = new Grid()
                .AddColumns(2)
                .AddRow(new Markup("[blue]Student ID[/]"), new Text(student.StudentId.ToString()!))
                .AddRow(
                    new Markup("[blue]Name:[/]"),
                    new Text($"{student.FirstName} {student.LastName}")
                )
                .AddRow(new Markup("[blue]SSN:[/]"), new Text($"{student.PersonalIdentityNumber}"))
                .AddRow(
                    new Markup("[blue]Class:[/]"),
                    new Text($"{student.Class?.ClassName ?? "None"}")
                );

            var teachers = student.Teachers.ToList();

            var teacherGrid = new Grid().AddColumns(1);
            if (teachers!.Any<Employee>())
            {
                foreach (Employee? t in teachers)
                {
                    teacherGrid.AddRow($"{t?.FirstName} {t?.LastName}");
                }
            }
            else
            {
                teacherGrid.AddRow("No Teachers");
            }

            var courses = student.Courses.ToList();
            var courseTable = new Table().AddColumns(
                new TableColumn(new Markup("[green]Course[/]")),
                new TableColumn(new Markup("[green]Teacher[/]"))
            );
            if (courses.Any())
            {
                foreach (Course? c in courses)
                {
                    courseTable.AddRow(
                        $"{c?.Name}",
                        $"{c?.Teacher?.FirstName} {c?.Teacher?.LastName}"
                    );
                }
            }
            else
            {
                courseTable.AddRow("No courses registered", "No courses registered");
            }

            var grades = student.Grades.ToList();

            var gradeTable = new Table().AddColumns(
                new TableColumn[]
                {
                    new TableColumn(new Markup("[red]Course[/]")),
                    new TableColumn(new Markup("[red]Grade[/]")),
                    new TableColumn(new Markup("[red]Date[/]")),
                    new TableColumn(new Markup("[red]Teacher[/]"))
                }
            );

            if (courses.Any())
            {
                foreach (Grade? g in grades)
                {
                    gradeTable.AddRow(
                        g!.Course.Name,
                        g.GradeValue.ToString(),
                        g.DateGraded.ToString(),
                        $"{g.Teacher.FirstName} {g.Teacher.LastName}"
                    );
                }
            }
            else
            {
                gradeTable.AddRow("No grades registered");
            }

            // Render the layout
            AnsiConsole.Clear();
            AnsiConsole.Write(new Panel(studentGrid).Expand().Header("Student"));
            AnsiConsole.Write(new Panel(teacherGrid).Expand().Header("Teachers"));
            AnsiConsole.Write(new Panel(courseTable).Expand().Header(("Courses")));
            AnsiConsole.Write(new Panel(gradeTable).Expand().Header("Grades"));

            AnsiConsole.MarkupLine("[grey]Press any key to continue[/]");
            Console.ReadKey();
        }
    }
}
