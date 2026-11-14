using System.Diagnostics;
using IndependentWork14;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var operations = new FlakyOperations();


static bool IsTransient(Exception ex) => ex is HttpRequestException or TimeoutException;


var policy = new RetryPolicy(maxRetries: 4, initialDelay: TimeSpan.FromMilliseconds(300), jitterFactor: 0.2);

Console.WriteLine("===== Сценарій 1. Прайс-лист: 3 збої, потім успіх =====");
Console.WriteLine("RetryPolicy(maxRetries: 4, initialDelay: 300 мс, jitterFactor: 0.2)");
var sw = Stopwatch.StartNew();
string priceList = policy.Execute(operations.DownloadPriceList, IsTransient);
sw.Stop();
Console.WriteLine($"Результат: {priceList}");
Console.WriteLine($"Загальний час: {sw.Elapsed.TotalSeconds:F2} с");


Console.WriteLine();
Console.WriteLine("===== Сценарій 2. Залишок на складі (T = int), shouldRetry не задано =====");
int stock = policy.Execute(() => operations.GetStock("Ноутбук"));
Console.WriteLine($"Результат: на складі {stock} шт.");


Console.WriteLine();
Console.WriteLine("===== Сценарій 3. База даних недоступна: усі спроби невдалі =====");
var dbPolicy = new RetryPolicy(maxRetries: 3, initialDelay: TimeSpan.FromMilliseconds(200), jitterFactor: 0.3);
Console.WriteLine("RetryPolicy(maxRetries: 3, initialDelay: 200 мс, jitterFactor: 0.3)");
sw.Restart();
try
{
    decimal revenue = dbPolicy.Execute(operations.GetDailyRevenue, IsTransient);
    Console.WriteLine($"Виручка: {revenue}");
}
catch (TimeoutException ex)
{
    sw.Stop();
    Console.WriteLine($"Main перехопив остаточний виняток: {ex.GetType().Name}: {ex.Message}");
    Console.WriteLine($"Загальний час: {sw.Elapsed.TotalSeconds:F2} с — показуємо користувачу повідомлення про помилку");
}


Console.WriteLine();
Console.WriteLine("===== Сценарій 4. Некоректне замовлення: shouldRetry = false =====");
try
{
    int orderId = policy.Execute(() => operations.CreateOrder(quantity: 0), IsTransient);
    Console.WriteLine($"Замовлення №{orderId}");
}
catch (ArgumentException ex)
{
    Console.WriteLine($"Main перехопив виняток одразу, без затримок: {ex.Message}");
}



Console.WriteLine();
Console.WriteLine("===== Затримки для 5 клієнтів, що отримали помилку одночасно (initialDelay 300 мс, jitter ±20%) =====");
Console.WriteLine($"{"Повтор",-8}{"База, мс",10}{"Діапазон, мс",18}   Затримки клієнтів, мс");
for (int retryAttempt = 0; retryAttempt < 4; retryAttempt++)
{
    double baseMs = policy.BaseDelay(retryAttempt).TotalMilliseconds;
    string range = $"{baseMs * 0.8:F0}–{baseMs * 1.2:F0}";
    var clients = Enumerable.Range(0, 5)
        .Select(_ => policy.CalculateDelay(retryAttempt).TotalMilliseconds.ToString("F0"));

    Console.WriteLine($"#{retryAttempt + 1,-7}{baseMs,10:F0}{range,18}   {string.Join(", ", clients)}");
}