using ConsoleApplication;
using ConsoleApplication.Data;
using ConsoleApplication.Services;
using ConsoleApplication.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder();
hostBuilder.ConfigureServices(services =>
{
    services.AddDbContext<HighSchoolDBContext>();
    services.AddSingleton<StudentService>();
    services.AddSingleton<AdminService>();
    services.AddSingleton<StudentService>();
    services.AddSingleton<EmployeeService>();
    services.AddSingleton<MenuService>();
});

var app = ConsoleApp.CreateFromHostBuilder(hostBuilder, args);
app.AddCommands<Daemon>();

app.AddSubCommands<TestRunner>();
app.Run();
