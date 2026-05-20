namespace LibraryManagement.Application.Models;

public class CreateBookRequest
{
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public int Year { get; init; }
    public string Code { get; init; } = string.Empty;
}
