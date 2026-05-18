namespace LibraryManagement.Domain.Exceptions;

public class BookAlreadyAvailableException : Exception
{
    public BookAlreadyAvailableException(string code)
        : base($"Book with code '{code}' is already available.")
    {
        Code = code;
    }

    public string Code { get; }
}
