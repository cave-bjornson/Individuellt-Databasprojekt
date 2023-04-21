using System.Collections;
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
        var menuSelection = AnsiConsole.Prompt(
            new SelectionPrompt<IMenu>()
                .Title(Name)
                .AddChoices(
                    MenuService?.GetMenus(
                        //"StudentTableMenu",
                        "SingleStudentInfoMenu",
                        //"AddStudentMenu",
                        "BackMenu"
                    )!
                )
                .UseConverter(menu => menu is null ? "Back" : menu.Name)
        );
        if (menuSelection is null)
        {
            return;
        }
        // MenuService.Navigate(this, menuSelection);
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

            // if (students is not null && students.Any())
            // {
            //     var staffTable = new Table()
            //         .Title("Students")
            //         .AddColumns("Student ID", "SSN", "Name", "Class");
            //     foreach (Student s in students)
            //     {
            //         staffTable.AddRow(
            //             s.PersonId.ToString(),
            //             s.PersonalIdentityNumber,
            //             $"{s.FirstName} {s.LastName}",
            //             s.Class?.ClassName ?? "None"
            //         );
            //     }
            //     AnsiConsole.Write(staffTable);
            // }
        }
    }

    // public class AddStudentMenu : IMenu
    // {
    //     /// <inheritdoc />
    //     public MenuService? MenuService { get; set; }
    //
    //     /// <inheritdoc />
    //     public string Name { get; init; } = "Add Student";
    //
    //     /// <inheritdoc />
    //     public void Show()
    //     {
    //         var (firstName, lastName, pin) = AskNamesAndPIN();
    //         var selectedClass = AnsiConsole.Prompt(
    //             new SelectionPrompt<Class>()
    //                 .Title("Select a class to enroll Student in")
    //                 .AddChoices(MenuService?.AdminService.GetAllClasses().ToList()!)
    //                 .UseConverter(c => c.ClassName)
    //         );
    //
    //         var grid = new Grid();
    //         grid.AddColumns(2);
    //         grid.AddRow("First name:", firstName);
    //         grid.AddRow("Last name:", lastName);
    //         grid.AddRow("PIN:", pin);
    //         grid.AddRow("Class:", selectedClass.ClassName);
    //         AnsiConsole.Write(grid);
    //         if (AnsiConsole.Confirm("Save this student?") == false)
    //         {
    //             return;
    //         }
    //
    //         var student = new Student()
    //         {
    //             FirstName = firstName,
    //             LastName = lastName,
    //             PersonalIdentityNumber = pin,
    //             Class = selectedClass
    //         };
    //
    //         //var newStudent = MenuService?.StudentService.Add(student);
    //         // AnsiConsole.MarkupLine(
    //         //     newStudent is not null
    //         //         ? $"[green]Student {newStudent.FirstName} {newStudent.LastName} added[/]"
    //         //         : "[red]Failed to add student[/]"
    //         // );
    //     }
    // }

    public class SingleStudentInfoMenu : IMenu
    {
        /// <inheritdoc />
        public MenuService? MenuService { get; set; }

        /// <inheritdoc />
        public string Name { get; init; } = "Student Info";

        /// <inheritdoc />
        public void Show()
        {
            AnsiConsole.Clear();

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

            Student? student = MenuService.StudentService.GetStudentByID(unNullStudentId);

            if (student is null)
            {
                AnsiConsole.MarkupLineInterpolated($"[red]No student with ID {unNullStudentId}[/]");
                return;
            }

            var studentGrid = new Grid()
                .AddColumns(2)
                .AddRow(new Markup("[blue]Student ID[/]"), new Text(student.PersonId.ToString()!))
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
            var courseGrid = new Grid()
                .AddColumns(2)
                .AddRow(new Markup("[green]Course[/]"), new Markup("[green]Teacher[/]"));
            if (courses.Any())
            {
                foreach (Course? c in courses)
                {
                    courseGrid.AddRow(
                        $"{c?.Name}",
                        $"{c?.Teacher?.FirstName} {c?.Teacher?.LastName}"
                    );
                }
            }
            else
            {
                courseGrid.AddRow("No courses registered");
            }

            var grades = student.Grades.ToList();

            var gradeGrid = new Grid()
                .AddColumns(4)
                .AddRow(
                    new Markup("[red]Course[/]"),
                    new Markup("[red]Grade[/]"),
                    new Markup("[red]Date[/]"),
                    new Markup("[red]Teacher[/]")
                );

            if (courses.Any())
            {
                foreach (Grade g in grades)
                {
                    gradeGrid.AddRow(
                        g.Course.Name,
                        g.GradeValue.ToString(),
                        g.DateGraded.ToString(),
                        $"{g.Teacher.FirstName} {g.Teacher.LastName}"
                    );
                }
            }
            else
            {
                gradeGrid.AddRow("No grades registered");
            }

            //Create the layout
            var layout = new Layout("Root").SplitRows(
                new Layout("Top").SplitRows(new Layout("Personal Info"), new Layout("Teachers")),
                new Layout("Bottom").SplitRows(new Layout("Courses"), new Layout("Grades"))
            );

            // Update the top row
            layout["Top"]["Personal Info"].Update(
                new Panel(studentGrid).Expand().Header("Student")
            );

            layout["Top"]["Teachers"].Update(new Panel(teacherGrid).Expand().Header("Teachers"));

            // Update the bottom row
            layout["Bottom"]["Courses"].Update(new Panel(courseGrid).Expand().Header("Courses"));

            layout["Bottom"]["Grades"].Update(new Panel(gradeGrid).Expand().Header("Grades"));

            // Render the layout
            AnsiConsole.Write(layout);

            AnsiConsole.MarkupLine("[grey]Press any key to continue[/]");
            Console.ReadKey();
        }
    }
}
