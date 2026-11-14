using System.Diagnostics;

namespace IndependentWork12;


public static class PerformanceExperiment
{
    private const int Runs = 3; 

    public static void Run(List<int> allNumbers, int[] sizes)
    {
        
        List<int> warmUp = allNumbers.GetRange(0, 10_000);
        Workloads.SumOfPrimesLinq(warmUp);
        Workloads.SumOfPrimesPlinq(warmUp);
        Workloads.SumOfEvenDoubledLinq(warmUp);
        Workloads.SumOfEvenDoubledPlinq(warmUp);

        PrintHeader();

        foreach (int size in sizes)
        {
            List<int> numbers = allNumbers.GetRange(0, size);
            Compare("Прості числа (важка)", numbers, Workloads.SumOfPrimesLinq, Workloads.SumOfPrimesPlinq);
        }

        Console.WriteLine(new string('-', 98));

        foreach (int size in sizes)
        {
            List<int> numbers = allNumbers.GetRange(0, size);
            Compare("Парні × 2 (легка)", numbers, Workloads.SumOfEvenDoubledLinq, Workloads.SumOfEvenDoubledPlinq);
        }
    }

    private static void PrintHeader()
    {
        Console.WriteLine($"{"Операція",-22} | {"Елементів",12} | {"LINQ, мс",10} | {"PLINQ, мс",10} | {"Прискорення",11} | Результат");
        Console.WriteLine(new string('-', 98));
    }

    private static void Compare(string name, List<int> numbers, Func<List<int>, long> linq, Func<List<int>, long> plinq)
    {
        var (linqResult, linqMs) = Measure(() => linq(numbers));
        var (plinqResult, plinqMs) = Measure(() => plinq(numbers));

        double speedup = linqMs / plinqMs;
        string check = linqResult == plinqResult ? $"однаковий ({linqResult:N0})" : "РІЗНИЙ!";

        Console.WriteLine($"{name,-22} | {numbers.Count,12:N0} | {linqMs,10:F1} | {plinqMs,10:F1} | {speedup,10:F2}x | {check}");
    }

   
    private static (long Result, double Ms) Measure(Func<long> action)
    {
        long result = 0;
        double bestMs = double.MaxValue;

        for (int i = 0; i < Runs; i++)
        {
            var sw = Stopwatch.StartNew();
            result = action();
            sw.Stop();
            bestMs = Math.Min(bestMs, sw.Elapsed.TotalMilliseconds);
        }

        return (result, bestMs);
    }
}