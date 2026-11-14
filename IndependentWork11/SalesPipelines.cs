namespace IndependentWork11;

public static class SalesPipelines
{
   
    public static List<CategoryTopProducts> TopProductsByCategory(
        IEnumerable<Sale> sales, IEnumerable<Product> products, int topN = 3)
    {
        return sales
           
            .GroupBy(s => s.ProductId)
            .Select(g => new { ProductId = g.Key, TotalQuantity = g.Sum(s => s.Quantity) })
          
            .Join(products,
                  stat => stat.ProductId,
                  p => p.Id,
                  (stat, p) => new { p.Name, p.Category, stat.TotalQuantity })
         
            .GroupBy(x => x.Category)
            .Select(g => new CategoryTopProducts(
                g.Key,
                g.OrderByDescending(x => x.TotalQuantity)
                 .ThenBy(x => x.Name)
                 .Take(topN)
                 .Select(x => new ProductSales(x.Name, x.TotalQuantity))
                 .ToList()))
            .OrderBy(c => c.Category)
            .ToList();
    }

 
    public static List<CityRevenue> RevenueByCity(
        IEnumerable<Sale> sales, IEnumerable<Product> products, IEnumerable<Customer> customers)
    {
        return sales
        
            .Join(products, s => s.ProductId, p => p.Id,
                  (s, p) => new { s.CustomerId, Revenue = s.Quantity * p.UnitPrice })
           
            .Join(customers, x => x.CustomerId, c => c.Id,
                  (x, c) => new { c.City, x.Revenue })
           
            .GroupBy(x => x.City)
            .Select(g => new CityRevenue(g.Key, g.Count(), g.Sum(x => x.Revenue)))
            
            .OrderByDescending(c => c.Revenue)
            .ToList();
    }

    
    public static List<MonthlySales> SalesByMonth(IEnumerable<Sale> sales, IEnumerable<Product> products)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id,
                  (s, p) => new { s.SaleDate, s.Quantity, Revenue = s.Quantity * p.UnitPrice })
     
            .GroupBy(x => new { x.SaleDate.Year, x.SaleDate.Month })
            .Select(g => new MonthlySales(
                g.Key.Year,
                g.Key.Month,
                g.Count(),
                g.Sum(x => x.Quantity),
                g.Sum(x => x.Revenue)))
            .OrderBy(m => m.Year)
            .ThenBy(m => m.Month)
            .ToList();
    }

    public static List<CustomerStats> TopCustomers(
        IEnumerable<Sale> sales, IEnumerable<Product> products, IEnumerable<Customer> customers, int topN = 5)
    {
        return sales
            .Join(products, s => s.ProductId, p => p.Id,
                  (s, p) => new { s.CustomerId, p.Category, Revenue = s.Quantity * p.UnitPrice })
            .GroupBy(x => x.CustomerId)
            .Select(g => new
            {
                CustomerId = g.Key,
                Purchases = g.Count(),
                TotalSpent = g.Sum(x => x.Revenue),
                
                FavoriteCategory = g.GroupBy(x => x.Category)
                                    .OrderByDescending(c => c.Sum(x => x.Revenue))
                                    .First().Key
            })
            .OrderByDescending(x => x.TotalSpent)
            .Take(topN)
            .Join(customers, x => x.CustomerId, c => c.Id,
                  (x, c) => new CustomerStats(
                      c.Name, c.City, x.Purchases, x.TotalSpent,
                      Math.Round(x.TotalSpent / x.Purchases, 2), x.FavoriteCategory))
            .ToList();
    }
}