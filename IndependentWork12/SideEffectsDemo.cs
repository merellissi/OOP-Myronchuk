using System.Collections.Concurrent;
using System.Diagnostics;

namespace IndependentWork12;

public static class SideEffectsDemo
{
    private const int Attempts = 5;

    public static void Run(List<int> numbers)
    {
        int expected = numbers.Count(n => n % 2 == 0);
        Console.WriteLine($"Елементів: {numbers.Count:N0}. Правильна кількість парних (звичайний LINQ): {expected:N0}");

        
        Console.WriteLine();
        Console.WriteLine("--- Проблема 1: counter++ зі спільною змінною всередині PLINQ ---");

        for (int attempt = 1; attempt <= Attempts; attempt++)
        {
            int counter = 0;

            
            numbers.AsParallel().ForAll(n =>
            {
                if (n % 2 == 0)
                    counter++;
            });

            int lost = expected - counter;
            Console.WriteLine($"Спроба {attempt}: counter = {counter,12:N0}  -> {(lost == 0 ? "випадково правильно" : $"втрачено {lost:N0}")}");
        }

       
        Console.WriteLine();
        Console.WriteLine("--- Виправлення 1 ---");

   
        var sync = new object();
        int lockCounter = 0;
        var sw = Stopwatch.StartNew();
        numbers.AsParallel().ForAll(n =>
        {
            if (n % 2 == 0)
            {
                lock (sync)
                {
                    lockCounter++;
                }
            }
        });
        sw.Stop();
        PrintFix("lock", lockCounter, expected, sw.Elapsed.TotalMilliseconds);

      
        int interlockedCounter = 0;
        sw = Stopwatch.StartNew();
        numbers.AsParallel().ForAll(n =>
        {
            if (n % 2 == 0)
                Interlocked.Increment(ref interlockedCounter);
        });
        sw.Stop();
        PrintFix("Interlocked.Increment", interlockedCounter, expected, sw.Elapsed.TotalMilliseconds);

       
        sw = Stopwatch.StartNew();
        int pureCount = numbers.AsParallel().Count(n => n % 2 == 0);
        sw.Stop();
        PrintFix("AsParallel().Count() без спільних змінних", pureCount, expected, sw.Elapsed.TotalMilliseconds);

        
        Console.WriteLine();
        Console.WriteLine("--- Проблема 2: List<int>.Add всередині PLINQ ---");

        for (int attempt = 1; attempt <= Attempts; attempt++)
        {
            var unsafeList = new List<int>();
            try
            {
                
                numbers.AsParallel().ForAll(n =>
                {
                    if (n % 2 == 0)
                        unsafeList.Add(n);
                });

                int lost = expected - unsafeList.Count;
                Console.WriteLine($"Спроба {attempt}: у списку {unsafeList.Count,12:N0}  -> {(lost == 0 ? "випадково правильно" : $"втрачено {lost:N0}")}");
            }
            catch (AggregateException ex)
            {
                Exception inner = ex.InnerExceptions[0];
                Console.WriteLine($"Спроба {attempt}: виняток {inner.GetType().Name}: {inner.Message}");
            }
        }

      
        Console.WriteLine();
        Console.WriteLine("--- Виправлення 2 ---");

        
        var bag = new ConcurrentBag<int>();
        sw = Stopwatch.StartNew();
        numbers.AsParallel().ForAll(n =>
        {
            if (n % 2 == 0)
                bag.Add(n);
        });
        sw.Stop();
        PrintFix("ConcurrentBag<int>", bag.Count, expected, sw.Elapsed.TotalMilliseconds);

       
        sw = Stopwatch.StartNew();
        List<int> evens = numbers.AsParallel().Where(n => n % 2 == 0).ToList();
        sw.Stop();
        PrintFix("AsParallel().Where().ToList()", evens.Count, expected, sw.Elapsed.TotalMilliseconds);
    }

    private static void PrintFix(string method, int actual, int expected, double ms)
    {
        string status = actual == expected ? "OK" : "ПОМИЛКА";
        Console.WriteLine($"{method,-42}: {actual,12:N0}  -> {status}, {ms,8:F1} мс");
    }
}