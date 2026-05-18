using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Application.DTOs;

public class BookDto
{
    public Guid Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Code { get; set; } = string.Empty;
    public BookStatus Status { get; set; }
}
