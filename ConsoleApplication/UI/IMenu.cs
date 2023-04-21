namespace ConsoleApplication.UI;

public interface IMenu
{
    public MenuService? MenuService { set; }
    public string Name { get; init; }
    public void Show();
}
