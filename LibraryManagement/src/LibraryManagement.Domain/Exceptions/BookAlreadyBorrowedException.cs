namespace LibraryManagement.Domain.Exceptions;

public class BookAlreadyBorrowedException : Exception
{
    public BookAlreadyBorrowedException(string code)
        : base($"Book with code '{code}' is already borrowed.")
    {
        Code = code;
    }

    public string Code { get; }
}
