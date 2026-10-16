class Guitar : Instrument
{
    public int NumStrings { get; set; }

    public Guitar(string brand, decimal price, int numStrings)
        : base(brand, price)
    {
        NumStrings = numStrings;
    }

    public override void PlaySound()
    {
        base.PlaySound();
        Console.WriteLine($"Гітара {Brand} ({NumStrings} струн): бринь-бринь!");
    }

    public void Strum()
    {
        Console.WriteLine("Граємо перебором по струнах.");
    }

    public new string GetInstrumentType()
    {
        return "Гітара";
    }
}