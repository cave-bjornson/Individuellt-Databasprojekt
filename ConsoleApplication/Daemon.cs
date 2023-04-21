using ConsoleApplication.UI;
using Microsoft.Extensions.Configuration;
using Spectre.Console;

namespace ConsoleApplication;

public class Daemon : ConsoleAppBase
{
    private readonly IConfiguration _configuration;
    private readonly MenuService _menuService;

    public Daemon(IConfiguration configuration, MenuService menuService)
    {
        _configuration = configuration;
        _menuService = menuService;
    }

    [Command("run-main")]
    public async Task Run()
    {
        // _menuService.GetMenus().ForEach(menu => Console.WriteLine(menu.Name));

        AnsiConsole.MarkupLineInterpolated(
            $"[yellow]{_configuration.GetConnectionString("HighSchoolDB")}[/]"
        );
        try
        {
            while (!this.Context.CancellationToken.IsCancellationRequested)
            {
                try
                {
                    AnsiConsole.WriteLine("Welcome To Sunnydale Highschool!");
                }
                catch (Exception ex)
                {
                    Console.WriteLine("Exception: {0}", ex);
                }

                await Task.Run(() =>
                    {
                        _menuService.Navigate(null, _menuService.GetMenu("MainMenu"));
                        bool quit = AnsiConsole.Confirm("Really Quit?");
                        if (quit)
                        {
                            this.Context.Cancel();
                        }
                    })
                    .WaitAsync(this.Context.CancellationToken);
            }
        }
        catch (Exception ex) when (ex is not OperationCanceledException)
        {
            Console.WriteLine("OperationCanceledException: {0}", ex);
        }
    }
}
