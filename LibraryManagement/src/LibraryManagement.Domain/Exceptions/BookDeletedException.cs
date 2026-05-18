namespace LibraryManagement.Domain.Exceptions;

public class BookDeletedException : Exception
{
    public BookDeletedException(string code)
        : base($"Book with code '{code}' is deleted and cannot be modified.")
    {
        Code = code;
    }

    public string Code { get; }
}
