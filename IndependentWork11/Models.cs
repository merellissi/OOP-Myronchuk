namespace IndependentWork11;

public class Product
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Category { get; set; } = string.Empty;
    public decimal UnitPrice { get; set; }
}


public class Customer
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string City { get; set; } = string.Empty;
}


public class Sale
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public int Quantity { get; set; }
    public DateTime SaleDate { get; set; }
    public int CustomerId { get; set; }
}



public record ProductSales(string Name, int TotalQuantity);

public record CategoryTopProducts(string Category, List<ProductSales> TopProducts);

public record CityRevenue(string City, int SalesCount, decimal Revenue);

public record MonthlySales(int Year, int Month, int SalesCount, int Units, decimal Revenue);

public record CustomerStats(string Name, string City, int Purchases, decimal TotalSpent, decimal AverageCheck, string FavoriteCategory);