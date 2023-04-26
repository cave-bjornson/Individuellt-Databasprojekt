using ConsoleApplication.Services;

namespace ConsoleApplication.UI;

public class MenuService
{
    private readonly Dictionary<string, IMenu?> _menuDict = new();

    private readonly Stack<IMenu?> _menuStack = new();

    public EmployeeService EmployeeService { get; }

    public StudentService StudentService { get; }

    public AdminService AdminService { get; }

    public MenuService(
        EmployeeService employeeService,
        StudentService studentService,
        AdminService adminService
    )
    {
        EmployeeService = employeeService;
        StudentService = studentService;
        AdminService = adminService;

        AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(x => x.GetTypes())
            .Where(x => x.GetInterfaces().Contains(typeof(IMenu)))
            .ToList()
            .ForEach(x =>
            {
                var menuInstance =
                    Activator.CreateInstance(x) as IMenu ?? throw new InvalidOperationException();
                menuInstance.MenuService = this;
                _menuDict.Add(x.Name.ToLower(), menuInstance);
            });
        _menuDict.Add("backmenu", null);
    }

    /// <summary>
    /// Navigate to a menu if it is not null, otherwise navigate back
    /// </summary>
    /// <param name="from">
    /// The menu to navigate from
    /// </param>
    /// <param name="to">
    /// The menu to navigate to
    /// </param>
    public void Navigate(IMenu? from, IMenu? to)
    {
        _menuStack.Push(from);
        to?.Show();
        _menuStack.Pop()?.Show();
    }

    /// <summary>
    /// Get a menu by name
    /// </summary>
    /// <param name="menuName">
    /// The name of the menu
    /// </param>
    /// <returns>
    /// The menu with the given name
    /// </returns>
    public IMenu? GetMenu(string menuName)
    {
        return _menuDict[menuName.ToLower()];
    }

    /// <summary>
    /// Get a list of menus by name
    /// </summary>
    /// <param name="menuNames">
    /// The names of the menus
    /// </param>
    /// <returns>
    /// A list of menus with the given names
    /// </returns>
    public List<IMenu?> GetMenus(params string[] menuNames)
    {
        return menuNames.Select(GetMenu).ToList();
    }

    /// <summary>
    /// Get all menus
    /// </summary>
    /// <returns>
    /// A list of all menus
    /// </returns>
    public List<IMenu?> GetMenus()
    {
        return _menuDict.Values.ToList();
    }
}
