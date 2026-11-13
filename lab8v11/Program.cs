Console.OutputEncoding = System.Text.Encoding.UTF8;


var numbers = new List<int> { 5, -3, 8, -1, 0, 12, -7, 4 };
Console.WriteLine("Числа: " + string.Join(", ", numbers));

int negativeCount = CollectionHelper<int>.Count(numbers, n => n < 0);
Console.WriteLine($"Кількість від'ємних чисел: {negativeCount}");

int evenCount = CollectionHelper<int>.Count(numbers, n => n % 2 == 0);
Console.WriteLine($"Кількість парних чисел: {evenCount}");

Console.WriteLine();


var words = new List<string> { "Apple", "Banana", "Avocado", "Cherry", "Apricot" };
Console.WriteLine("Рядки: " + string.Join(", ", words));

int startsWithA = CollectionHelper<string>.Count(words, w => w.StartsWith("A"));
Console.WriteLine($"Кількість рядків, що починаються на 'A': {startsWithA}");

Console.WriteLine();


var products = new List<Product>
{
    new Product(1, "Молоко", 45m, "Їжа"),
    new Product(2, "Ноутбук", 25000m, "Техніка"),
    new Product(3, "Хліб", 25m, "Їжа"),
    new Product(4, "Телефон", 15000m, "Техніка"),
    new Product(5, "Сир", 180m, "Їжа")
};
Console.WriteLine("Продукти:");
foreach (var p in products)
    Console.WriteLine("  " + p);

int foodCount = CollectionHelper<Product>.Count(products, p => p.Category == "Їжа");
Console.WriteLine($"Кількість продуктів категорії 'Їжа': {foodCount}");

int expensiveCount = CollectionHelper<Product>.Count(products, p => p.Price > 100);
Console.WriteLine($"Кількість продуктів дорожче 100 грн: {expensiveCount}");


var empty = new List<int>();
Console.WriteLine($"\nПорожня колекція, кількість додатних: {CollectionHelper<int>.Count(empty, n => n > 0)}");