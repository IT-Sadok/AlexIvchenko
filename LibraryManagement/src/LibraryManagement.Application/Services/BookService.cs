using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Application.Validators;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Application.Services;

public class BookService : IBookService
{
    private readonly IBookRepository _bookRepository;
    private readonly IBookValidator _bookValidator;
    private readonly ILogger _logger;

    public BookService(
        IBookRepository bookRepository,
        IBookValidator bookValidator,
        ILogger<BookService> logger)
    {
        _bookRepository = bookRepository;
        _bookValidator = bookValidator;
        _logger = logger;
    }
    public async Task<BookDto> AddAsync(CreateBookRequest request)
    {
        await _bookValidator.ValidateCreateAsync(request);

        var book = new Book(
            request.Title,
            request.Author,
            request.Year,
            request.Code);

        await _bookRepository.AddAsync(book);

        _logger.LogInformation("Book added. Code: {BookCode}, Title: {BookTitle}", book.Code, book.Title);

        return MapToDto(book);
    }

    public async Task BorrowAsync(string code)
    {
        _bookValidator.ValidateCode(code);

        var book = await GetBookOrThrowAsync(code);

        book.Borrow();

        await _bookRepository.UpdateAsync(book);

        _logger.LogInformation("Book borrowed. Code: {BookCode}", book.Code);
    }

    public async Task DeleteAsync(string code)
    {
        _bookValidator.ValidateCode(code);

        var book = await GetBookOrThrowAsync(code);

        book.MarkAsDeleted();

        await _bookRepository.UpdateAsync(book);

        _logger.LogInformation("Book deleted. Code: {BookCode}", book.Code);
    }

    public async Task<IReadOnlyCollection<BookDto>> GetAllAsync()
    {
        var books = await _bookRepository.GetAllAsync();

        return books
            .Where(book => !book.IsDeleted)
            .OrderBy(book => book.Title)
            .Select(MapToDto)
            .ToList();
    }

    public async Task<IReadOnlyCollection<BookDto>> GetAvailableAsync()
    {
        var books = await _bookRepository.GetAllAsync();

        return books
            .Where(book => !book.IsDeleted)
            .Where(book => book.Status == BookStatus.Available)
            .OrderBy(book => book.Title)
            .Select(MapToDto)
            .ToList();
    }

    public async Task ReturnAsync(string code)
    {
        _bookValidator.ValidateCode(code);

        var book = await GetBookOrThrowAsync(code);

        book.Return();

        await _bookRepository.UpdateAsync(book);

        _logger.LogInformation("Book returned. Code: {BookCode}", book.Code);
    }

    public async Task<IReadOnlyCollection<BookDto>> SearchAsync(string searchTerm)
    {
        _bookValidator.ValidateSearchTerm(searchTerm);

        string normalizedSearchTerm = searchTerm.Trim();

        var books = await _bookRepository.GetAllAsync();

        return books
            .Where(book => !book.IsDeleted)
            .Where(book =>
                book.Title.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                book.Author.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                book.Code.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase))
            .OrderBy(book => book.Title)
            .Select(MapToDto)
            .ToList();
    }

    private async Task<Book> GetBookOrThrowAsync(string code)
    {
        string normalizedCode = code.Trim();

        var book = await _bookRepository.GetByCodeAsync(normalizedCode);

        if (book is null || book.IsDeleted)
        {
            throw new BookNotFoundException(normalizedCode);
        }

        return book;
    }

    private static BookDto MapToDto(Book book)
    {
        return new BookDto
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Year = book.Year,
            Code = book.Code,
            Status = book.Status
        };
    }

    public async Task<BookDto> UpdateAsync(string code, UpdateBookRequest request)
    {
        _bookValidator.ValidateCode(code);
        _bookValidator.ValidateUpdate(request);

        var book = await GetBookOrThrowAsync(code);

        book.UpdateDetails(
            request.Title,
            request.Author,
            request.Year);

        await _bookRepository.UpdateAsync(book);

        _logger.LogInformation("Book updated. Code: {BookCode}, Title: {BookTitle}", book.Code, book.Title);

        return MapToDto(book);
    }
}
