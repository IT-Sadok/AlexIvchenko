using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.Models;

public class BookSearchRequest
{
    public string? SearchTerm { get; set; }
    public BookStatus? Status { get; set; }
}
