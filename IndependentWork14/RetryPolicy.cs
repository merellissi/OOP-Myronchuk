namespace IndependentWork14;


public class RetryPolicy
{
    private readonly int _maxRetries;
    private readonly TimeSpan _initialDelay;
    private readonly double _jitterFactor;
    private readonly Random _random = new();

 
    public RetryPolicy(int maxRetries, TimeSpan initialDelay, double jitterFactor = 0.2)
    {
        if (maxRetries < 0)
            throw new ArgumentOutOfRangeException(nameof(maxRetries), "Кількість повторів не може бути від'ємною.");
        if (initialDelay < TimeSpan.Zero)
            throw new ArgumentOutOfRangeException(nameof(initialDelay), "Затримка не може бути від'ємною.");
        if (jitterFactor < 0 || jitterFactor > 1)
            throw new ArgumentOutOfRangeException(nameof(jitterFactor), "jitterFactor має бути в межах від 0 до 1.");

        _maxRetries = maxRetries;
        _initialDelay = initialDelay;
        _jitterFactor = jitterFactor;
    }

    
    public T Execute<T>(Func<T> operation, Func<Exception, bool>? shouldRetry = null)
    {
        ArgumentNullException.ThrowIfNull(operation);

        int totalAttempts = _maxRetries + 1;

        for (int retryAttempt = 0; ; retryAttempt++)
        {
            int attemptNumber = retryAttempt + 1;
            try
            {
                Console.WriteLine($"   Спроба {attemptNumber}/{totalAttempts}...");
                T result = operation();
                Console.WriteLine($"   Спроба {attemptNumber}: УСПІХ");
                return result;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"   Спроба {attemptNumber}: НЕВДАЧА — {ex.GetType().Name}: {ex.Message}");

                bool canRetry = shouldRetry?.Invoke(ex) ?? true;
                if (!canRetry)
                {
                    Console.WriteLine($"   {ex.GetType().Name} не підлягає повтору (shouldRetry = false). Прокидаємо виняток.");
                    throw; 
                }

                if (retryAttempt >= _maxRetries)
                {
                    Console.WriteLine($"   Вичерпано всі {totalAttempts} спроби. Прокидаємо останній виняток.");
                    throw;
                }

                TimeSpan delay = CalculateDelay(retryAttempt, out double jitterMultiplier);
                Console.WriteLine($"   Очікування {delay.TotalMilliseconds:F0} мс " +
                                  $"(база {BaseDelay(retryAttempt).TotalMilliseconds:F0} мс, jitter {(jitterMultiplier - 1) * 100:+0;-0;0}%) " +
                                  $"перед повтором #{retryAttempt + 1}");
                Thread.Sleep(delay);
            }
        }
    }

    
    public TimeSpan BaseDelay(int retryAttempt) =>
        TimeSpan.FromMilliseconds(_initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt));

   
    public TimeSpan CalculateDelay(int retryAttempt) => CalculateDelay(retryAttempt, out _);

    private TimeSpan CalculateDelay(int retryAttempt, out double jitterMultiplier)
    {
        
        jitterMultiplier = 1 + _random.NextDouble() * _jitterFactor * 2 - _jitterFactor;

        double delayMs = _initialDelay.TotalMilliseconds * Math.Pow(2, retryAttempt) * jitterMultiplier;
        return TimeSpan.FromMilliseconds(delayMs);
    }
}