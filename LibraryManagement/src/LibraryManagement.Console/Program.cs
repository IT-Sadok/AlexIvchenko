using LibraryManagement.Console;
using LibraryManagement.Console.Menus;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = DependencyInjection.ConfigureServices();

var menu = serviceProvider.GetRequiredService<MainMenu>();

await menu.RunAsync();
