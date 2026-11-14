
using System.Globalization;
using Lab13v11;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

var service = new PaymentService(gatewayName: "LiqPay", cardBalance: 10_000m);


Console.WriteLine("===== 1. ProcessPayment: обробка винятків через try-catch =====");
Console.WriteLine($"Баланс картки: {service.CardBalance:N2} грн");

foreach (decimal amount in new[] { 1_500m, 60_000m, 12_000m, -5m })
    Pay(service, amount);

Console.WriteLine();
Console.WriteLine("--- Шлюз недоступний ---");
var offlineService = new PaymentService(gatewayName: "LiqPay", cardBalance: 10_000m, isGatewayAvailable: false);
Pay(offlineService, 100m);


Console.WriteLine();
Console.WriteLine("===== 2. RefundPayment: обробка Result<bool> через IsSuccess / Error =====");
string firstTransaction = service.LastTransactionId!;

foreach (string transactionId in new[] { firstTransaction, firstTransaction, "TX-9999", "" })
{
    Result<bool> result = service.RefundPayment(transactionId);
    string shownId = transactionId.Length == 0 ? "\"\"" : transactionId;

    if (result.IsSuccess)
        Console.WriteLine($"RefundPayment({shownId}): УСПІХ — кошти повернено, баланс {service.CardBalance:N2} грн");
    else
        Console.WriteLine($"RefundPayment({shownId}): ВІДМОВА — {result.Error}");
}


Console.WriteLine();
Console.WriteLine("===== 3. TryProcessPayment: Result<string>.Failure(Exception) =====");
foreach (decimal amount in new[] { 2_000m, 75_000m })
{
    Result<string> result = service.TryProcessPayment(amount);
    Console.WriteLine($"TryProcessPayment({amount:N0}): {result}");
}
Console.WriteLine($"Підсумковий баланс картки: {service.CardBalance:N2} грн");


Console.WriteLine();
Console.WriteLine("===== 4. Конструктори PaymentGatewayException =====");
var examples = new PaymentGatewayException[]
{
    new(),
    new("Шлюз відхилив запит."),
    new("Помилка під час платежу.", new HttpRequestException("502 Bad Gateway")),
    new("Картку заблоковано.", "Monobank Acquiring", "CARD_BLOCKED")
};
foreach (PaymentGatewayException ex in examples)
    Console.WriteLine($"Message: {ex.Message} | Gateway: {Show(ex.GatewayName)} | Code: {Show(ex.ErrorCode)} | Inner: {ex.InnerException?.Message ?? "—"}");

static string Show(string value) => value.Length == 0 ? "—" : value;

static void Pay(PaymentService payments, decimal amount)
{
    Console.WriteLine($"ProcessPayment({amount:N2})");
    try
    {
        payments.ProcessPayment(amount);
        Console.WriteLine($"   УСПІХ: транзакція {payments.LastTransactionId}, баланс {payments.CardBalance:N2} грн");
    }
    catch (PaymentGatewayException ex) when (ex.ErrorCode == "GATEWAY_UNAVAILABLE")
    {
        Console.WriteLine($"   PaymentGatewayException [{ex.GatewayName}, {ex.ErrorCode}]: {ex.Message}");
        Console.WriteLine($"   Причина: {ex.InnerException?.Message}. Пропонуємо спробувати пізніше або інший спосіб оплати.");
    }
    catch (PaymentGatewayException ex)
    {
        Console.WriteLine($"   PaymentGatewayException [{ex.GatewayName}, {ex.ErrorCode}]: {ex.Message}");
    }
    catch (ArgumentOutOfRangeException ex)
    {
        Console.WriteLine($"   ArgumentOutOfRangeException: {ex.Message}");
    }
    finally
    {
        Console.WriteLine("   finally: спробу платежу записано в журнал");
    }
}