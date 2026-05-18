using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Application.Validators;

public interface IBookValidator
{
    Task ValidateCreateAsync(CreateBookRequest request);
    void ValidateUpdate(UpdateBookRequest request);
    void ValidateSearchTerm(string searchTerm);
    void ValidateCode(string code);
}
