Instrument basic = new Instrument("Generic", 100m);
Guitar guitar = new Guitar("Fender", 1200m, 6);
Piano piano = new Piano("Steinway", 25000m, 88);

Console.WriteLine("=== 1. Поліморфізм (virtual/override) ===");
Instrument[] instruments = { basic, guitar, piano };
foreach (Instrument item in instruments)
{
    item.PlaySound();
    Console.WriteLine();
}

Console.WriteLine("=== 2. override vs new ===");
Instrument asInstrument = guitar;
Console.WriteLine("Через Instrument: " + asInstrument.GetInstrumentType());
Console.WriteLine("Через Guitar:     " + guitar.GetInstrumentType());

Console.WriteLine();
Console.WriteLine("=== 3. Власні методи ===");
guitar.Strum();
piano.PressPedal();