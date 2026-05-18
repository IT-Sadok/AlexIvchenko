using System.Text.Json;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Configuration;
using Microsoft.Extensions.Options;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Infrastructure.Persistence;

public class JsonFileContext
{
    private readonly string _filePath;
    private readonly ILogger<JsonFileContext> _logger;

    private List<Book>? _cachedBooks;

    private readonly JsonSerializerOptions _jsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public JsonFileContext(IOptions<LibraryStorageOptions> options, ILogger<JsonFileContext> logger)
    {
        _filePath = options.Value.FilePath;
        _logger = logger;

        EnsureFileExists();
    }

    public async Task<List<Book>> ReadBooksAsync()
    {
        if (_cachedBooks is not null)
        {
            _logger.LogDebug("Books loaded from cache.");
            return _cachedBooks;
        }

        _cachedBooks = await ReadBooksFromFileAsync();

        return _cachedBooks;
    }

    public async Task WriteBooksAsync(List<Book> books)
    {
        try
        {
            EnsureFileExists();

            string json = JsonSerializer.Serialize(books, _jsonOptions);

            await File.WriteAllTextAsync(_filePath, json);

            _cachedBooks = books;
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogError(exception, "Access denied while writing storage file. FilePath: {FilePath}", _filePath);
            throw;
        }
        catch (IOException exception)
        {
            _logger.LogError(exception, "I/O error while writing storage file. FilePath: {FilePath}", _filePath);
            throw;
        }
    }

    private void EnsureFileExists()
    {
        string? directoryPath = Path.GetDirectoryName(_filePath);

        if (!string.IsNullOrWhiteSpace(directoryPath) && !Directory.Exists(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        if (!File.Exists(_filePath))
        {
            File.WriteAllText(_filePath, "[]");
        }
    }

    // For future/testing porpuse
    public void ClearCache()
    {
        _cachedBooks = null;
    }

    private async Task<List<Book>> ReadBooksFromFileAsync()
    {
        try
        {
            EnsureFileExists();

            string json = await File.ReadAllTextAsync(_filePath);

            if (string.IsNullOrWhiteSpace(json))
            {
                return new List<Book>();
            }

            return JsonSerializer.Deserialize<List<Book>>(json, _jsonOptions)
                   ?? new List<Book>();
        }
        catch (JsonException exception)
        {
            _logger.LogError(exception, "JSON storage file is corrupted. FilePath: {FilePath}", _filePath);

            throw new InvalidOperationException(
                "Something went wrong while reading storage file. JSON file is corrupted.",
                exception);
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogError(exception, "Access denied while reading storage file. FilePath: {FilePath}", _filePath);
            throw;
        }
        catch (IOException exception)
        {
            _logger.LogError(exception, "I/O error while reading storage file. FilePath: {FilePath}", _filePath);
            throw;
        }
    }
}
