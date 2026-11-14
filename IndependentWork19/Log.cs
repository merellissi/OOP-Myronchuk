using System.Diagnostics;

namespace IndependentWork19;


public static class Log
{
    private static readonly Stopwatch Clock = Stopwatch.StartNew();

    public static void Restart() => Clock.Restart();

    public static void Write(string message) =>
        Console.WriteLine($"[{Clock.Elapsed.TotalSeconds,5:F2} с] {message}");
}