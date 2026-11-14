using System.Diagnostics;

namespace InvertedIndexDemo;


public static class PerformanceTest
{
    public static void PrintHeader()
    {
        Console.WriteLine(
            $"{"Документів",10} | {"Лінійний, мкс",14} | {"Індекс, мкс",12} | {"Побудова індексу, мкс",22} | {"У скільки разів",15} | {"Знайдено (лін/інд)",18} | Окупність, запитів");
        Console.WriteLine(new string('-', 125));
    }

    public static void CompareSearchMethods(List<Document> documents, string searchWord)
    {
       
        int iterations = Math.Clamp(100_000 / documents.Count, 10, 1000);
        string normalizedWord = TextProcessor.GetTerms(searchWord).First();

       
        List<Document> LinearSearch() => documents
            .Where(d => TextProcessor.GetTerms(d.Content).Contains(normalizedWord))
            .ToList();

        List<Document> linearResults = LinearSearch(); 
        var sw = Stopwatch.StartNew();
        for (int i = 0; i < iterations; i++)
            linearResults = LinearSearch();
        sw.Stop();
        double linearUs = ToMicroseconds(sw.ElapsedTicks) / iterations;

        
        InvertedIndex index = BuildIndex(documents); 
        int buildRepeats = Math.Clamp(10_000 / documents.Count, 1, 100);
        sw.Restart();
        for (int i = 0; i < buildRepeats; i++)
            index = BuildIndex(documents);
        sw.Stop();
        double buildUs = ToMicroseconds(sw.ElapsedTicks) / buildRepeats;

      
        List<Document> indexResults = index.SearchSingleWord(searchWord); 
        sw.Restart();
        for (int i = 0; i < iterations; i++)
            indexResults = index.SearchSingleWord(searchWord);
        sw.Stop();
        double indexUs = ToMicroseconds(sw.ElapsedTicks) / iterations;

      
        double speedup = linearUs / indexUs;

        
        string breakEven = linearUs > indexUs
            ? Math.Ceiling(buildUs / (linearUs - indexUs)).ToString()
            : "не окупається";

        string found = $"{linearResults.Count}/{indexResults.Count}";
        if (linearResults.Count != indexResults.Count)
            found += " ПОМИЛКА!";

        Console.WriteLine(
            $"{documents.Count,10} | {linearUs,14:F2} | {indexUs,12:F3} | {buildUs,22:F1} | {speedup,14:F0}x | {found,18} | {breakEven}");
    }

    private static InvertedIndex BuildIndex(List<Document> documents)
    {
        var index = new InvertedIndex();
        foreach (Document doc in documents)
            index.AddDocument(doc);
        return index;
    }

    private static double ToMicroseconds(long ticks) => ticks * 1_000_000.0 / Stopwatch.Frequency;
}