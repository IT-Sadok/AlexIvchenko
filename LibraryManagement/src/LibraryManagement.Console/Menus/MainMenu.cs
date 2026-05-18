using LibraryManagement.Application.Abstractions;
using LibraryManagement.Application.DTOs;
using LibraryManagement.Console.Input;
using LibraryManagement.Console.Output;
using LibraryManagement.Domain.Exceptions;
using Microsoft.Extensions.Logging;

namespace LibraryManagement.Console.Menus;

public class MainMenu
{
    private readonly IBookService _bookService;
    private readonly ConsoleInputReader _inputReader;
    private readonly ConsoleWriter _writer;
    private readonly ILogger<MainMenu> _logger;

    public MainMenu(
        IBookService bookService,
        ConsoleInputReader inputReader,
        ConsoleWriter writer,
        ILogger<MainMenu> logger)
    {
        _bookService = bookService;
        _inputReader = inputReader;
        _writer = writer;
        _logger = logger;
    }

    public async Task RunAsync()
    {
        bool shouldExit = false;

        while (!shouldExit)
        {
            ShowMenu();

            string option = _inputReader.ReadRequiredString("Select option: ");

            System.Console.WriteLine();

            try
            {
                shouldExit = await HandleOptionAsync(option);
            }
            catch (BookNotFoundException exception)
            {
#if DEBUG
                _logger.LogWarning(exception, "Book was not found.");
#else
                _logger.LogWarning("Book was not found. Code: {BookCode}", exception.Code);
#endif
                _writer.WriteError("Book was not found.");
            }
            catch (BookAlreadyBorrowedException exception)
            {
#if DEBUG

                _logger.LogWarning(exception, "Book is already borrowed.");
#else
                _logger.LogWarning("Book is already borrowed. Code: {BookCode}", exception.Code);
#endif
                _writer.WriteError("Book is already borrowed.");
            }
            catch (BookAlreadyAvailableException exception)
            {
                //
#if DEBUG

                _logger.LogWarning(exception, "Book is already available.");
#else
                _logger.LogWarning("Book is already available. Code: {BookCode}", exception.Code);
#endif
                _writer.WriteError("Book is already available.");
            }
            catch (BookDeletedException exception)
            {
#if DEBUG

                _logger.LogWarning(exception, "Book is deleted.");
#else
                _logger.LogWarning("Book is deleted. Code: {BookCode}", exception.Code);
#endif
                _writer.WriteError("Book is deleted and cannot be modified.");
            }
            catch (ArgumentException exception)
            {
#if DEBUG

                _logger.LogWarning(exception, "Validation error.");
#else
                _logger.LogWarning("Validation error: {Message}", exception.Message);
#endif
                _writer.WriteError(exception.Message);
            }
            catch (InvalidOperationException exception)
            {
#if DEBUG

                _logger.LogError(exception, "Application operation error.");
#else
                _logger.LogError("Application error: {Message}", exception.Message);
#endif
                _writer.WriteError(exception.Message);
            }
            catch (UnauthorizedAccessException exception)
            {
#if DEBUG

                _logger.LogError(exception, "Storage access error.");
#else
                _logger.LogError("Storage access error: {Message}", exception.Message);
#endif
                _writer.WriteError("Access to the storage file is denied.");
            }
            catch (IOException exception)
            {
#if DEBUG

                _logger.LogError(exception, "Storage file error.");
#else
                _logger.LogError("Storage file error: {Message}", exception.Message);
#endif
                _writer.WriteError("Something went wrong while reading or writing the storage file.");
            }
            catch (Exception exception)
            {
                _logger.LogError(exception, "Unhandled exception.");
                _writer.WriteError("Unexpected error occurred. Please try again.");
            }

            System.Console.WriteLine();
            System.Console.WriteLine("Press any key to continue...");
            System.Console.ReadKey();
            System.Console.Clear();
        }
    }

    private static void ShowMenu()
    {
        System.Console.WriteLine("===== Library Management =====");
        System.Console.WriteLine("1. Add book");
        System.Console.WriteLine("2. Delete book");
        System.Console.WriteLine("3. Search books");
        System.Console.WriteLine("4. Show all books");
        System.Console.WriteLine("5. Show available books");
        System.Console.WriteLine("6. Borrow book");
        System.Console.WriteLine("7. Return book");
        System.Console.WriteLine("8. Update book");
        System.Console.WriteLine("0. Exit");
        System.Console.WriteLine();
    }

    private async Task<bool> HandleOptionAsync(string option)
    {
        switch (option)
        {
            case "1":
                await AddBookAsync();
                return false;

            case "2":
                await DeleteBookAsync();
                return false;

            case "3":
                await SearchBooksAsync();
                return false;

            case "4":
                await ShowAllBooksAsync();
                return false;

            case "5":
                await ShowAvailableBooksAsync();
                return false;

            case "6":
                await BorrowBookAsync();
                return false;

            case "7":
                await ReturnBookAsync();
                return false;

            case "8":
                await UpdateBookAsync();
                return false;

            case "0":
                _writer.WriteInfo("Goodbye!");
                return true;

            default:
                _writer.WriteError("Invalid option. Please try again.");
                return false;
        }
    }

    private async Task AddBookAsync()
    {
        string title = _inputReader.ReadRequiredString("Enter title: ");
        string author = _inputReader.ReadRequiredString("Enter author: ");
        int year = _inputReader.ReadInt("Enter year: ");
        string code = _inputReader.ReadRequiredString("Enter code: ");

        var request = new CreateBookRequest
        {
            Title = title,
            Author = author,
            Year = year,
            Code = code
        };

        var book = await _bookService.AddAsync(request);

        _writer.WriteSuccess($"Book '{book.Title}' was added successfully.");
    }

    private async Task DeleteBookAsync()
    {
        string code = _inputReader.ReadRequiredString("Enter book code: ");

        await _bookService.DeleteAsync(code);

        _writer.WriteSuccess("Book was deleted successfully.");
    }

    private async Task SearchBooksAsync()
    {
        string searchTerm = _inputReader.ReadRequiredString("Enter search term (title, author, or code): ");

        var books = await _bookService.SearchAsync(searchTerm);

        _writer.WriteBooks(books);
    }

    private async Task ShowAllBooksAsync()
    {
        var books = await _bookService.GetAllAsync();

        _writer.WriteBooks(books);
    }

    private async Task ShowAvailableBooksAsync()
    {
        var books = await _bookService.GetAvailableAsync();

        _writer.WriteBooks(books);
    }

    private async Task BorrowBookAsync()
    {
        string code = _inputReader.ReadRequiredString("Enter book code: ");

        await _bookService.BorrowAsync(code);

        _writer.WriteSuccess("Book was borrowed successfully.");
    }

    private async Task ReturnBookAsync()
    {
        string code = _inputReader.ReadRequiredString("Enter book code: ");

        await _bookService.ReturnAsync(code);

        _writer.WriteSuccess("Book was returned successfully.");
    }

    private async Task UpdateBookAsync()
    {
        string code = _inputReader.ReadRequiredString("Enter book code: ");
        string title = _inputReader.ReadRequiredString("Enter new title: ");
        string author = _inputReader.ReadRequiredString("Enter new author: ");
        int year = _inputReader.ReadInt("Enter new year: ");

        var request = new UpdateBookRequest
        {
            Title = title,
            Author = author,
            Year = year
        };

        var updatedBook = await _bookService.UpdateAsync(code, request);

        _writer.WriteSuccess($"Book '{updatedBook.Title}' was updated successfully.");
    }
}
