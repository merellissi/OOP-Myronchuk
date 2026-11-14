namespace IndependentWork19;


public class OutOfStockException : Exception
{
    
    public string? ProductName { get; }

    
    public int RequestedQuantity { get; }

    
    public int AvailableQuantity { get; }

   

    public OutOfStockException()
        : base("Товару недостатньо на складі.")
    {
    }

    public OutOfStockException(string message)
        : base(message)
    {
    }

    public OutOfStockException(string message, Exception innerException)
        : base(message, innerException)
    {
    }

    

    public OutOfStockException(string message, string productName, int requestedQuantity, int availableQuantity)
        : base(message)
    {
        ProductName = productName;
        RequestedQuantity = requestedQuantity;
        AvailableQuantity = availableQuantity;
    }

  
    public int Shortage => RequestedQuantity - AvailableQuantity;
}