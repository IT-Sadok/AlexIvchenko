using LibraryManagement.Application.Common;
using LibraryManagement.Application.Models;

namespace LibraryManagement.Application.Abstractions;

public interface IBookService
{
    Task<Result<BookModel>> AddAsync(CreateBookRequest request, CancellationToken cancellationToken = default);
    Task<Result<BookModel>> UpdateAsync(string code, UpdateBookRequest request, CancellationToken cancellationToken = default);
    Task<Result> DeleteAsync(string code, CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<BookModel>>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<BookModel>>> GetAvailableAsync(CancellationToken cancellationToken = default);
    Task<Result<IReadOnlyCollection<BookModel>>> SearchAsync(string searchTerm, CancellationToken cancellationToken = default);
    Task<Result> BorrowAsync(string code, CancellationToken cancellationToken = default);
    Task<Result> ReturnAsync(string code, CancellationToken cancellationToken = default);
}
