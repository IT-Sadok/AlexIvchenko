using LibraryManagement.Domain.Enums;

namespace LibraryManagement.Domain.Exceptions;

public class BookStateException : Exception
{
    public BookStateException(
        string code,
        BookStatus status,
        string operation,
        string message)
        : base(message)
    {
        Code = code;
        Status = status;
        Operation = operation;
    }

    public string Code { get; }

    public BookStatus Status { get; }

    public string Operation { get; }
}
