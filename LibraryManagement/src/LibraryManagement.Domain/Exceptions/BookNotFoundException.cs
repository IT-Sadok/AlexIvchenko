namespace LibraryManagement.Domain.Exceptions;

public class BookNotFoundException : Exception
{
    public BookNotFoundException(string code)
        : base($"Book with code '{code}' was not found.")
    {
        Code = code;
    }

    public string Code { get; }
}
