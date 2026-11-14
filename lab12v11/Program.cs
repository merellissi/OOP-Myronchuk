
using Lab12v11;

Console.OutputEncoding = System.Text.Encoding.UTF8;

static bool ShouldRetry(Exception ex) => ex is FileNotFoundException or HttpRequestException;

string dataDir = Path.Combine(Path.GetTempPath(), "lab12v11");
Directory.CreateDirectory(dataDir);
string productsFile = Path.Combine(dataDir, "products.txt");
File.WriteAllLines(productsFile, new[] { "Смартфон Samsung A55", "Планшет Lenovo Tab", "Повербанк Xiaomi 20000", "Смарт-годинник Amazfit", "Колонка JBL Go 4" });

TimeSpan initialDelay = TimeSpan.FromMilliseconds(200);


Console.WriteLine("===== 1. FileProcessor.LoadProductNames: 2 збої, потім успіх =====");
Console.WriteLine($"Файл: {productsFile}");
var fileProcessor = new FileProcessor(failuresBeforeSuccess: 2);
try
{
    List<string> products = RetryHelper.ExecuteWithRetry(
        () => fileProcessor.LoadProductNames(productsFile),
        retryCount: 3, initialDelay: initialDelay, shouldRetry: ShouldRetry);

    Console.WriteLine($"Результат ({products.Count} продуктів, викликів: {fileProcessor.Calls}):");
    products.ForEach(p => Console.WriteLine($"   - {p}"));
}
catch (Exception ex)
{
    Console.WriteLine($"Помилка: {ex.Message}");
}
finally
{
    Console.WriteLine("finally: роботу з файлом завершено");
}


Console.WriteLine();
Console.WriteLine("===== 2. NetworkClient.GetProductsFromApi: 3 збої, потім успіх =====");
var networkClient = new NetworkClient(failuresBeforeSuccess: 3);
try
{
    List<string> products = RetryHelper.ExecuteWithRetry(
        () => networkClient.GetProductsFromApi("https://api.shop.example/products"),
        retryCount: 3, initialDelay: initialDelay, shouldRetry: ShouldRetry);

    Console.WriteLine($"Результат ({products.Count} продуктів, викликів: {networkClient.Calls}):");
    products.ForEach(p => Console.WriteLine($"   - {p}"));
}
catch (HttpRequestException ex)
{
    Console.WriteLine($"Мережева помилка: {ex.Message}");
}
finally
{
    Console.WriteLine("finally: з'єднання з API закрито");
}


Console.WriteLine();
Console.WriteLine("===== 3. shouldRetry: DirectoryNotFoundException не повторюється =====");
var noFailures = new FileProcessor(failuresBeforeSuccess: 0);
string wrongDirPath = Path.Combine(dataDir, "no_such_folder", "products.txt");
try
{
    RetryHelper.ExecuteWithRetry(
        () => noFailures.LoadProductNames(wrongDirPath),
        retryCount: 3, initialDelay: initialDelay, shouldRetry: ShouldRetry);
}
catch (DirectoryNotFoundException ex)
{
    Console.WriteLine($"Main перехопив DirectoryNotFoundException без повторів (викликів: {noFailures.Calls})");
    Console.WriteLine($"   {ex.Message}");
}
finally
{
    Console.WriteLine("finally: роботу з файлом завершено");
}

Console.WriteLine();
Console.WriteLine("===== 4. shouldRetry: ArgumentException не повторюється =====");
var client2 = new NetworkClient(failuresBeforeSuccess: 0);
try
{
    RetryHelper.ExecuteWithRetry(
        () => client2.GetProductsFromApi("api.shop/products"),
        retryCount: 3, initialDelay: initialDelay, shouldRetry: ShouldRetry);
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Main перехопив ArgumentException без повторів (мережевих викликів: {client2.Calls}): {ex.Message}");
}
finally
{
    Console.WriteLine("finally: з'єднання з API закрито");
}


Console.WriteLine();
Console.WriteLine("===== 5. Файлу немає: FileNotFoundException після всіх спроб =====");
var realMissing = new FileProcessor(failuresBeforeSuccess: 0);
string missingFile = Path.Combine(dataDir, "missing.txt");
try
{
    RetryHelper.ExecuteWithRetry(
        () => realMissing.LoadProductNames(missingFile),
        retryCount: 3, initialDelay: initialDelay, shouldRetry: ShouldRetry);
}
catch (FileNotFoundException ex)
{
    Console.WriteLine($"Main перехопив FileNotFoundException після {realMissing.Calls} спроб: {ex.Message}");
}
finally
{
    Console.WriteLine("finally: роботу з файлом завершено");
}