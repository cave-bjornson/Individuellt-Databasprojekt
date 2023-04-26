using System.Globalization;
using ConsoleApplication.Models;
using Spectre.Console;
using static ConsoleApplication.UI.UIHelpers;

namespace ConsoleApplication.UI;

public class StaffMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Staff Administration";

    /// <inheritdoc />
    public void Show()
    {
        AnsiConsole.Write(new Rule($"[yellow]{this.Name}[/]"));
#pragma warning disable CS8714
        var menuSelection = AnsiConsole.Prompt(
            new SelectionPrompt<IMenu>()
                .Title(Name)!
                .AddChoices(MenuService?.GetMenus("StaffTableMenu", "AddEmployeeMenu", "BackMenu")!)
                .UseConverter(menu => menu is null ? "Back" : menu.Name)
        );
#pragma warning restore CS8714
        if (menuSelection is null)
        {
            return;
        }
        MenuService!.Navigate(this, menuSelection);
    }
}

public class StaffTableMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Staff Table";

    /// <inheritdoc />
    public void Show()
    {
        var staff = MenuService?.EmployeeService.GetAllEmployees().ToList();

        if (staff is not null && staff.Any())
        {
            var staffTable = new Table()
                .Title("Staff")
                .AddColumns("SSN", "Name", "Position", "Faculty", "Salary", "Years");
            foreach (Employee? e in staff)
            {
                staffTable.AddRow(
                    e!.PersonalIdentityNumber,
                    $"{e.FirstName} {e.LastName}",
                    e.Position?.Title ?? "None",
                    e.Faculty?.Name ?? "None",
                    $"{e.Salary:C2}",
                    $"{DateTime.Now.Year - e.HireDate.Year}"
                );
            }
            AnsiConsole.Write(staffTable);
        }
    }
}

public class AddEmployeeMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Add Employee";

    /// <inheritdoc />
    public void Show()
    {
        var (firstName, lastName, pin) = AskNamesAndPIN();
        var selectedPosition = AnsiConsole.Prompt(
            new SelectionPrompt<Position>()
                .Title("Select a position")
                .AddChoices(MenuService?.AdminService.GetAllPositions().ToList()!)
                .UseConverter(p => p.Title)
        );

        var canSelectFaculty = selectedPosition.Title switch
        {
            "Teacher" => true,
            "Administrator" => true,
            _ => false
        };

        Faculty? faculty = null;
        if (canSelectFaculty)
        {
#pragma warning disable CS8714
            faculty = AnsiConsole.Prompt(
                new SelectionPrompt<Faculty>()
                    .Title("Select a faculty")!
                    .AddChoices(MenuService?.AdminService.GetAllFaculties().Append(null).ToList()!)
                    .UseConverter(f => f is null ? "None" : f.Name)
            );
#pragma warning restore CS8714
        }

        var salary = AnsiConsole.Prompt(
            new TextPrompt<decimal>("Enter monthly salary")
                .ValidationErrorMessage("Not a valid input")
                .Validate(
                    salary =>
                        salary >= 0M
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Enter a positive decimal value")
                )
        );

        var currentYear = DateTime.Today.Year;
        var yearHired = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter hiring year")
                .DefaultValue(currentYear)
                .ValidationErrorMessage("Not a valid input")
                .Validate(
                    year =>
                        year >= 1900 && year <= currentYear
                            ? ValidationResult.Success()
                            : ValidationResult.Error($"Enter a year between 1900 and {currentYear}")
                )
        );
        var monthHired = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter hiring month")
                .DefaultValue(DateTime.Today.Month)
                .ValidationErrorMessage("Not a valid month")
                .Validate(
                    month =>
                        Enumerable.Range(1, 12).Contains(month)
                            ? ValidationResult.Success()
                            : ValidationResult.Error("Enter a number between 1 and 12")
                )
        );

        var daysInSelectedMonth = DateTime.DaysInMonth(yearHired, monthHired);
        var dayHired = AnsiConsole.Prompt(
            new TextPrompt<int>("Enter hiring day")
                .DefaultValue(DateTime.Today.Day)
                .ValidationErrorMessage("Not a valid day of month")
                .Validate(
                    day =>
                        Enumerable.Range(1, daysInSelectedMonth).Contains(day)
                            ? ValidationResult.Success()
                            : ValidationResult.Error(
                                $"Enter a number between 1 and {daysInSelectedMonth}"
                            )
                )
        );

        var dateHired = new DateOnly(yearHired, monthHired, dayHired);

        var grid = new Grid();
        grid.AddColumns(2);
        grid.AddRow("First name:", firstName);
        grid.AddRow("Last name:", lastName);
        grid.AddRow("PIN:", pin);
        grid.AddRow("Position:", selectedPosition.Title);
        grid.AddRow("Monthly Salary:", salary.ToString(CultureInfo.InvariantCulture));
        grid.AddRow("Date Hired:", dateHired.ToString());
        AnsiConsole.Write(grid);
        if (AnsiConsole.Confirm("Save this employee?") == false)
        {
            return;
        }

        var employee = new Employee()
        {
            FirstName = firstName,
            LastName = lastName,
            PersonalIdentityNumber = pin,
            Position = selectedPosition,
            Faculty = faculty,
            Salary = salary,
            HireDate = dateHired
        };

        var success = MenuService?.EmployeeService.AddEmployee(employee);
        AnsiConsole.MarkupLine(
            success is not null && (bool)success
                ? $"[green]Employee {employee.FirstName} {employee.LastName} added[/]"
                : "[red]Failed to add employee[/]"
        );
    }
}
