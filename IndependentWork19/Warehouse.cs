namespace IndependentWork19;


public class Warehouse
{
    private readonly Dictionary<string, int> _stock;

    public Warehouse(Dictionary<string, int> stock)
    {
        _stock = new Dictionary<string, int>(stock, StringComparer.OrdinalIgnoreCase);
    }

    public int GetStock(string product) => _stock.TryGetValue(product, out int count) ? count : 0;

 
    public void Reserve(string product, int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentOutOfRangeException(nameof(quantity), "Кількість має бути більшою за 0.");

        if (!_stock.TryGetValue(product, out int available))
            throw new KeyNotFoundException($"Товару «{product}» немає в каталозі.");

        if (quantity > available)
            throw new OutOfStockException($"Недостатньо товару «{product}» на складі.", product, quantity, available);

        _stock[product] = available - quantity;
    }

 
    public Result<int> TryReserve(string product, int quantity)
    {
        if (quantity <= 0)
            return Result<int>.Failure("Кількість має бути більшою за 0.");

        if (!_stock.TryGetValue(product, out int available))
            return Result<int>.Failure($"Товару «{product}» немає в каталозі.");

        if (quantity > available)
            return Result<int>.Failure($"На складі лише {available} шт. товару «{product}», запитано {quantity}.");

        _stock[product] = available - quantity;
        return Result<int>.Success(available - quantity);
    }
}