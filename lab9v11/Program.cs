using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;

namespace lab9v11;


public class CacheEntry
{
    public string Key { get; }
    public string Value { get; set; }

    public CacheEntry(string key, string value)
    {
        Key = key;
        Value = value;
    }

    public override string ToString() => $"{Key} = {Value}";
}


public class SimpleCache
{
    private readonly int _capacity;

 
    private readonly Dictionary<string, CacheEntry> _storage = new();


    private readonly HashSet<string> _keys = new();

 
    private Queue<string> _evictionQueue = new();

   
    private readonly Stack<string> _accessHistory = new();

    public SimpleCache(int capacity)
    {
        _capacity = capacity;
    }

  
    public void Put(string key, string value)
    {
        if (_keys.Contains(key))
        {
            _storage[key].Value = value;
            Touch(key);
            Console.WriteLine($"  [PUT] Ключ '{key}' вже є в кеші -> значення оновлено");
        }
        else
        {
            if (_keys.Count >= _capacity)
            {
                Evict();
            }

            _keys.Add(key);                   
            _storage[key] = new CacheEntry(key, value);
            _evictionQueue.Enqueue(key);          
            Console.WriteLine($"  [PUT] Додано новий ключ '{key}'");
        }

        _accessHistory.Push($"PUT {key}");      
    }

   
    public string? Get(string key)
    {
        if (!_keys.Contains(key))              
        {
            _accessHistory.Push($"MISS {key}");
            Console.WriteLine($"  [GET] Ключ '{key}' НЕ знайдено (cache miss)");
            return null;
        }

        Touch(key);
        _accessHistory.Push($"GET {key}");
        Console.WriteLine($"  [GET] Ключ '{key}' знайдено (cache hit): {_storage[key].Value}");
        return _storage[key].Value;
    }


    private void Touch(string key)
    {
        _evictionQueue = new Queue<string>(_evictionQueue.Where(k => k != key));
        _evictionQueue.Enqueue(key);
    }

    
    private void Evict()
    {
        string oldest = _evictionQueue.Dequeue(); 
        _keys.Remove(oldest);                     
        _storage.Remove(oldest);
        Console.WriteLine($"  [EVICT] Кеш повний -> видалено найдавніший ключ '{oldest}'");
    }

    public void PrintState()
    {
        Console.WriteLine($"  Ключі (HashSet):       {{ {string.Join(", ", _keys)} }}");
        Console.WriteLine($"  Черга видалення (Queue): {string.Join(" -> ", _evictionQueue)}  (перший буде видалений)");
        Console.WriteLine($"  Peek черги:            {(_evictionQueue.Count > 0 ? _evictionQueue.Peek() : "-")}");
    }

   
    public void PrintHistory()
    {
        Console.WriteLine($"  Історія (Stack, зверху вниз): {string.Join(" | ", _accessHistory)}");
    }


    public void PopLastAccess()
    {
        if (_accessHistory.Count == 0)
        {
            Console.WriteLine("  Історія порожня");
            return;
        }

        string last = _accessHistory.Pop();       
        Console.WriteLine($"  [POP] Вилучено з історії: {last}");
    }

    public string? PeekLastAccess() => _accessHistory.Count > 0 ? _accessHistory.Peek() : null;
}

public class Program
{
    public static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;


        Console.WriteLine("=== 1. HashSet<T>: унікальні ключі кешу ===");
        var keys = new HashSet<string>();
        foreach (var k in new[] { "user:1", "user:2", "user:1", "page:home", "user:2" })
        {
            bool added = keys.Add(k);
            Console.WriteLine($"  Add(\"{k}\") -> {(added ? "додано" : "дублікат, ігноровано")}");
        }
        Console.WriteLine($"  Унікальних ключів: {keys.Count}");
        Console.WriteLine($"  Contains(\"page:home\") = {keys.Contains("page:home")}");
        Console.WriteLine($"  Contains(\"page:about\") = {keys.Contains("page:about")}");
        keys.Remove("user:1");
        Console.WriteLine($"  Після Remove(\"user:1\"): {{ {string.Join(", ", keys)} }}");

  
        Console.WriteLine("\n=== 2. Queue<T>: черга на видалення (FIFO) ===");
        var queue = new Queue<string>();
        queue.Enqueue("A");
        queue.Enqueue("B");
        queue.Enqueue("C");
        Console.WriteLine($"  Peek() = {queue.Peek()}");
        while (queue.Count > 0)
        {
            Console.WriteLine($"  Dequeue() = {queue.Dequeue()}");
        }

  
        Console.WriteLine("\n=== 3. Stack<T>: історія звернень (LIFO) ===");
        var stack = new Stack<string>();
        stack.Push("A");
        stack.Push("B");
        stack.Push("C");
        Console.WriteLine($"  Peek() = {stack.Peek()}");
        while (stack.Count > 0)
        {
            Console.WriteLine($"  Pop() = {stack.Pop()}");
        }
        Console.WriteLine("  Висновок: Queue віддає A,B,C (FIFO), Stack віддає C,B,A (LIFO)");

    
        Console.WriteLine("\n=== 4. Система кешування (ємність = 3) ===");
        var cache = new SimpleCache(3);

        cache.Put("a", "1");
        cache.Put("b", "2");
        cache.Put("c", "3");
        cache.PrintState();

        Console.WriteLine("\n  Звертаємось до 'a' (він стає найновішим):");
        cache.Get("a");
        cache.PrintState();

        Console.WriteLine("\n  Додаємо 'd' -> кеш повний, має піти найдавніший ('b'):");
        cache.Put("d", "4");
        cache.PrintState();

        Console.WriteLine("\n  Перевіряємо відсутній ключ:");
        cache.Get("b");

        Console.WriteLine("\n  Оновлюємо існуючий ключ 'c':");
        cache.Put("c", "33");

        Console.WriteLine("\n  Історія звернень:");
        cache.PrintHistory();
        Console.WriteLine($"  Останнє звернення (Peek): {cache.PeekLastAccess()}");
        cache.PopLastAccess();
        cache.PopLastAccess();
        cache.PrintHistory();


        Console.WriteLine("\n=== 5. Продуктивність: List.Contains vs HashSet.Contains ===");
        const int size = 50_000;
        const int lookups = 1_000;
        var list = Enumerable.Range(0, size).ToList();
        var set = new HashSet<int>(list);

        var sw = Stopwatch.StartNew();
        int foundList = 0;
        for (int i = 0; i < lookups; i++)
        {
            if (list.Contains(size - 1 - i)) foundList++; 
        }
        sw.Stop();
        long listMs = sw.ElapsedMilliseconds;

        sw.Restart();
        int foundSet = 0;
        for (int i = 0; i < lookups; i++)
        {
            if (set.Contains(size - 1 - i)) foundSet++;
        }
        sw.Stop();
        long setMs = sw.ElapsedMilliseconds;

        Console.WriteLine($"  Елементів: {size}, пошуків: {lookups}");
        Console.WriteLine($"  List<int>.Contains:    {listMs} мс (знайдено {foundList})");
        Console.WriteLine($"  HashSet<int>.Contains: {setMs} мс (знайдено {foundSet})");
        Console.WriteLine("  Висновок: List шукає за O(n), HashSet в середньому за O(1).");
    }
}