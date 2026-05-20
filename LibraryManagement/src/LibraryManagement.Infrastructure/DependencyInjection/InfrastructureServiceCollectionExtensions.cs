using LibraryManagement.Application.Abstractions;
using LibraryManagement.Infrastructure.Configuration;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace LibraryManagement.Infrastructure.DependencyInjection;

public static class InfrastructureServiceCollectionExtensions
{
    public static IServiceCollection AddInfrastructure(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.Configure<LibraryStorageOptions>(
            configuration.GetSection(LibraryStorageOptions.SectionName));

        services.AddSingleton<JsonFileContext>();
        services.AddScoped<IBookRepository, JsonBookRepository>();

        return services;
    }
}