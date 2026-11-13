using CollectionsDemo;

Console.OutputEncoding = System.Text.Encoding.UTF8;


static string Show<T>(IEnumerable<T> items) =>
    "{ " + string.Join(", ", items.OrderBy(x => x)) + " }";


Console.WriteLine("===== Сценарій 1: FileMetadataCache (Dictionary) =====");

var cache = new FileMetadataCache();
cache.Add(new FileMetadata { FileName = "report.pdf", Size = 204_800, LastModified = new DateTime(2026, 11, 10, 14, 30, 0), Hash = "a1b2c3" });
cache.Add(new FileMetadata { FileName = "photo.jpg", Size = 3_145_728, LastModified = new DateTime(2026, 11, 12, 9, 15, 0), Hash = "d4e5f6" });
cache.Add(new FileMetadata { FileName = "notes.txt", Size = 1_024, LastModified = new DateTime(2026, 11, 13, 8, 0, 0), Hash = "0f9e8d" });

Console.WriteLine($"Файлів у кеші: {cache.Count}");
Console.WriteLine($"Get(\"report.pdf\"): {cache.Get("report.pdf")}");
Console.WriteLine($"Contains(\"REPORT.PDF\") (інший регістр): {cache.Contains("REPORT.PDF")}");

Console.WriteLine($"Remove(\"photo.jpg\"): {cache.Remove("photo.jpg")}");
Console.WriteLine($"Contains(\"photo.jpg\") після видалення: {cache.Contains("photo.jpg")}");
Console.WriteLine($"Файлів у кеші: {cache.Count}");

try
{
    cache.Get("missing.doc");
}
catch (KeyNotFoundException ex)
{
    Console.WriteLine($"Get(\"missing.doc\") -> KeyNotFoundException: {ex.Message}");
}

Console.WriteLine();
Console.WriteLine("===== Сценарій 2: TagManager (HashSet) =====");

var tags = new TagManager();
tags.AddTag("article-1", "csharp");
tags.AddTag("article-1", "dotnet");
tags.AddTag("article-1", "collections");
tags.AddTag("article-1", "CSharp"); 

tags.AddTag("article-2", "dotnet");
tags.AddTag("article-2", "collections");
tags.AddTag("article-2", "performance");

Console.WriteLine($"Теги article-1: {Show(tags.GetTags("article-1"))}");
Console.WriteLine($"Теги article-2: {Show(tags.GetTags("article-2"))}");
Console.WriteLine($"Усі унікальні теги (UnionWith): {Show(tags.GetAllUniqueTags())}");
Console.WriteLine($"Спільні теги (IntersectWith): {Show(tags.FindCommonTags("article-1", "article-2"))}");
Console.WriteLine($"Є в article-1, немає в article-2 (ExceptWith): {Show(tags.FindUniqueTags("article-1", "article-2"))}");

tags.RemoveTag("article-1", "collections");
Console.WriteLine($"Після RemoveTag(\"article-1\", \"collections\"): {Show(tags.GetTags("article-1"))}");


Console.WriteLine();
Console.WriteLine("===== Сценарій 3: SearchIndex (Dictionary + HashSet) =====");

var index = new SearchIndex();
index.AddDocument(1, "Dictionary stores key-value pairs");
index.AddDocument(2, "HashSet stores unique values");
index.AddDocument(3, "Dictionary and HashSet use hashing");

Console.WriteLine($"Search(\"dictionary\"): {Show(index.Search("dictionary"))}");
Console.WriteLine($"Search(\"stores\"): {Show(index.Search("stores"))}");
Console.WriteLine($"SearchMultiple(\"dictionary\", \"hashset\"): {Show(index.SearchMultiple("dictionary", "hashset"))}");
Console.WriteLine($"Search(\"python\"): {Show(index.Search("python"))}");