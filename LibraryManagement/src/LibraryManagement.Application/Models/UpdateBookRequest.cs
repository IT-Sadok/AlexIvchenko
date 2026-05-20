namespace LibraryManagement.Application.Models;

public class UpdateBookRequest
{
    public string Title { get; init; } = string.Empty;
    public string Author { get; init; } = string.Empty;
    public int Year { get; init; }
}
