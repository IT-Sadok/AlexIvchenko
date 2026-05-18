using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Validators;
using LibraryManagement.Console.Input;
using LibraryManagement.Console.Menus;
using LibraryManagement.Console.Output;
using LibraryManagement.Infrastructure.DependencyInjection;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Console;

public static class DependencyInjection
{
    public static ServiceProvider ConfigureServices()
    {
        IConfiguration configuration = new ConfigurationBuilder()
            .SetBasePath(AppContext.BaseDirectory)
            .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
            .Build();

        var services = new ServiceCollection();

        services.AddLogging(builder =>
        {
            builder.AddConsole();
        });

        services.AddInfrastructure(configuration);

        services.AddScoped<IBookService, BookService>();
        services.AddScoped<IBookValidator, BookValidator>();

        services.AddSingleton<ConsoleInputReader>();
        services.AddSingleton<ConsoleWriter>();
        services.AddScoped<MainMenu>();

        return services.BuildServiceProvider();
    }
}