using Polly;
using Polly.CircuitBreaker;
using Polly.Fallback;
using Polly.Retry;
using Polly.Timeout;

namespace IndependentWork19;


public static class ResilienceScenarios
{
 
    public static async Task PaymentWithRetryAsync()
    {
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddRetry(new RetryStrategyOptions
            {
                
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                MaxRetryAttempts = 3,
                Delay = TimeSpan.FromMilliseconds(500),
                BackoffType = DelayBackoffType.Exponential, 
                UseJitter = true,                            
                OnRetry = args =>
                {
                    Log.Write($"   Retry #{args.AttemptNumber + 1} через {args.RetryDelay.TotalSeconds:F2} с. Причина: {args.Outcome.Exception?.Message}");
                    return default;
                }
            })
            .Build();

        Log.Restart();
        Log.Write("1а. Шлюз двічі відповідає 503, на третій спробі працює");
        await RunPaymentAsync(pipeline, new PaymentGateway(failuresBeforeSuccess: 2), 1500m, "order-1001");

        Console.WriteLine();
        Log.Restart();
        Log.Write("1б. Шлюз недоступний довго (усі 4 спроби невдалі)");
        await RunPaymentAsync(pipeline, new PaymentGateway(failuresBeforeSuccess: 100), 820m, "order-1002");

        Console.WriteLine();
        Log.Restart();
        Log.Write("1в. Некоректна сума — постійна помилка, Retry не застосовується");
        await RunPaymentAsync(pipeline, new PaymentGateway(failuresBeforeSuccess: 0), -5m, "order-1003");
    }

    private static async Task RunPaymentAsync(ResiliencePipeline pipeline, PaymentGateway gateway, decimal amount, string orderKey)
    {
        try
        {
            string transaction = await pipeline.ExecuteAsync(
                async token => await gateway.ChargeAsync(amount, orderKey, token),
                CancellationToken.None);

            Log.Write($"   УСПІХ: транзакція {transaction}, викликів шлюзу: {gateway.Calls}");
        }
        catch (HttpRequestException ex)
        {
            Log.Write($"   ОСТАТОЧНА НЕВДАЧА після {gateway.Calls} викликів: {ex.Message}");
        }
        catch (ArgumentException ex)
        {
            Log.Write($"   ПОМИЛКА ВАЛІДАЦІЇ, повторів не було (викликів шлюзу: {gateway.Calls}): {ex.Message}");
        }
    }


    public static async Task DeliveryWithCircuitBreakerAsync()
    {
        ResiliencePipeline pipeline = new ResiliencePipelineBuilder()
            .AddCircuitBreaker(new CircuitBreakerStrategyOptions
            {
                ShouldHandle = new PredicateBuilder().Handle<HttpRequestException>(),
                FailureRatio = 0.5,                         
                MinimumThroughput = 3,                       
                SamplingDuration = TimeSpan.FromSeconds(5),  
                BreakDuration = TimeSpan.FromSeconds(1.5),   
                OnOpened = args =>
                {
                    Log.Write($"   >>> Circuit OPEN на {args.BreakDuration.TotalSeconds:F1} с: запити більше не йдуть до сервісу");
                    return default;
                },
                OnHalfOpened = args =>
                {
                    Log.Write("   >>> Circuit HALF-OPEN: пропускаємо пробний запит");
                    return default;
                },
                OnClosed = args =>
                {
                    Log.Write("   >>> Circuit CLOSED: сервіс відновився, робота в нормальному режимі");
                    return default;
                }
            })
            .Build();

        Log.Restart();
        Log.Write("Сервіс доставки недоступний перші 2 с. Клієнт запитує статус кожні 0,4 с");

        var service = new DeliveryService(downFor: TimeSpan.FromSeconds(2));
        const int Requests = 10;

        for (int request = 1; request <= Requests; request++)
        {
            int orderId = 5000 + request;
            try
            {
                string status = await pipeline.ExecuteAsync(
                    async token => await service.GetStatusAsync(orderId, token),
                    CancellationToken.None);

                Log.Write($"Запит {request,2}: OK — {status}");
            }
            catch (BrokenCircuitException)
            {
                Log.Write($"Запит {request,2}: ВІДХИЛЕНО миттєво (ланцюг розімкнений), сервіс не викликався");
            }
            catch (HttpRequestException ex)
            {
                Log.Write($"Запит {request,2}: ПОМИЛКА — {ex.Message}");
            }

            await Task.Delay(400);
        }

        Log.Write($"Підсумок: {Requests} запитів клієнта, реальних викликів сервісу: {service.Calls}");
    }

   
    private const decimal CachedRate = 41.10m; 
    public static async Task ExchangeRateWithTimeoutAndFallbackAsync()
    {
        
        ResiliencePipeline<decimal> pipeline = new ResiliencePipelineBuilder<decimal>()
            .AddFallback(new FallbackStrategyOptions<decimal>
            {
                ShouldHandle = new PredicateBuilder<decimal>()
                    .Handle<TimeoutRejectedException>()
                    .Handle<HttpRequestException>(),
                FallbackAction = args => Outcome.FromResultAsValueTask(CachedRate),
                OnFallback = args =>
                {
                    Log.Write($"   FALLBACK: сервіс не відповів ({args.Outcome.Exception?.GetType().Name}), беремо кешований курс {CachedRate}");
                    return default;
                }
            })
            .AddRetry(new RetryStrategyOptions<decimal>
            {
                ShouldHandle = new PredicateBuilder<decimal>().Handle<TimeoutRejectedException>(),
                MaxRetryAttempts = 2,
                Delay = TimeSpan.FromMilliseconds(200),
                BackoffType = DelayBackoffType.Constant,
                OnRetry = args =>
                {
                    Log.Write($"   Retry #{args.AttemptNumber + 1} через {args.RetryDelay.TotalSeconds:F1} с");
                    return default;
                }
            })
            .AddTimeout(new TimeoutStrategyOptions
            {
                Timeout = TimeSpan.FromMilliseconds(500),
                OnTimeout = args =>
                {
                    Log.Write($"   TIMEOUT: немає відповіді за {args.Timeout.TotalMilliseconds} мс, запит скасовано");
                    return default;
                }
            })
            .Build();

        var service = new ExchangeRateService();

        Log.Restart();
        Log.Write("3а. Сервіс курсу працює нормально (відповідь за 100 мс)");
        service.ResponseTime = TimeSpan.FromMilliseconds(100);
        decimal rate = await pipeline.ExecuteAsync(async token => await service.GetUsdRateAsync(token), CancellationToken.None);
        Log.Write($"   Курс USD: {rate} грн (актуальний)");

        Console.WriteLine();
        Log.Restart();
        Log.Write("3б. Сервіс курсу «зависає» (відповідь за 2000 мс)");
        service.ResponseTime = TimeSpan.FromSeconds(2);
        rate = await pipeline.ExecuteAsync(async token => await service.GetUsdRateAsync(token), CancellationToken.None);
        Log.Write($"   Курс USD: {rate} грн (кешований). Користувач отримав відповідь замість помилки");
    }
}