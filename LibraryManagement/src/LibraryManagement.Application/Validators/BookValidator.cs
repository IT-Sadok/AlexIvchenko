using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Domain.Common;

namespace LibraryManagement.Application.Validators;

public class BookValidator : IBookValidator
{
    private const int MinTextLength = 2;
    private readonly IBookRepository _bookRepository;

    public BookValidator(IBookRepository bookRepository)
    {
        _bookRepository = bookRepository;
    }
    public void ValidateCode(string code)
    {
        Guard.RequiredText(code, nameof(code), "Book code");
    }

    public async Task ValidateCreateAsync(CreateBookRequest request)
    {
        Guard.RequiredText(request.Title, nameof(request.Title), "Book title", MinTextLength);
        Guard.RequiredText(request.Author, nameof(request.Author), "Book author", MinTextLength);
        Guard.YearNotInFuture(request.Year, nameof(request.Year), "Book year");
        string code = Guard.RequiredText(request.Code, nameof(request.Code), "Book code");

        bool codeExists = await _bookRepository.ExistsByCodeAsync(code);

        if (codeExists)
        {
            throw new ArgumentException("Book code must be unique.", nameof(request.Code));
        }
    }

    public void ValidateSearchTerm(string searchTerm)
    {
        Guard.RequiredText(searchTerm, nameof(searchTerm), "Search term");
    }

    public void ValidateUpdate(UpdateBookRequest request)
    {
        Guard.RequiredText(request.Title, nameof(request.Title), "Book title", MinTextLength);
        Guard.RequiredText(request.Author, nameof(request.Author), "Book author", MinTextLength);
        Guard.YearNotInFuture(request.Year, nameof(request.Year), "Book year");
    }
}
