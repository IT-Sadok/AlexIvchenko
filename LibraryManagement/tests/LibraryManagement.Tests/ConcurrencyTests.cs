using FluentAssertions;
using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.Models;
using LibraryManagement.Application.Services;
using LibraryManagement.Application.Validators;
using LibraryManagement.Infrastructure.Configuration;
using LibraryManagement.Infrastructure.Persistence;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using Moq;

namespace LibraryManagement.Tests;

public class ConcurrencyTests : IDisposable
{
    private readonly string _testDirectory;
    private readonly string _filePath;
    private readonly IBookService _bookService;

    public ConcurrencyTests()
    {
        _testDirectory = Path.Combine(Path.GetTempPath(), Guid.NewGuid().ToString());
        Directory.CreateDirectory(_testDirectory);

        _filePath = Path.Combine(_testDirectory, "books.json");

        var options = Options.Create(new LibraryStorageOptions
        {
            FilePath = _filePath
        });

        var jsonFileContext = new JsonFileContext(
            options,
            NullLogger<JsonFileContext>.Instance);

        IBookRepository bookRepository = new JsonBookRepository(jsonFileContext);

        var validator = new BookValidator(bookRepository);

        _bookService = new BookService(
            bookRepository,
            validator,
            NullLogger<BookService>.Instance);
    }

    [Fact]
    public async Task ManyTasks_WhenRunConcurrently_ShouldNotCorruptBooksData()
    {
        // Arrange
        for (int i = 0; i < 70; i++)
        {
            await _bookService.AddAsync(new CreateBookRequest
            {
                Title = $"Book {i}",
                Author = $"Author {i}",
                Year = 2000 + i % 20,
                Code = $"BK-{i:D3}"
            });
        }

        for (int i = 20; i < 30; i++)
        {
            await _bookService.BorrowAsync($"BK-{i:D3}");
        }

        var tasks = new List<Task>();

        /* add 20 tasks */
        for (int i = 0; i < 20; i++)
        {
            int taskNumber = i;

            tasks.Add(Task.Run(async () =>
            {
                await _bookService.AddAsync(new CreateBookRequest
                {
                    Title = $"New Book {taskNumber}",
                    Author = $"New Author {taskNumber}",
                    Year = 2024,
                    Code = $"NEW-{taskNumber:D3}"
                });
            }));
        }

        /* delete 10 tasks */
        for (int i = 0; i < 10; i++)
        {
            int taskNumber = i;

            tasks.Add(Task.Run(async () =>
            {
                await _bookService.DeleteAsync($"BK-{taskNumber:D3}");
            }));
        }

        /* borrow 10 tasks */
        for (int i = 10; i < 20; i++)
        {
            int taskNumber = i;

            tasks.Add(Task.Run(async () =>
            {
                await _bookService.BorrowAsync($"BK-{taskNumber:D3}");
            }));
        }

        /* return 10 tasks */
        for (int i = 20; i < 30; i++)
        {
            int taskNumber = i;

            tasks.Add(Task.Run(async () =>
            {
                await _bookService.ReturnAsync($"BK-{taskNumber:D3}");
            }));
        }

        /* update 30 tasks */
        for (int i = 30; i < 60; i++)
        {
            int taskNumber = i;

            tasks.Add(Task.Run(async () =>
            {
                await _bookService.UpdateAsync(
                    $"BK-{taskNumber:D3}",
                    new UpdateBookRequest
                    {
                        Title = $"Updated Book {taskNumber}",
                        Author = $"Updated Author {taskNumber}",
                        Year = 2025
                    });
            }));
        }

        /* read 20 tasks */
        for (int i = 0; i < 20; i++)
        {
            tasks.Add(Task.Run(async () =>
            {
                await _bookService.GetAllAsync(It.IsAny<CancellationToken>());
            }));
        }

        // Act
        await Task.WhenAll(tasks);

        var result = await _bookService.GetAllAsync(It.IsAny<CancellationToken>());

        // Assert
        result.IsSuccess.Should().BeTrue();

        var books = result.Value;

        books.Should().NotBeNull();

        /* 70 initial + 20 added - 10 soft deleted = 80 active books */
        books!.Should().HaveCount(80);

        books
            .Select(book => book.Code)
            .Should()
            .OnlyHaveUniqueItems();
    }

    public void Dispose()
    {
        if (Directory.Exists(_testDirectory))
        {
            Directory.Delete(_testDirectory, recursive: true);
        }
    }
}
