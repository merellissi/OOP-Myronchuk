using System;

class Plane
{
    private string airline;
    private string model;

    public int Capacity { get; set; }

    public Plane(string airline, string model, int capacity)
    {
        this.airline = airline;
        this.model = model;
        Capacity = capacity;
    }

    public void Fly()
    {
        Console.WriteLine($"Літак {airline} {model} летить. Кількість пасажирів: {Capacity}");
    }

    ~Plane()
    {
        Console.WriteLine($"Літак {model} видалено.");
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        Console.WriteLine("ЛАБОРАТОРНА РОБОТА №1");
        Console.WriteLine("Варіант №11");
        Console.WriteLine("Клас Plane");
        Console.WriteLine();

        Plane plane1 = new Plane("Ryanair", "Boeing 737", 189);
        Plane plane2 = new Plane("Wizz Air", "Airbus A320", 180);
        Plane plane3 = new Plane("Lufthansa", "Airbus A350", 325);

        plane1.Fly();
        plane2.Fly();
        plane3.Fly();

        Console.WriteLine();
        Console.WriteLine("Програму завершено.");
    }
}