using System.Diagnostics;

namespace IndependentWork19;


public class PaymentGateway
{
    private readonly int _failuresBeforeSuccess;

    public PaymentGateway(int failuresBeforeSuccess)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int Calls { get; private set; }

    public async Task<string> ChargeAsync(decimal amount, string idempotencyKey, CancellationToken token)
    {
        if (amount <= 0)
            throw new ArgumentException("Сума платежу має бути додатною.", nameof(amount));

        Calls++;
        Log.Write($"   -> виклик шлюзу #{Calls}: списати {amount} грн (ключ {idempotencyKey})");
        await Task.Delay(50, token);

        if (Calls <= _failuresBeforeSuccess)
            throw new HttpRequestException("503 Service Unavailable: платіжний шлюз перевантажений");

        return $"TX-{idempotencyKey}-OK";
    }
}

public class DeliveryService
{
    private readonly Stopwatch _clock = Stopwatch.StartNew();
    private readonly TimeSpan _downFor;

    public DeliveryService(TimeSpan downFor)
    {
        _downFor = downFor;
    }

    public int Calls { get; private set; }

    public async Task<string> GetStatusAsync(int orderId, CancellationToken token)
    {
        Calls++;
        await Task.Delay(20, token);

        if (_clock.Elapsed < _downFor)
            throw new HttpRequestException("сервіс доставки недоступний (connection refused)");

        return $"замовлення #{orderId} в дорозі";
    }
}


public class ExchangeRateService
{
    public TimeSpan ResponseTime { get; set; } = TimeSpan.FromMilliseconds(100);

    public int Calls { get; private set; }

    public async Task<decimal> GetUsdRateAsync(CancellationToken token)
    {
        Calls++;
        Log.Write($"   -> виклик сервісу курсу #{Calls} (відповідає за {ResponseTime.TotalMilliseconds} мс)");
        await Task.Delay(ResponseTime, token); 
        return 41.35m;
    }
}