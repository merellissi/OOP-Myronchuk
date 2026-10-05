using lab2v11;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.WriteLine("--- Creating objects ---");

CreateAndUsePlanes();

Console.WriteLine("--- End of Main, preparing for GC ---");

GC.Collect();
GC.WaitForPendingFinalizers();

Console.WriteLine("--- GC finished ---");

static void CreateAndUsePlanes()
{
    Plane plane1 = new Plane();
    Plane plane2 = new Plane("Ukraine International", "Embraer 190", 100);
    Plane plane3 = new Plane("Lufthansa", "Airbus A320", 180);

    Console.WriteLine("--- Objects created ---");

    plane1.Fly();
    plane2.Fly();
    plane3.Fly();

    
    try
    {
        plane2.Capacity = -5;
    }
    catch (ArgumentException ex)
    {
        Console.WriteLine($"Помилка: {ex.Message}");
    }
}