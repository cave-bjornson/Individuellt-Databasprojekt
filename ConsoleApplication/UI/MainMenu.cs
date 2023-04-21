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
        var menuSelection = AnsiConsole.Prompt(
            new SelectionPrompt<IMenu>()
                .Title(Name)
                .AddChoices(MenuService?.GetMenus("SingleStudentInfoMenu", "BackMenu")!)
                .UseConverter(menu => menu is null ? "Back" : menu.Name)
        );
        if (menuSelection is null)
        {
            return;
        }
        MenuService.Navigate(this, menuSelection);
    }
}
