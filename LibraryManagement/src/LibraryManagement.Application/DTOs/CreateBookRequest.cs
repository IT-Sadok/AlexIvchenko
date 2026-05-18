namespace LibraryManagement.Application.DTOs;

public class CreateBookRequest
{
    public string Title { get; set; } = string.Empty;
    public string Author { get; set; } = string.Empty;
    public int Year { get; set; }
    public string Code { get; set; } = string.Empty;
}
