using System.Diagnostics;
using System.Globalization;
using IndependentWork12;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

const int MaxSize = 10_000_000;
int[] sizes = { 10_000, 1_000_000, 5_000_000, 10_000_000 };

Console.WriteLine("===== Середовище =====");
Console.WriteLine($"Логічних процесорів (ядер/потоків): {Environment.ProcessorCount}");
Console.WriteLine($".NET: {Environment.Version}, Release-збірка: {!IsDebugBuild()}");


Console.WriteLine();
Console.WriteLine($"Генерація {MaxSize:N0} випадкових чисел від 1 до 1 000 000...");
var random = new Random(2026); 
var allNumbers = new List<int>(MaxSize);
for (int i = 0; i < MaxSize; i++)
    allNumbers.Add(random.Next(1, 1_000_001));


Console.WriteLine();
Console.WriteLine("===== Частина 1. Продуктивність: LINQ vs PLINQ (найкращий час з 3 запусків) =====");
var total = Stopwatch.StartNew();
PerformanceExperiment.Run(allNumbers, sizes);
total.Stop();
Console.WriteLine($"Експеримент тривав {total.Elapsed.TotalSeconds:F1} с");


Console.WriteLine();
Console.WriteLine("===== Частина 2. Побічні ефекти в PLINQ =====");
SideEffectsDemo.Run(allNumbers);

static bool IsDebugBuild()
{
#if DEBUG
    return true;
#else
    return false;
#endif
}