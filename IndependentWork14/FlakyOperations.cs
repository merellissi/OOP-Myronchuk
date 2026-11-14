namespace IndependentWork14;


public class FlakyOperations
{
    private int _priceListCalls;
    private int _stockCalls;

    
    public string DownloadPriceList()
    {
        _priceListCalls++;
        return _priceListCalls switch
        {
            1 => throw new HttpRequestException("503 Service Unavailable"),
            2 => throw new TimeoutException("сервер постачальника не відповів за 2 с"),
            3 => throw new HttpRequestException("502 Bad Gateway"),
            _ => "price-list-2026-11.csv (1 248 позицій)"
        };
    }

   
    public int GetStock(string product)
    {
        _stockCalls++;
        if (_stockCalls == 1)
            throw new TimeoutException("склад не відповів");
        return product == "Ноутбук" ? 7 : 0;
    }

   
    public decimal GetDailyRevenue()
    {
        throw new TimeoutException("не вдалося з'єднатися з сервером БД (connection timeout)");
    }

   
    public int CreateOrder(int quantity)
    {
        if (quantity <= 0)
            throw new ArgumentException("кількість товару має бути більшою за 0", nameof(quantity));
        return 10_001;
    }
}