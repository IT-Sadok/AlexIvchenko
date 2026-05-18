using LibraryManagement.Application.DTOs;

namespace LibraryManagement.Console.Output;

public class ConsoleWriter
{
    public void WriteSuccess(string message)
    {
        WriteWithColor(message, ConsoleColor.Green);
    }

    public void WriteError(string message)
    {
        WriteWithColor(message, ConsoleColor.Red);
    }

    public void WriteInfo(string message)
    {
        WriteWithColor(message, ConsoleColor.Cyan);
    }

    public void WriteBooks(IEnumerable<BookDto> books)
    {
        var bookList = books.ToList();

        if (bookList.Count == 0)
        {
            WriteInfo("No books found.");
            return;
        }

        foreach (var book in bookList)
        {
            System.Console.WriteLine("----------------------------------------");
            System.Console.WriteLine($"Id:     {book.Id}");
            System.Console.WriteLine($"Title:  {book.Title}");
            System.Console.WriteLine($"Author: {book.Author}");
            System.Console.WriteLine($"Year:   {book.Year}");
            System.Console.WriteLine($"Code:   {book.Code}");
            System.Console.WriteLine($"Status: {book.Status}");
        }

        System.Console.WriteLine("----------------------------------------");
    }

    private static void WriteWithColor(string message, ConsoleColor color)
    {
        var originalColor = System.Console.ForegroundColor;

        System.Console.ForegroundColor = color;
        System.Console.WriteLine(message);
        System.Console.ForegroundColor = originalColor;
    }
}
