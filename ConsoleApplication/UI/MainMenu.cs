using Spectre.Console;

namespace ConsoleApplication.UI;

public class MainMenu : IMenu
{
    /// <inheritdoc />
    public MenuService? MenuService { private get; set; }

    /// <inheritdoc />
    public string Name { get; init; } = "Main Menu";

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
                        nameof(StudentMenu),
                        nameof(StaffMenu),
                        nameof(FacultyMenu),
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
}
