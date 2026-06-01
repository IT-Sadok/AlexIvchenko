using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Abstractions;

public interface IBookRepository
{
    Task<IReadOnlyCollection<Book>> GetAllAsync(CancellationToken cancellationToken = default);
    Task<Book?> GetByCodeAsync(string code, CancellationToken cancellationToken = default);
    Task AddAsync(Book book, CancellationToken cancellationToken = default);
    Task<Book> UpdateByCodeAsync(string code, Action<Book> updateAction, CancellationToken cancellationToken = default);
    Task<bool> ExistsByCodeAsync(string code, CancellationToken cancellationToken = default);
}
