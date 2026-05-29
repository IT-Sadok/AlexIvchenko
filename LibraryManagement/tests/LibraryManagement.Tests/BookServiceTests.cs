using FluentAssertions;
using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.Models;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Validators;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Domain.Enums;
using Microsoft.Extensions.Logging;
using Moq;

namespace LibraryManagement.Tests;

public class BookServiceTests
{
    private readonly Mock<IBookRepository> _bookRepositoryMock;
    private readonly Mock<ILogger<BookService>> _loggerMock;
    private readonly BookValidator _bookValidator;
    private readonly BookService _bookService;

    public BookServiceTests()
    {
        _bookRepositoryMock = new Mock<IBookRepository>();
        _loggerMock = new Mock<ILogger<BookService>>();

        _bookValidator = new BookValidator(_bookRepositoryMock.Object);

        _bookService = new BookService(
            _bookRepositoryMock.Object,
            _bookValidator,
            _loggerMock.Object);
    }

    [Fact]
    public async Task AddAsync_WhenCodeIsUnique_ShouldAddBook()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            Year = 2008,
            Code = "BK-001"
        };

        _bookRepositoryMock
            .Setup(repository => repository.ExistsByCodeAsync(request.Code))
            .ReturnsAsync(false);

        // Act
        var result = await _bookService.AddAsync(request);

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value!.Title.Should().Be(request.Title);
        result.Value!.Author.Should().Be(request.Author);
        result.Value!.Year.Should().Be(request.Year);
        result.Value!.Code.Should().Be(request.Code);
        result.Value!.Status.Should().Be(BookStatus.Available);

        _bookRepositoryMock.Verify(
            repository => repository.AddAsync(It.Is<Book>(book =>
                book.Title == request.Title &&
                book.Author == request.Author &&
                book.Year == request.Year &&
                book.Code == request.Code)),
            Times.Once);
    }

    [Fact]
    public async Task AddAsync_WhenCodeAlreadyExists_ShouldReturnFailure()
    {
        // Arrange
        var request = new CreateBookRequest
        {
            Title = "Clean Code",
            Author = "Robert Martin",
            Year = 2008,
            Code = "BK-001"
        };

        _bookRepositoryMock
            .Setup(repository => repository.ExistsByCodeAsync(request.Code))
            .ReturnsAsync(true);

        // Act
        var result = await _bookService.AddAsync(request);

        // Assert
        result.IsFailure.Should().BeTrue();
        result.Error.Should().Contain("unique");

        _bookRepositoryMock.Verify(
            repository => repository.AddAsync(It.IsAny<Book>()),
            Times.Never);
    }

    [Fact]
    public async Task GetAvailableAsync_ShouldReturnOnlyAvailableBooks()
    {
        // Arrange
        var availableBook = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        var borrowedBook = new Book("Refactoring", "Martin Fowler", 1999, "BK-002");
        borrowedBook.Borrow();

        var books = new List<Book>
        {
            availableBook,
            borrowedBook
        };

        _bookRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(books);

        // Act
        var result = await _bookService.GetAvailableAsync();

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
        result.Value!.Single().Code.Should().Be("BK-001");
        result.Value!.Single().Status.Should().Be(BookStatus.Available);
    }

    [Fact]
    public async Task SearchAsync_ShouldFindBooksByTitle()
    {
        // Arrange
        var books = new List<Book>
        {
            new("Clean Code", "Robert Martin", 2008, "BK-001"),
            new("Refactoring", "Martin Fowler", 1999, "BK-002")
        };

        _bookRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(books);

        // Act
        var result = await _bookService.SearchAsync("clean");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
        result.Value!.Single().Title.Should().Be("Clean Code");
    }

    [Fact]
    public async Task SearchAsync_ShouldFindBooksByAuthor()
    {
        // Arrange
        var books = new List<Book>
        {
            new("Clean Code", "Robert Martin", 2008, "BK-001"),
            new("Refactoring", "Martin Fowler", 1999, "BK-002")
        };

        _bookRepositoryMock
            .Setup(repository => repository.GetAllAsync())
            .ReturnsAsync(books);

        // Act
        var result = await _bookService.SearchAsync("fowler");

        // Assert
        result.IsSuccess.Should().BeTrue();
        result.Value.Should().NotBeNull();
        result.Value!.Should().HaveCount(1);
        result.Value!.Single().Author.Should().Be("Martin Fowler");
    }

    [Fact]
    public async Task BorrowAsync_WhenBookExists_ShouldBorrowBook()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        _bookRepositoryMock
            .Setup(repository => repository.UpdateByCodeAsync("BK-001", It.IsAny<Action<Book>>()))
            .ReturnsAsync((string code, Action<Book> updateAction) =>
            {
                updateAction(book);
                return book;
            });

        // Act
        await _bookService.BorrowAsync("BK-001");

        // Assert
        book.Status.Should().Be(BookStatus.Borrowed);

        _bookRepositoryMock.Verify(
            repository => repository.UpdateByCodeAsync(
                "BK-001",
                It.IsAny<Action<Book>>()),
            Times.Once);
    }

    [Fact]
    public async Task ReturnAsync_WhenBookExists_ShouldReturnBook()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");
        book.Borrow();

        _bookRepositoryMock
            .Setup(repository => repository.UpdateByCodeAsync("BK-001", It.IsAny<Action<Book>>()))
            .ReturnsAsync((string code, Action<Book> updateAction) =>
            {
                updateAction(book);
                return book;
            });

        // Act
        var result = await _bookService.ReturnAsync("BK-001");

        // Assert
        result.IsSuccess.Should().BeTrue();
        book.Status.Should().Be(BookStatus.Available);

        _bookRepositoryMock.Verify(
            repository => repository.UpdateByCodeAsync(
                "BK-001",
                It.IsAny<Action<Book>>()),
            Times.Once);
    }

    [Fact]
    public async Task DeleteAsync_WhenBookExists_ShouldSoftDeleteBook()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        _bookRepositoryMock
            .Setup(repository => repository.UpdateByCodeAsync("BK-001", It.IsAny<Action<Book>>()))
            .ReturnsAsync((string code, Action<Book> updateAction) =>
            {
                updateAction(book);
                return book;
            });

        // Act
        var result = await _bookService.DeleteAsync("BK-001");

        // Assert
        result.IsSuccess.Should().BeTrue();
        book.IsDeleted.Should().BeTrue();
        book.DeletedAt.Should().NotBeNull();

        _bookRepositoryMock.Verify(
            repository => repository.UpdateByCodeAsync(
                "BK-001",
                It.IsAny<Action<Book>>()),
            Times.Once);
    }
}
