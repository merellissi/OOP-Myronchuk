using System.Globalization;
using IndependentWork19;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");


Console.WriteLine("===== Частина 1. Власний виняток OutOfStockException =====");

Console.WriteLine("--- Три стандартні конструктори + конструктор з контекстом ---");
var examples = new OutOfStockException[]
{
    new(),
    new("Товар закінчився під час оформлення замовлення."),
    new("Не вдалося зарезервувати товар.", new TimeoutException("Склад не відповів за 5 с")),
    new("Недостатньо товару «Ноутбук» на складі.", "Ноутбук", 5, 2)
};
foreach (OutOfStockException ex in examples)
{
    Console.WriteLine($"Message: {ex.Message}");
    Console.WriteLine($"   InnerException: {ex.InnerException?.Message ?? "немає"}; " +
                      $"Product: {ex.ProductName ?? "—"}, Requested: {ex.RequestedQuantity}, Available: {ex.AvailableQuantity}");
}

Console.WriteLine();
Console.WriteLine("--- Резервування товарів з try-catch-finally ---");
var warehouse = new Warehouse(new Dictionary<string, int>
{
    ["Ноутбук"] = 3,
    ["Мишка"] = 10,
    ["Монітор"] = 0
});

var orders = new (string Product, int Quantity)[]
{
    ("Ноутбук", 2),
    ("Ноутбук", 5),
    ("Монітор", 1),
    ("Мишка", 0),
    ("Планшет", 1)
};

foreach (var (product, quantity) in orders)
{
    Console.WriteLine($"Замовлення: {product} × {quantity}");
    try
    {
        warehouse.Reserve(product, quantity);
        Console.WriteLine($"   OK: зарезервовано. Залишок: {warehouse.GetStock(product)} шт.");
    }
    
    catch (OutOfStockException ex) when (ex.AvailableQuantity > 0)
    {
        Console.WriteLine($"   OutOfStockException: {ex.Message} Запитано {ex.RequestedQuantity}, є {ex.AvailableQuantity}, бракує {ex.Shortage}.");
        Console.WriteLine($"   Пропозиція клієнту: оформити {ex.AvailableQuantity} шт. зараз, решту — під замовлення.");
    }
    catch (OutOfStockException ex)
    {
        Console.WriteLine($"   OutOfStockException: {ex.Message} Товару немає зовсім — повідомимо, коли з'явиться.");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"   ArgumentOutOfRangeException: {ex.Message}");
    }
    catch (Exception ex)
    {
        Console.WriteLine($"   {ex.GetType().Name}: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("   finally: запис у журнал замовлень виконано.");
    }
}


Console.WriteLine();
Console.WriteLine("===== Частина 2. Ті самі замовлення через Result<T> =====");
var warehouse2 = new Warehouse(new Dictionary<string, int>
{
    ["Ноутбук"] = 3,
    ["Мишка"] = 10,
    ["Монітор"] = 0
});

foreach (var (product, quantity) in orders)
{
    Result<int> result = warehouse2.TryReserve(product, quantity);
    Console.WriteLine(result.IsSuccess
        ? $"{product} × {quantity}: OK, залишок {result.Value} шт."
        : $"{product} × {quantity}: відмова — {result.Error}");
}


Console.WriteLine();
Console.WriteLine("===== Сценарій 1. Оплата: Retry + exponential backoff + jitter =====");
await ResilienceScenarios.PaymentWithRetryAsync();

Console.WriteLine();
Console.WriteLine("===== Сценарій 2. Статус доставки: Circuit Breaker =====");
await ResilienceScenarios.DeliveryWithCircuitBreakerAsync();

Console.WriteLine();
Console.WriteLine("===== Сценарій 3. Курс валют: Fallback + Retry + Timeout =====");
await ResilienceScenarios.ExchangeRateWithTimeoutAndFallbackAsync();