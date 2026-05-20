namespace LibraryManagement.Infrastructure.Configuration;

public class LibraryStorageOptions
{
    public const string SectionName = "LibraryStorage";
    public string FilePath { get; set; } = "data/books.json";
}
