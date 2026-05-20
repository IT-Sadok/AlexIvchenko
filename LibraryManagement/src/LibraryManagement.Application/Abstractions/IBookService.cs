using LibraryManagement.Application.Common;
using LibraryManagement.Application.Models;

namespace LibraryManagement.Application.Abstractions;

public interface IBookService
{
    Task<Result<BookModel>> AddAsync(CreateBookRequest request);
    Task<Result<BookModel>> UpdateAsync(string code, UpdateBookRequest request);
    Task<Result> DeleteAsync(string code);
    Task<Result<IReadOnlyCollection<BookModel>>> GetAllAsync();
    Task<Result<IReadOnlyCollection<BookModel>>> GetAvailableAsync();
    Task<Result<IReadOnlyCollection<BookModel>>> SearchAsync(string searchTerm);
    Task<Result> BorrowAsync(string code);
    Task<Result> ReturnAsync(string code);
}
