namespace Lab13v11;

public class Transaction
{
    public string Id { get; init; } = string.Empty;
    public decimal Amount { get; init; }
    public DateTime CreatedAt { get; init; }
    public bool IsRefunded { get; set; }
}

public class PaymentService
{
    private const decimal GatewayLimit = 50_000m;

    private readonly string _gatewayName;
    private readonly bool _isGatewayAvailable;
    private readonly Dictionary<string, Transaction> _transactions = new();
    private int _nextTransactionNumber = 1;

    public PaymentService(string gatewayName, decimal cardBalance, bool isGatewayAvailable = true)
    {
        _gatewayName = gatewayName;
        CardBalance = cardBalance;
        _isGatewayAvailable = isGatewayAvailable;
    }

    public decimal CardBalance { get; private set; }

    public string? LastTransactionId { get; private set; }

    public void ProcessPayment(decimal amount)
    {
        if (amount <= 0)
            throw new ArgumentOutOfRangeException(nameof(amount), "Сума платежу має бути додатною.");

        if (!_isGatewayAvailable)
            throw new PaymentGatewayException(
                $"Шлюз {_gatewayName} не відповідає.", _gatewayName, "GATEWAY_UNAVAILABLE",
                new TimeoutException("Немає відповіді за 30 с"));

        if (amount > GatewayLimit)
            throw new PaymentGatewayException(
                $"Сума {amount:N2} грн перевищує ліміт шлюзу {GatewayLimit:N0} грн.", _gatewayName, "LIMIT_EXCEEDED");

        if (amount > CardBalance)
            throw new PaymentGatewayException(
                $"Банк відхилив платіж: на картці {CardBalance:N2} грн, потрібно {amount:N2} грн.", _gatewayName, "INSUFFICIENT_FUNDS");

        string id = $"TX-{_nextTransactionNumber++:D4}";
        _transactions[id] = new Transaction { Id = id, Amount = amount, CreatedAt = DateTime.Now };
        CardBalance -= amount;
        LastTransactionId = id;
    }

    public Result<bool> RefundPayment(string transactionId)
    {
        if (string.IsNullOrWhiteSpace(transactionId))
            return Result<bool>.Failure("Не вказано ідентифікатор транзакції.");

        if (!_transactions.TryGetValue(transactionId, out Transaction? transaction))
            return Result<bool>.Failure($"Транзакцію {transactionId} не знайдено.");

        if (transaction.IsRefunded)
            return Result<bool>.Failure($"Транзакцію {transactionId} вже повернено раніше.");

        transaction.IsRefunded = true;
        CardBalance += transaction.Amount;
        return Result<bool>.Success(true);
    }

    public Result<string> TryProcessPayment(decimal amount)
    {
        try
        {
            ProcessPayment(amount);
            return Result<string>.Success(LastTransactionId!);
        }
        catch (PaymentGatewayException ex)
        {
            return Result<string>.Failure(ex);
        }
    }
}