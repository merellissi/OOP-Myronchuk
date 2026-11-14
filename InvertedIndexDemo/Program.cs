using InvertedIndexDemo;

Console.OutputEncoding = System.Text.Encoding.UTF8;
Console.InputEncoding = System.Text.Encoding.UTF8;


var documents = new List<Document>
{
    new Document
    {
        Id = 1,
        Title = "Introduction to C#",
        Content = "C# is a modern object-oriented programming language developed by Microsoft."
    },
    new Document
    {
        Id = 2,
        Title = "Java Basics",
        Content = "Java is a popular programming language known for its portability and performance."
    },
    new Document
    {
        Id = 3,
        Title = "Python Overview",
        Content = "Python is a high-level programming language with simple syntax."
    },
    new Document
    {
        Id = 4,
        Title = "C# Advanced Features",
        Content = "C# supports LINQ, async/await, and many modern programming paradigms."
    },
    new Document
    {
        Id = 5,
        Title = "Comparing Languages",
        Content = "C#, Java, and Python are among the most popular programming languages today."
    }
};


var index = new InvertedIndex();
foreach (Document doc in documents)
    index.AddDocument(doc);

Console.WriteLine("===== Частина 1. Інвертований індекс =====");
Console.WriteLine($"Loaded {index.DocumentCount} documents");
Console.WriteLine($"Index contains {index.UniqueWords} unique words");
Console.WriteLine($"Tokenize(\"Hello, world! C# is great.\") -> [{string.Join(", ", TextProcessor.Tokenize("Hello, world! C# is great."))}]");

PrintResults("SearchSingleWord(\"python\")", index.SearchSingleWord("python"));
PrintResults("SearchSingleWord(\"C#\")", index.SearchSingleWord("C#"));
PrintResults("SearchMultipleWords(\"programming\", \"language\")  — AND", index.SearchMultipleWords("programming", "language"));
PrintResults("SearchMultipleWords(\"c#\", \"async\")  — AND", index.SearchMultipleWords("c#", "async"));
PrintResults("SearchAnyWord(\"java\", \"python\", \"linq\")  — OR", index.SearchAnyWord("java", "python", "linq"));
PrintResults("SearchSingleWord(\"rust\")", index.SearchSingleWord("rust"));


Console.WriteLine();
Console.WriteLine("===== Частина 3. Продуктивність: лінійний пошук vs інвертований індекс =====");
Console.WriteLine("Шукане слово: \"python\". Час — середній на один пошук.");
Console.WriteLine();

PerformanceTest.PrintHeader();
foreach (int count in new[] { 10, 100, 1000, 10000 })
{
    List<Document> generated = DocumentGenerator.Generate(documents, count);
    PerformanceTest.CompareSearchMethods(generated, "python");
}


Console.WriteLine();
Console.WriteLine("===== Частина 4. Інтерактивний пошук (AND) =====");

while (true)
{
    Console.Write("Enter search query (or 'exit'): ");
    string? query = Console.ReadLine();

    if (query is null || query.Trim().Equals("exit", StringComparison.OrdinalIgnoreCase))
        break;

    string[] words = query.Split(' ', StringSplitOptions.RemoveEmptyEntries);
    PrintResults("Search Results (AND)", index.SearchMultipleWords(words));
}


static void PrintResults(string title, List<Document> results)
{
    Console.WriteLine();
    Console.WriteLine($"--- {title} ---");

    foreach (Document doc in results)
    {
        Console.WriteLine(doc);
        Console.WriteLine($"   {doc.Content.Substring(0, Math.Min(100, doc.Content.Length))}");
    }

    Console.WriteLine($"Found {results.Count} documents");
}