using System.Diagnostics;
using System.Globalization;
using IndependentWork11;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");


List<Product> products = DataGenerator.GenerateProducts();
List<Customer> customers = DataGenerator.GenerateCustomers(80);
List<Sale> sales = DataGenerator.GenerateSales(products, customers, 1000);

Console.WriteLine("===== Дані =====");
Console.WriteLine($"Товарів: {products.Count}, клієнтів: {customers.Count}, продажів: {sales.Count}");
Console.WriteLine($"Період продажів: {sales.Min(s => s.SaleDate):dd.MM.yyyy} – {sales.Max(s => s.SaleDate):dd.MM.yyyy}");

const int Repeats = 100;
var timings = new List<(string Name, double FirstMs, double AvgMs)>();


var (topProducts, first1, avg1) = Measure(() => SalesPipelines.TopProductsByCategory(sales, products));
timings.Add(("1. Топ-3 продукти за категоріями", first1, avg1));

Console.WriteLine();
Console.WriteLine("===== Пайплайн 1. Найпопулярніші продукти за категоріями (топ-3) =====");
foreach (CategoryTopProducts category in topProducts)
{
    Console.WriteLine(category.Category);
    int place = 1;
    foreach (ProductSales product in category.TopProducts)
        Console.WriteLine($"   {place++}. {product.Name,-20} {product.TotalQuantity,5} шт.");
}
PrintTime(first1, avg1);


var (cityRevenue, first2, avg2) = Measure(() => SalesPipelines.RevenueByCity(sales, products, customers));
timings.Add(("2. Дохід за містами", first2, avg2));

Console.WriteLine();
Console.WriteLine("===== Пайплайн 2. Загальний дохід за містами клієнтів =====");
Console.WriteLine($"{"Місто",-14} {"Продажів",9} {"Дохід, грн",18}");
foreach (CityRevenue city in cityRevenue)
    Console.WriteLine($"{city.City,-14} {city.SalesCount,9} {city.Revenue,18:N2}");
Console.WriteLine($"{"Разом",-14} {cityRevenue.Sum(c => c.SalesCount),9} {cityRevenue.Sum(c => c.Revenue),18:N2}");
PrintTime(first2, avg2);


var (monthly, first3, avg3) = Measure(() => SalesPipelines.SalesByMonth(sales, products));
timings.Add(("3. Продажі за місяцями", first3, avg3));

Console.WriteLine();
Console.WriteLine("===== Пайплайн 3. Продажі за місяцями та роками =====");
Console.WriteLine($"{"Рік-місяць",-10} {"Продажів",9} {"Одиниць",8} {"Дохід, грн",18}");
foreach (MonthlySales month in monthly)
    Console.WriteLine($"{month.Year}-{month.Month:D2}    {month.SalesCount,9} {month.Units,8} {month.Revenue,18:N2}");
Console.WriteLine($"{"Разом",-10} {monthly.Sum(m => m.SalesCount),9} {monthly.Sum(m => m.Units),8} {monthly.Sum(m => m.Revenue),18:N2}");
PrintTime(first3, avg3);

var (topCustomers, first4, avg4) = Measure(() => SalesPipelines.TopCustomers(sales, products, customers));
timings.Add(("4. Топ-5 клієнтів", first4, avg4));

Console.WriteLine();
Console.WriteLine("===== Пайплайн 4. Топ-5 клієнтів за сумою покупок =====");
int rank = 1;
foreach (CustomerStats c in topCustomers)
{
    Console.WriteLine($"{rank++}. {c.Name} ({c.City})");
    Console.WriteLine($"   покупок: {c.Purchases}, сума: {c.TotalSpent:N2} грн, середній чек: {c.AverageCheck:N2} грн, улюблена категорія: {c.FavoriteCategory}");
}
PrintTime(first4, avg4);


Console.WriteLine();
Console.WriteLine("===== Перевірка =====");
decimal totalByCity = cityRevenue.Sum(c => c.Revenue);
decimal totalByMonth = monthly.Sum(m => m.Revenue);
Console.WriteLine($"Дохід за містами = дохід за місяцями: {totalByCity:N2} = {totalByMonth:N2} -> {(totalByCity == totalByMonth ? "OK" : "ПОМИЛКА")}");
Console.WriteLine($"Усього продано одиниць: {sales.Sum(s => s.Quantity)} = {monthly.Sum(m => m.Units)} -> {(sales.Sum(s => s.Quantity) == monthly.Sum(m => m.Units) ? "OK" : "ПОМИЛКА")}");

Console.WriteLine();
Console.WriteLine($"===== Підсумок вимірювань (Stopwatch, середнє з {Repeats} повторів) =====");
Console.WriteLine($"{"Пайплайн",-34} {"Перший запуск, мс",18} {"Середній, мс",14}");
foreach (var (name, firstMs, avgMs) in timings)
    Console.WriteLine($"{name,-34} {firstMs,18:F3} {avgMs,14:F3}");


static (T Result, double FirstMs, double AvgMs) Measure<T>(Func<T> pipeline)
{
    var firstWatch = Stopwatch.StartNew();
    T result = pipeline();
    firstWatch.Stop();

    var warmWatch = Stopwatch.StartNew();
    for (int i = 0; i < Repeats; i++)
        pipeline();
    warmWatch.Stop();

    return (result, firstWatch.Elapsed.TotalMilliseconds, warmWatch.Elapsed.TotalMilliseconds / Repeats);
}

static void PrintTime(double firstMs, double avgMs)
{
    Console.WriteLine($"Час: перший запуск {firstMs:F3} мс, середній {avgMs:F3} мс");
}