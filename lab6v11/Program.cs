using Lab6v11;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("=== Частина 1: IComparer<string> ===");

List<Lab6v11.IComparer<string>> comparers = new List<Lab6v11.IComparer<string>>
{
    new StringLengthComparer(),
    new AlphabeticalComparer()
};

List<string> words = new List<string> { "banana", "kiwi", "apple", "fig", "cherry" };

foreach (Lab6v11.IComparer<string> comparer in comparers)
{
    Console.WriteLine($"\n{comparer.GetType().Name}: Compare(\"kiwi\", \"apple\") = {comparer.Compare("kiwi", "apple")}");

    List<string> copy = new List<string>(words);
    copy.Sort(comparer.Compare);
    Console.WriteLine("Відсортовано: " + string.Join(", ", copy));
}

Console.WriteLine("\n=== Частина 2: FileProcessor ===");

File.WriteAllLines("app.log", new[]
{
    "2025-01-01 INFO  Application started",
    "2025-01-01 WARN  Disk space low",
    "2025-01-01 INFO  User logged in",
    "2025-01-01 ERROR Connection lost"
});

File.WriteAllLines("settings.cfg", new[]
{
    "# Налаштування програми",
    "",
    "  host=localhost  ",
    "port=8080",
    "# debug=true",
    "mode=production"
});

List<(FileProcessor Processor, string Input, string Output)> jobs = new()
{
    (new LogFileProcessor(),    "app.log",      "app_result.txt"),
    (new ConfigFileProcessor(), "settings.cfg", "settings_result.txt")
};

foreach (var (processor, input, output) in jobs)
{
    Console.WriteLine($"\n{processor.GetType().Name}:");
    processor.ReadFile(input);
    processor.ProcessContent();
    processor.PrintLines();
    processor.WriteResult(output);
}