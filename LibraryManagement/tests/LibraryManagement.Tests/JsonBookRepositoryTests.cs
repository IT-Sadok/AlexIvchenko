using FluentAssertions;
using LibraryManagement.Domain.Entities;
using LibraryManagement.Infrastructure.Configuration;
using LibraryManagement.Infrastructure.Persistence;
using LibraryManagement.Domain.Enums;
using LibraryManagement.Domain.Exceptions;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Moq;

namespace LibraryManagement.Tests;

public class JsonBookRepositoryTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly string _testFilePath;
    private readonly JsonBookRepository _repository;

    public JsonBookRepositoryTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        _testFilePath = Path.Combine(_testDirectory, "books.json");

        var options = Options.Create(new LibraryStorageOptions
        {
            FilePath = _testFilePath
        });

        var loggerMock = new Mock<ILogger<JsonFileContext>>();

        var context = new JsonFileContext(options, loggerMock.Object);

        _repository = new JsonBookRepository(context);
    }

    [Fact]
    public async Task AddAsync_ShouldSaveBookToJson()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        // Act
        await _repository.AddAsync(book);

        // Assert
        File.Exists(_testFilePath).Should().BeTrue();

        string json = await File.ReadAllTextAsync(_testFilePath);

        json.Should().Contain("Clean Code");
        json.Should().Contain("Robert Martin");
        json.Should().Contain("BK-001");
    }

    [Fact]
    public async Task GetAllAsync_ShouldReadBooksFromJson()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        await _repository.AddAsync(book);

        // Act
        var result = await _repository.GetAllAsync(It.IsAny<CancellationToken>());

        // Assert
        result.Should().HaveCount(1);

        var savedBook = result.Single();

        savedBook.Title.Should().Be("Clean Code");
        savedBook.Author.Should().Be("Robert Martin");
        savedBook.Year.Should().Be(2008);
        savedBook.Code.Should().Be("BK-001");
    }

    [Fact]
    public void Repository_ShouldCreateFile_WhenFileDoesNotExist()
    {
        // Assert
        File.Exists(_testFilePath).Should().BeTrue();
    }

    [Fact]
    public async Task Repository_ShouldReturnEmptyList_WhenFileIsEmpty()
    {
        // Arrange
        await File.WriteAllTextAsync(_testFilePath, string.Empty);

        // Act
        var result = await _repository.GetAllAsync(It.IsAny<CancellationToken>());

        // Assert
        result.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateByCodeAsync_WhenBookExists_ShouldUpdateBook()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        await _repository.AddAsync(book);

        // Act
        var updatedBook = await _repository.UpdateByCodeAsync(
            "BK-001",
            book => book.UpdateDetails("Updated Title", "Updated Author", 2020));

        // Assert
        updatedBook.Title.Should().Be("Updated Title");
        updatedBook.Author.Should().Be("Updated Author");
        updatedBook.Year.Should().Be(2020);

        var books = await _repository.GetAllAsync(It.IsAny<CancellationToken>());

        books.Should().HaveCount(1);
        books.Single().Title.Should().Be("Updated Title");
    }

    [Fact]
    public async Task UpdateByCodeAsync_WhenBookIsSoftDeleted_ShouldNotReturnBookFromGetAll()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        await _repository.AddAsync(book);

        // Act
        await _repository.UpdateByCodeAsync(
            "BK-001",
            book => book.MarkAsDeleted());

        // Assert
        var books = await _repository.GetAllAsync(It.IsAny<CancellationToken>());

        books.Should().BeEmpty();
    }

    [Fact]
    public async Task UpdateByCodeAsync_WhenBorrowingBook_ShouldChangeStatus()
    {
        // Arrange
        var book = new Book("Clean Code", "Robert Martin", 2008, "BK-001");

        await _repository.AddAsync(book);

        // Act
        await _repository.UpdateByCodeAsync(
            "BK-001",
            book => book.Borrow());

        // Assert
        var updatedBook = await _repository.GetByCodeAsync("BK-001");

        updatedBook.Should().NotBeNull();
        updatedBook!.Status.Should().Be(BookStatus.Borrowed);
    }

    [Fact]
    public async Task UpdateByCodeAsync_WhenBookDoesNotExist_ShouldThrowBookNotFoundException()
    {
        // Act
        Func<Task> action = () => _repository.UpdateByCodeAsync(
            "BK-404",
            book => book.Borrow());

        // Assert
        await action.Should().ThrowAsync<BookNotFoundException>();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}
