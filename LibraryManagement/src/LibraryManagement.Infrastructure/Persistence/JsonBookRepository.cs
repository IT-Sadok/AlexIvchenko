using LibraryManagement.Application.Abstractions;
using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Infrastructure.Persistence;

public class JsonBookRepository : IBookRepository
{
    private readonly JsonFileContext _jsonFileContext;

    public JsonBookRepository(JsonFileContext jsonFileContext)
    {
        _jsonFileContext = jsonFileContext;
    }

    public async Task<IReadOnlyCollection<Book>> GetAllAsync()
    {
        var books = await _jsonFileContext.ReadBooksAsync();

        return books
            .Where(book => !book.IsDeleted)
            .ToList();
    }

    public async Task<Book?> GetByCodeAsync(string code)
    {
        var books = await _jsonFileContext.ReadBooksAsync();

        return books.FirstOrDefault(book =>
            !book.IsDeleted &&
            book.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAsync(Book book)
    {
        var books = await _jsonFileContext.ReadBooksAsync();

        books.Add(book);

        await _jsonFileContext.WriteBooksAsync(books);
    }

    public async Task UpdateAsync(Book book)
    {
        var books = await _jsonFileContext.ReadBooksAsync();

        int index = books.FindIndex(existingBook =>
            existingBook.Id == book.Id ||
            existingBook.Code.Equals(book.Code, StringComparison.OrdinalIgnoreCase));

        if (index < 0)
        {
            books.Add(book);
        }
        else
        {
            books[index] = book;
        }

        await _jsonFileContext.WriteBooksAsync(books);
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        var books = await _jsonFileContext.ReadBooksAsync();

        return books.Any(book =>
            !book.IsDeleted &&
            book.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}