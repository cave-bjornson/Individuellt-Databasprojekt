// using ConsoleApplication.Models;
// using Spectre.Console;
//
// namespace ConsoleApplication.UI;
//
// public class FacultyMenu : IMenu
// {
//     /// <inheritdoc />
//     public MenuService? MenuService { get; set; }
//
//     /// <inheritdoc />
//     public string Name { get; init; } = "Faculty Administration";
//
//     /// <inheritdoc />
//     public void Show()
//     {
//         // AnsiConsole.Clear();
//         var menuSelection = AnsiConsole.Prompt(
//             new SelectionPrompt<IMenu>()
//                 .Title(Name)
//                 .AddChoices(MenuService?.GetMenus("GradeMenu", "BackMenu")!)
//                 // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
//                 .UseConverter(menu => menu is null ? "Back" : menu.Name)
//         );
//         // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
//         if (menuSelection is null)
//         {
//             return;
//         }
//         MenuService!.Navigate(this, menuSelection);
//     }
// }
//
// public class GradeMenu : IMenu
// {
//     /// <inheritdoc />
//     public MenuService? MenuService { get; set; }
//
//     /// <inheritdoc />
//     public string Name { get; init; } = "Grade Students";
//
//     /// <inheritdoc />
//     public void Show()
//     {
//         var teachers = MenuService!.EmployeeService
//             .GetAll()
//             .Where(e => e.Position?.Title == "Teacher")
//             .ToList();
//
// #pragma warning disable CS8714
//         var teacher = AnsiConsole.Prompt(
//             new SelectionPrompt<Employee>()
//                 .Title("Login as teacher")!
//                 .AddChoices(teachers.Append(null))
//                 .UseConverter(
//                     t => t is null ? "Cancel" : $"{t.FirstName} {t.LastName} {t.Faculty?.Name}"
//                 )
//         );
// #pragma warning restore CS8714
//
//         if (teacher is null)
//         {
//             return;
//         }
//
//         var teacherCourses = teacher.Courses.ToList();
//
//         if (!teacherCourses!.Any())
//         {
//             AnsiConsole.WriteLine("You don't teach any courses.");
//             return;
//         }
//
// #pragma warning disable CS8714
//         var course = AnsiConsole.Prompt(
//             new SelectionPrompt<Course>()
//                 .Title("Select a Course")!
//                 .AddChoices(teacherCourses!)
//                 .UseConverter(tC => tC!.Name)
//         );
// #pragma warning restore CS8714
//
//         var courseStudents = course!.Students.ToList();
//
//         if (!courseStudents.Any())
//         {
//             AnsiConsole.WriteLine("No Students in Course");
//             return;
//         }
//
// #pragma warning disable CS8714
//         var student = AnsiConsole.Prompt(
//             new SelectionPrompt<Student>()
//                 .Title("Select a Student")!
//                 .AddChoices(courseStudents)
//                 .UseConverter(cS => $"{cS?.PersonalIdentityNumber} {cS?.FirstName} {cS?.LastName}")
//         );
// #pragma warning restore CS8714
//
//         AnsiConsole.WriteLine($"Print grades here count {student.Grades.Count()}");
//         foreach (var studentGrade in student.Grades)
//         {
//             AnsiConsole.WriteLine($"Current grades: {studentGrade}");
//         }
//
//         AnsiConsole.Confirm("pause");
//         var gradeValue = AnsiConsole.Prompt(
//             new TextPrompt<int>("Enter grade")
//                 .ValidationErrorMessage("Enter grade between 1 and 5")
//                 .Validate(grade => Enumerable.Range(1, 5).Contains(grade))
//         );
//
//         var grid = new Grid();
//         grid.AddColumns(2);
//         grid.AddRow(
//             "Student:",
//             $"{student?.PersonalIdentityNumber} {student?.FirstName} {student?.LastName}"
//         );
//         grid.AddRow("Course:", course.Name);
//         grid.AddRow("Grade:", gradeValue.ToString());
//         AnsiConsole.Write(grid);
//         if (AnsiConsole.Confirm("Save this grade?") == false)
//         {
//             return;
//         }
//
//         var grade = new Grade()
//         {
//             Course = course,
//             Student = student!,
//             GradeValue = (short)gradeValue,
//             DateGraded = DateOnly.FromDateTime(DateTime.Today),
//             Teacher = teacher
//         };
//
//         var newGrade = MenuService.AdminService.AddGrade(grade);
//         AnsiConsole.WriteLine("Grade " + newGrade.ToString());
//         AnsiConsole.MarkupLine(
//             newGrade is not null ? $"[green]Grade added[/]" : "[red]Failed to add Grade[/]"
//         );
//
//         var testStudent = MenuService.StudentService.GetByPIN(student.PersonalIdentityNumber);
//     }
// }
//
// public class ActiveCoursesMenu : IMenu
// {
//     /// <inheritdoc />
//     public MenuService? MenuService { get; set; }
//
//     /// <inheritdoc />
//     public string Name { get; init; } = "Faculty Administration";
//
//     /// <inheritdoc />
//     public void Show()
//     {
//         
//     }
// } 
