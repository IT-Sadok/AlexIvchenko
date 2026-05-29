using FluentAssertions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;

namespace LibraryManagement.Tests;

public class BookTests
{
    [Fact]
    public void Borrow_WhenBookIsAvailable_ShouldChangeStatusToBorrowed()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        // Act
        book.Borrow();

        // Assert
        book.Status.Should().Be(BookStatus.Borrowed);
        book.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Borrow_WhenBookIsAlreadyBorrowed_ShouldThrowException()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");
        book.Borrow();

        // Act
        Action action = () => book.Borrow();

        // Assert
        action.Should().Throw<BookStateException>();
    }

    [Fact]
    public void Return_WhenBookIsBorrowed_ShouldChangeStatusToAvailable()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");
        book.Borrow();

        // Act
        book.Return();

        // Assert
        book.Status.Should().Be(BookStatus.Available);
        book.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void Return_WhenBookIsAlreadyAvailable_ShouldThrowException()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        // Act
        Action action = () => book.Return();

        // Assert
        action.Should().Throw<BookStateException>();
    }

    [Fact]
    public void UpdateDetails_WhenValidData_ShouldUpdateBook()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        // Act
        book.UpdateDetails("The Clean Coder", "Robert C. Martin", 2011);

        // Assert
        book.Title.Should().Be("The Clean Coder");
        book.Author.Should().Be("Robert C. Martin");
        book.Year.Should().Be(2011);
        book.Code.Should().Be("BK-001");
        book.UpdatedAt.Should().NotBeNull();
    }

    [Fact]
    public void MarkAsDeleted_WhenBookIsNotDeleted_ShouldSoftDeleteBook()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        // Act
        book.MarkAsDeleted();

        // Assert
        book.IsDeleted.Should().BeTrue();
        book.DeletedAt.Should().NotBeNull();
        book.UpdatedAt.Should().NotBeNull();
    }
}
