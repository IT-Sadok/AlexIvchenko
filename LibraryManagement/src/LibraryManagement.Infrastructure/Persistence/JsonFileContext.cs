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
    private readonly SemaphoreSlim _semaphore = new(1, 1);

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

    public async Task<IReadOnlyCollection<Book>> ReadBooksSnapshotAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (_cachedBooks is null)
            {
                _logger.LogInformation(
                    "Books loaded from JSON file. FilePath: {FilePath}",
                    _filePath);

                _cachedBooks = await ReadBooksFromFileAsync(cancellationToken);
            }
            else
            {
                _logger.LogDebug("Books loaded from cache.");
            }

            return CloneBooks(_cachedBooks);
        }
        finally
        {
            _semaphore.Release();
        }
    }

    public async Task<TResult> UpdateBooksAsync<TResult>(Func<List<Book>, TResult> updateAction
        , CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);

        try
        {
            if (_cachedBooks is null)
            {
                _cachedBooks = await ReadBooksFromFileAsync(cancellationToken);
            }

            TResult result = updateAction(_cachedBooks);
            await WriteBooksToFileAsync(_cachedBooks, cancellationToken);
            _logger.LogDebug("Books were updated.");

            return result;
        }
        finally { _semaphore.Release(); }
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

    private async Task<List<Book>> ReadBooksFromFileAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            EnsureFileExists();

            string json = await File.ReadAllTextAsync(_filePath, cancellationToken);

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

    private async Task WriteBooksToFileAsync(List<Book> books, CancellationToken cancellationToken = default)
    {
        try
        {
            EnsureFileExists();

            string json = JsonSerializer.Serialize(books, _jsonOptions);

            await File.WriteAllTextAsync(_filePath, json, cancellationToken);
        }
        catch (UnauthorizedAccessException exception)
        {
            _logger.LogError(
                exception,
                "Access denied while writing storage file. FilePath: {FilePath}",
                _filePath);

            throw;
        }
        catch (IOException exception)
        {
            _logger.LogError(
                exception,
                "I/O error while writing storage file. FilePath: {FilePath}",
                _filePath);

            throw;
        }
    }

    private List<Book> CloneBooks(List<Book> books)
    {
        string json = JsonSerializer.Serialize(books, _jsonOptions);

        return JsonSerializer.Deserialize<List<Book>>(json, _jsonOptions)
               ?? new List<Book>();
    }
}
