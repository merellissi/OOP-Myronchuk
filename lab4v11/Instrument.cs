class Instrument
{
    private string _brand;
    private decimal _price;

    public string Brand
    {
        get { return _brand; }
        set { _brand = value; }
    }

    public decimal Price
    {
        get { return _price; }
        set { _price = value; }
    }

    public Instrument(string brand, decimal price)
    {
        _brand = brand;
        _price = price;
    }

    public virtual void PlaySound()
    {
        Console.WriteLine($"{Brand}: загальний звук музичного інструмента.");
    }


    public string GetInstrumentType()
    {
        return "Музичний інструмент";
    }
}