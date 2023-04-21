using ConsoleApplication;
using ConsoleApplication.Data;
using ConsoleApplication.Models;
using ConsoleApplication.Repositories;
using ConsoleApplication.Services;
using ConsoleApplication.UI;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

var hostBuilder = Host.CreateDefaultBuilder();
hostBuilder.ConfigureServices(services =>
{
    services.AddDbContext<HighSchoolDBContext>();
    //services.AddSingleton<IPersonService<Employee>, EmployeeService>();
    services.AddSingleton<StudentService>();
    //services.AddSingleton<IAdminService, AdminService>();
    services.AddTransient<StudentRepository>();
    services.AddTransient<EmployeeRepository>();
    services.AddTransient<FacultyRepository>();
    services.AddSingleton<MenuService>();
});

var app = ConsoleApp.CreateFromHostBuilder(hostBuilder, args);
app.AddCommands<Daemon>();
app.AddSubCommands<TestRunner>();
app.Run();
