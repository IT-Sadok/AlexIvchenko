using LibraryManagement.Domain.Common;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using System.Text.Json.Serialization;

namespace LibraryManagement.Domain.Entities;

public class Book : BaseEntity
{
    private const int MinTextLength = 2;

    [JsonInclude]
    public string Title { get; private set; }
    [JsonInclude]
    public string Author { get; private set; }
    [JsonInclude]
    public int Year { get; private set; }
    [JsonInclude]
    public string Code { get; private set; }
    [JsonInclude]
    public BookStatus Status { get; private set; }

    private Book()
    {
        Title = string.Empty;
        Author = string.Empty;
        Code = string.Empty;
    }

    public Book(string title, string author, int year, string code)
    {
        Title = Guard.RequiredText(title, nameof(title), "Book title", MinTextLength);
        Author = Guard.RequiredText(author, nameof(author), "Book author", MinTextLength);
        Year = Guard.YearNotInFuture(year, nameof(year), "Book year");
        Code = Guard.RequiredText(code, nameof(code), "Book code");
        Status = BookStatus.Available;
    }

    public void Borrow()
    {
        EnsureNotDeleted();

        if (Status == BookStatus.Borrowed)
        {
            throw new BookStateException(
                        Code,
                        Status,
                        "Borrow",
                        $"Book with code '{Code}' cannot be borrowed because it is already borrowed.");
        }

        Status = BookStatus.Borrowed;
        SetUpdatedAt();
    }

    public void Return()
    {
        EnsureNotDeleted();

        if (Status == BookStatus.Available)
        {
            throw new BookStateException(
                        Code,
                        Status,
                        "Return",
                        $"Book with code '{Code}' cannot be returned because it is already available.");
        }

        Status = BookStatus.Available;
        SetUpdatedAt();
    }

    public void UpdateDetails(string title, string author, int year)
    {
        EnsureNotDeleted();

        Title = Guard.RequiredText(title, nameof(title), "Book title", MinTextLength);
        Author = Guard.RequiredText(author, nameof(author), "Book author", MinTextLength);
        Year = Guard.YearNotInFuture(year, nameof(year), "Book year");

        SetUpdatedAt();
    }

    public void MarkAsDeleted() => SetDeletedAt();

    private void EnsureNotDeleted()
    {
        if (IsDeleted)
            throw new BookStateException(
                        Code,
                        Status,
                        "Modify",
                        $"Book with code '{Code}' is deleted and cannot be modified.");
    }
}
