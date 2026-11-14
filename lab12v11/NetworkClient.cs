namespace Lab12v11;

public class NetworkClient
{
    private readonly int _failuresBeforeSuccess;
    private int _calls;

    public NetworkClient(int failuresBeforeSuccess = 3)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int Calls => _calls;

    public List<string> GetProductsFromApi(string url)
    {
        if (!Uri.TryCreate(url, UriKind.Absolute, out Uri? uri) || (uri.Scheme != Uri.UriSchemeHttp && uri.Scheme != Uri.UriSchemeHttps))
            throw new ArgumentException($"Некоректна адреса API: «{url}»", nameof(url));

        _calls++;
        Thread.Sleep(50);

        if (_calls <= _failuresBeforeSuccess)
            throw new HttpRequestException($"503 Service Unavailable від {uri.Host} (виклик #{_calls})");

        return new List<string> { "Ноутбук Lenovo IdeaPad", "Мишка Logitech M185", "Монітор Dell 24\"", "Клавіатура A4Tech", "Навушники JBL Tune 510" };
    }
}