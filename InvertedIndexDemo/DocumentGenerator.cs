namespace InvertedIndexDemo;

public static class DocumentGenerator
{
    private static readonly string[] Vocabulary =
    {
        "data", "cloud", "server", "database", "algorithm", "network", "security",
        "framework", "compiler", "memory", "thread", "api", "testing", "design",
        "pattern", "interface", "class", "object", "method", "variable", "library",
        "debugging", "deployment", "version", "git", "web", "mobile", "game", "engine", "queue"
    };

    public static List<Document> Generate(IReadOnlyList<Document> baseDocuments, int count, int seed = 42)
    {
        
        var random = new Random(seed);
        var documents = new List<Document>(count);

        for (int i = 1; i <= count; i++)
        {
            Document template = baseDocuments[(i - 1) % baseDocuments.Count];
            IEnumerable<string> extraWords = Enumerable.Range(0, 10)
                .Select(_ => Vocabulary[random.Next(Vocabulary.Length)]);

            documents.Add(new Document
            {
                Id = i,
                Title = $"{template.Title} #{i}",
                Content = template.Content + " " + string.Join(' ', extraWords)
            });
        }

        return documents;
    }
}