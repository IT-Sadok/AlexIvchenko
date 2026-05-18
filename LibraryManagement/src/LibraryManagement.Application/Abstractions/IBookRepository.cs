using LibraryManagement.Domain.Entities;

namespace LibraryManagement.Application.Abstractions;

public interface IBookRepository
{
    Task<IReadOnlyCollection<Book>> GetAllAsync();
    Task<Book?> GetByCodeAsync(string code);
    Task AddAsync(Book book);
    Task UpdateAsync(Book book);
    Task<bool> ExistsByCodeAsync(string code);
}
