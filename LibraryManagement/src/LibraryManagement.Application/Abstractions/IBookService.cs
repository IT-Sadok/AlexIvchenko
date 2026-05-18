using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Abstractions;

public interface IBookService
{
    Task<BookDto> AddAsync(CreateBookRequest request);
    Task<BookDto> UpdateAsync(string code, UpdateBookRequest request);
    Task DeleteAsync(string code);
    Task<IReadOnlyCollection<BookDto>> GetAllAsync();
    Task<IReadOnlyCollection<BookDto>> GetAvailableAsync();
    Task<IReadOnlyCollection<BookDto>> SearchAsync(string searchTerm);
    Task BorrowAsync(string code);
    Task ReturnAsync(string code);
}
