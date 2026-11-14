using System.Diagnostics;

namespace Lab12v11;

public static class RetryHelper
{
    private static readonly TimeSpan DefaultInitialDelay = TimeSpan.FromMilliseconds(500);

    public static T ExecuteWithRetry<T>(
        Func<T> operation,
        int retryCount = 3,
        TimeSpan initialDelay = default,
        Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);
        if (retryCount < 0)
            throw new ArgumentOutOfRangeException(nameof(retryCount), "Кількість повторів не може бути від'ємною.");

        if (initialDelay == default)
            initialDelay = DefaultInitialDelay;

        int totalAttempts = retryCount + 1;
        var clock = Stopwatch.StartNew();

        for (int retryAttempt = 0; ; retryAttempt++)
        {
            int attempt = retryAttempt + 1;
            try
            {
                Log(clock, $"Спроба {attempt}/{totalAttempts}...");
                T result = operation();
                Log(clock, $"Спроба {attempt}: УСПІХ");
                return result;
            }
            catch (Exception ex)
            {
                Log(clock, $"Спроба {attempt}: НЕВДАЧА — {ex.GetType().Name}: {ex.Message}");

                if (shouldRetry is not null && !shouldRetry(ex))
                {
                    Log(clock, $"{ex.GetType().Name} не підлягає повтору (shouldRetry = false)");
                    throw;
                }

                if (retryAttempt >= retryCount)
                {
                    Log(clock, $"Вичерпано всі {totalAttempts} спроби");
                    throw;
                }

                TimeSpan delay = TimeSpan.FromMilliseconds(initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt));
                Log(clock, $"Очікування {delay.TotalMilliseconds:F0} мс ({initialDelay.TotalMilliseconds:F0} × 2^{retryAttempt}) перед повтором #{attempt}");
                Thread.Sleep(delay);
            }
        }
    }

    private static void Log(Stopwatch clock, string message) =>
        Console.WriteLine($"   [{clock.Elapsed.TotalSeconds,5:F2} с] {message}");
}