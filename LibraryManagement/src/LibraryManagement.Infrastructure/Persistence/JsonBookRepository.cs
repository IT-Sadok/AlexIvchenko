using LibraryManagement.Application.Abstractions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Exceptions;

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
        var books = await _jsonFileContext.ReadBooksSnapshotAsync();

        return books
            .Where(book => !book.IsDeleted)
            .ToList();
    }

    public async Task<Book?> GetByCodeAsync(string code)
    {
        var books = await _jsonFileContext.ReadBooksSnapshotAsync();

        return books.FirstOrDefault(book =>
            !book.IsDeleted &&
            book.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
    }

    public async Task AddAsync(Book book)
    {
        await _jsonFileContext.UpdateBooksAsync(books =>
        {
            bool codeExists = books.Any(existingBook =>
                !existingBook.IsDeleted &&
                existingBook.Code.Equals(book.Code, StringComparison.OrdinalIgnoreCase));

            if (codeExists)
            {
                throw new ArgumentException("Book code must be unique.", nameof(book.Code));
            }

            books.Add(book);

            return true;
        });
    }

    public async Task<Book> UpdateByCodeAsync(string code, Action<Book> updateAction)
    {
        return await _jsonFileContext.UpdateBooksAsync(books =>
        {
            string normalizedCode = code.Trim();

            var book = books.FirstOrDefault(existingBook =>
                !existingBook.IsDeleted &&
                existingBook.Code.Equals(normalizedCode, StringComparison.OrdinalIgnoreCase));

            if (book is null)
            {
                throw new BookNotFoundException(normalizedCode);
            }

            updateAction(book);

            return book;
        });
    }

    public async Task<bool> ExistsByCodeAsync(string code)
    {
        var books = await _jsonFileContext.ReadBooksSnapshotAsync();

        return books.Any(book =>
            !book.IsDeleted &&
            book.Code.Equals(code.Trim(), StringComparison.OrdinalIgnoreCase));
    }
}