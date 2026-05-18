namespace LibraryManagement.Console.Input;

public class ConsoleInputReader
{
    public string ReadRequiredString(string message)
    {
        while (true)
        {
            System.Console.Write(message);

            string? input = System.Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(input))
            {
                return input.Trim();
            }

            System.Console.WriteLine("Value is required. Please try again.");
        }
    }

    public int ReadInt(string message)
    {
        while (true)
        {
            System.Console.Write(message);

            string? input = System.Console.ReadLine();

            if (int.TryParse(input, out int value))
            {
                return value;
            }

            System.Console.WriteLine("Invalid number. Please try again.");
        }
    }
}
