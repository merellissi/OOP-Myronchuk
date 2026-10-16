class Piano : Instrument
{
    public int NumKeys { get; set; }

    public Piano(string brand, decimal price, int numKeys)
        : base(brand, price)
    {
        NumKeys = numKeys;
    }

    public override void PlaySound()
    {
        Console.WriteLine($"Піаніно {Brand} ({NumKeys} клавіш): дзинь-дзинь!");
    }

    public void PressPedal()
    {
        Console.WriteLine("Натиснуто педаль.");
    }
}