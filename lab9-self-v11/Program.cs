using Lab9Self;

Console.OutputEncoding = System.Text.Encoding.UTF8;


Console.WriteLine("===== Колекція 1: List<int> =====");
var numbers = new List<int> { 5, 12, 8, 3, 20, 15, 7 };
Console.WriteLine("Числа: " + string.Join(", ", numbers));


var squares = CollectionUtils.Map(numbers, n => n * n);
Console.WriteLine("Map (квадрати): " + string.Join(", ", squares));

Predicate<int> isEven = n => n % 2 == 0;
var evens = CollectionUtils.Filter(numbers, isEven);
Console.WriteLine("Filter (парні): " + string.Join(", ", evens));


Console.WriteLine("MaxBy (найбільше): " + CollectionUtils.MaxBy(numbers, n => n));
Console.WriteLine("MaxBy (найбільша остання цифра): " + CollectionUtils.MaxBy(numbers, n => n % 10));


Console.WriteLine("ForEach:");
CollectionUtils.ForEach(numbers, n => Console.WriteLine($"  {n} — {(isEven(n) ? "парне" : "непарне")}"));


Console.WriteLine();
Console.WriteLine("===== Колекція 2: List<string> =====");
var fruits = new List<string> { "apple", "banana", "kiwi", "cherry", "watermelon", "fig" };
Console.WriteLine("Фрукти: " + string.Join(", ", fruits));

Console.WriteLine("Map (великими літерами): " + string.Join(", ", CollectionUtils.Map(fruits, f => f.ToUpper())));
Console.WriteLine("Map (довжини слів): " + string.Join(", ", CollectionUtils.Map(fruits, f => f.Length)));
Console.WriteLine("Filter (довші за 4 літери): " + string.Join(", ", CollectionUtils.Filter(fruits, f => f.Length > 4)));
Console.WriteLine("Filter (містять 'a'): " + string.Join(", ", CollectionUtils.Filter(fruits, f => f.Contains('a'))));
Console.WriteLine("MaxBy (найдовше слово): " + CollectionUtils.MaxBy(fruits, f => f.Length));
Console.WriteLine("MaxBy (останнє за алфавітом): " + CollectionUtils.MaxBy(fruits, f => f));

Console.WriteLine("ForEach (нумерований список):");
int index = 1;
CollectionUtils.ForEach(fruits, f => Console.WriteLine($"  {index++}. {f}"));


Console.WriteLine();
Console.WriteLine("===== Колекція 3: List<Product> =====");
var products = new List<Product>
{
    new Product { Name = "Laptop", Price = 25000, Category = "Комп'ютери" },
    new Product { Name = "Mouse", Price = 500, Category = "Аксесуари" },
    new Product { Name = "Keyboard", Price = 1500, Category = "Аксесуари" },
    new Product { Name = "Monitor", Price = 8000, Category = "Комп'ютери" },
    new Product { Name = "USB cable", Price = 150, Category = "Аксесуари" }
};

Console.WriteLine("ForEach (усі товари):");
CollectionUtils.ForEach(products, p => Console.WriteLine($"  {p.Name}: {p.Price} грн ({p.Category})"));

var names = CollectionUtils.Map(products, p => p.Name);
Console.WriteLine("Map (назви): " + string.Join(", ", names));

var withDiscount = CollectionUtils.Map(products, p => $"{p.Name} зі знижкою 10% = {p.Price * 0.9m} грн");
Console.WriteLine("Map (ціни зі знижкою):");
CollectionUtils.ForEach(withDiscount, s => Console.WriteLine("  " + s));

var mostExpensive = CollectionUtils.MaxBy(products, p => p.Price);
Console.WriteLine($"MaxBy (найдорожчий): {mostExpensive.Name} — {mostExpensive.Price} грн");

var longestName = CollectionUtils.MaxBy(products, p => p.Name.Length);
Console.WriteLine($"MaxBy (найдовша назва): {longestName.Name}");

var expensive = CollectionUtils.Filter(products, p => p.Price > 1000);
Console.WriteLine("Filter (дорожчі за 1000 грн):");
CollectionUtils.ForEach(expensive, p => Console.WriteLine($"  - {p.Name}"));

var accessories = CollectionUtils.Filter(products, p => p.Category == "Аксесуари");
Console.WriteLine("Filter (категорія «Аксесуари»): " + string.Join(", ", CollectionUtils.Map(accessories, p => p.Name)));


Console.WriteLine();
Console.WriteLine("===== Порівняння з LINQ =====");
Console.WriteLine("Map    : " + string.Join(", ", CollectionUtils.Map(numbers, n => n * 2)));
Console.WriteLine("Select : " + string.Join(", ", numbers.Select(n => n * 2)));
Console.WriteLine("Filter : " + string.Join(", ", CollectionUtils.Filter(numbers, n => n > 7)));
Console.WriteLine("Where  : " + string.Join(", ", numbers.Where(n => n > 7)));
Console.WriteLine("MaxBy (наш) : " + CollectionUtils.MaxBy(products, p => p.Price).Name);
Console.WriteLine("MaxBy (LINQ): " + products.MaxBy(p => p.Price)!.Name);


try
{
    CollectionUtils.MaxBy(new List<int>(), n => n);
}
catch (InvalidOperationException ex)
{
    Console.WriteLine($"MaxBy для порожнього списку -> InvalidOperationException: {ex.Message}");
}