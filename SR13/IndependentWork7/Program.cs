using IndependentWork7;

Console.OutputEncoding = System.Text.Encoding.UTF8;

var cache = new Cache<string, string>(3);

Console.WriteLine("=== Додаємо 3 елементи (ємність = 3) ===");
cache.Add("ua", "Україна");
cache.Add("pl", "Польща");
cache.Add("de", "Німеччина");
Console.WriteLine($"Count = {cache.Count}");
Console.WriteLine($"Get(\"pl\") = {cache.Get("pl")}");

Console.WriteLine();
Console.WriteLine("=== Додаємо 4-й елемент -> найстаріший (\"ua\") має видалитися ===");
cache.Add("fr", "Франція");
Console.WriteLine($"Count = {cache.Count}");
Console.WriteLine($"ContainsKey(\"ua\") = {cache.ContainsKey("ua")}");
Console.WriteLine($"ContainsKey(\"fr\") = {cache.ContainsKey("fr")}");

Console.WriteLine();
Console.WriteLine("=== Пробуємо отримати видалений ключ ===");
try
{
    Console.WriteLine(cache.Get("ua"));
}
catch (KeyNotFoundException ex)
{
    Console.WriteLine($"KeyNotFoundException: {ex.Message}");
}