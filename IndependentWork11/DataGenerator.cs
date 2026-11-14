namespace IndependentWork11;

public static class DataGenerator
{
    private static readonly (string Category, string[] Names, decimal MinPrice, decimal MaxPrice)[] Catalog =
    {
        ("Електроніка", new[] { "Смартфон", "Ноутбук", "Навушники", "Планшет", "Смарт-годинник", "Павербанк", "Монітор", "Клавіатура", "Мишка", "Колонка" }, 300m, 40000m),
        ("Одяг", new[] { "Футболка", "Джинси", "Куртка", "Светр", "Кросівки", "Шапка", "Сукня", "Сорочка", "Шарф", "Рукавички" }, 150m, 4000m),
        ("Продукти", new[] { "Кава", "Чай", "Шоколад", "Сир", "Мед", "Печиво", "Горіхи", "Оливкова олія", "Макарони", "Сік" }, 40m, 600m),
        ("Книги", new[] { "Роман", "Детектив", "Підручник з C#", "Енциклопедія", "Комікс", "Поезія", "Казки", "Біографія", "Атлас", "Словник" }, 100m, 900m),
        ("Спорт", new[] { "М'яч", "Гантелі", "Йога-мат", "Велосипед", "Ракетка", "Скакалка", "Рюкзак", "Пляшка для води", "Еспандер", "Ролики" }, 100m, 15000m),
        ("Дім", new[] { "Лампа", "Подушка", "Ковдра", "Сковорідка", "Чайник", "Рушник", "Ваза", "Свічка", "Килимок", "Настінний годинник" }, 100m, 3000m)
    };

    private static readonly string[] FirstNames =
        { "Олександр", "Марія", "Андрій", "Олена", "Дмитро", "Анна", "Максим", "Ірина", "Іван", "Софія", "Богдан", "Наталія" };

    private static readonly string[] LastNames =
        { "Коваленко", "Бондаренко", "Шевченко", "Мельник", "Ткаченко", "Кравченко", "Олійник", "Поліщук", "Лисенко", "Савчук", "Руденко", "Мороз" };

    private static readonly string[] Cities =
        { "Рівне", "Київ", "Львів", "Луцьк", "Житомир", "Тернопіль", "Хмельницький", "Одеса" };

    public static List<Product> GenerateProducts(int seed = 2026)
    {
        var random = new Random(seed);
        var products = new List<Product>();
        int id = 1;

        foreach (var (category, names, minPrice, maxPrice) in Catalog)
        {
            foreach (string name in names)
            {
                decimal price = minPrice + (maxPrice - minPrice) * (decimal)random.NextDouble();
                products.Add(new Product { Id = id++, Name = name, Category = category, UnitPrice = Math.Round(price, 2) });
            }
        }

        return products;
    }

    public static List<Customer> GenerateCustomers(int count = 80, int seed = 2026)
    {
        var random = new Random(seed);

        return Enumerable.Range(1, count)
            .Select(id => new Customer
            {
                Id = id,
                Name = $"{FirstNames[random.Next(FirstNames.Length)]} {LastNames[random.Next(LastNames.Length)]}",
                City = Cities[random.Next(Cities.Length)]
            })
            .ToList();
    }

    public static List<Sale> GenerateSales(List<Product> products, List<Customer> customers, int count = 1000, int seed = 2026)
    {
        var random = new Random(seed);

        int[] weights = products.Select(_ => random.Next(1, 11)).ToArray();
        int totalWeight = weights.Sum();

        var start = new DateTime(2025, 1, 1);
        int days = (new DateTime(2026, 6, 30) - start).Days + 1; 

        var sales = new List<Sale>(count);
        for (int id = 1; id <= count; id++)
        {
            sales.Add(new Sale
            {
                Id = id,
                ProductId = products[PickWeighted(random, weights, totalWeight)].Id,
                CustomerId = customers[random.Next(customers.Count)].Id,
                Quantity = random.Next(1, 11),
                SaleDate = start.AddDays(random.Next(days))
            });
        }

        return sales;
    }

    private static int PickWeighted(Random random, int[] weights, int totalWeight)
    {
        int value = random.Next(totalWeight);
        for (int i = 0; i < weights.Length; i++)
        {
            value -= weights[i];
            if (value < 0)
                return i;
        }
        return weights.Length - 1;
    }
}