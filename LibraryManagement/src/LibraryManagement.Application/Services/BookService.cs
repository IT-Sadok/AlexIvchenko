using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.Common;
using LibraryManagement.Application.Models;
using LibraryManagement.Application.Validators;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using System;

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
    public async Task<Result<BookModel>> AddAsync(CreateBookRequest request)
    {
        try
        {
            await _bookValidator.ValidateCreateAsync(request);

            var book = new Book(
                request.Title,
                request.Author,
                request.Year,
                request.Code);

            await _bookRepository.AddAsync(book);

            _logger.LogInformation("Book added. Code: {BookCode}, Title: {BookTitle}", book.Code, book.Title);

            return Result<BookModel>.Success(MapToModel(book));
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to add book. Error: {Error}", ex.Message);
            return Result<BookModel>.Failure(ex.Message);
        }
    }

    public async Task<Result<BookModel>> UpdateAsync(string code, UpdateBookRequest request)
    {
        try
        {
            _bookValidator.ValidateCode(code);
            _bookValidator.ValidateUpdate(request);

            var book = await _bookRepository.UpdateByCodeAsync(code, 
                            book => book.UpdateDetails(
                                request.Title,
                                request.Author,
                                request.Year));

            _logger.LogInformation("Book updated. Code: {BookCode}, Title: {BookTitle}", book.Code, book.Title);

            return Result<BookModel>.Success(MapToModel(book));
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to update book. Error: {Error}", ex.Message);
            return Result<BookModel>.Failure(ex.Message);
        }
    }

    public async Task<Result> BorrowAsync(string code)
    {
        try
        {
            _bookValidator.ValidateCode(code);

            await _bookRepository.UpdateByCodeAsync(code, book => book.Borrow());

            _logger.LogInformation("Book borrowed. Code: {BookCode}", code);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to borrow book. Error: {Error}", ex.Message);
            return Result.Failure(ex.Message);
        }

    }

    public async Task<Result> DeleteAsync(string code)
    {
        try
        {
            _bookValidator.ValidateCode(code);

            await _bookRepository.UpdateByCodeAsync(code, book => book.MarkAsDeleted());

            _logger.LogInformation("Book deleted. Code: {BookCode}", code);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to delete book. Error: {Error}", ex.Message);
            return Result.Failure(ex.Message);
        }

    }

    public async Task<Result<IReadOnlyCollection<BookModel>>> GetAllAsync()
    {
        try
        {
            var books = await _bookRepository.GetAllAsync();

            var result = books
                .Where(book => !book.IsDeleted)
                .OrderBy(book => book.Title)
                .Select(MapToModel)
                .ToList();

            return Result<IReadOnlyCollection<BookModel>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get all books.");
            return Result<IReadOnlyCollection<BookModel>>.Failure("Failed to get all books.");
        }
    }

    public async Task<Result<IReadOnlyCollection<BookModel>>> GetAvailableAsync()
    {
        try
        {
            var books = await _bookRepository.GetAllAsync();

            var result = books
                .Where(book => !book.IsDeleted)
                .Where(book => book.Status == BookStatus.Available)
                .OrderBy(book => book.Title)
                .Select(MapToModel)
                .ToList();

            return Result<IReadOnlyCollection<BookModel>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Failed to get available books.");
            return Result<IReadOnlyCollection<BookModel>>.Failure("Failed to get available books.");
        }
    }

    public async Task<Result> ReturnAsync(string code)
    {
        try
        {
            _bookValidator.ValidateCode(code);

            await _bookRepository.UpdateByCodeAsync(code, book => book.Return());

            _logger.LogInformation("Book returned. Code: {BookCode}", code);

            return Result.Success();
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to return book. Error: {Error}", ex.Message);
            return Result.Failure(ex.Message);
        }
    }

    public async Task<Result<IReadOnlyCollection<BookModel>>> SearchAsync(string searchTerm)
    {
        try
        {
            _bookValidator.ValidateSearchTerm(searchTerm);

            string normalizedSearchTerm = searchTerm.Trim();

            var books = await _bookRepository.GetAllAsync();

            var result = books
                .Where(book => !book.IsDeleted)
                .Where(book =>
                    book.Title.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Author.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase) ||
                    book.Code.Contains(normalizedSearchTerm, StringComparison.OrdinalIgnoreCase))
                .OrderBy(book => book.Title)
                .Select(MapToModel)
                .ToList();

            return Result<IReadOnlyCollection<BookModel>>.Success(result);
        }
        catch (Exception ex)
        {
            _logger.LogWarning("Failed to search books. Error: {Error}", ex.Message);
            return Result<IReadOnlyCollection<BookModel>>.Failure(ex.Message);
        }
        
    }

    private static BookModel MapToModel(Book book)
    {
        return new BookModel
        {
            Id = book.Id,
            Title = book.Title,
            Author = book.Author,
            Year = book.Year,
            Code = book.Code,
            Status = book.Status
        };
    }
}
