using System;
using System.Collections.Generic;


class Building
{
    public string Address { get; set; }

   
    public static List<string> Purposes = new List<string>();

    public Building(string address)
    {
        Address = address;
    }

    public virtual void GetPurpose()
    {
        string text = $"Будівля за адресою {Address}: призначення не визначено";
        Console.WriteLine(text);
        Purposes.Add(text);
    }
}

class House : Building
{
    public int NumRooms { get; set; }

    public House(string address, int numRooms) : base(address)
    {
        NumRooms = numRooms;
    }

    public override void GetPurpose()
    {
        string text = $"Житловий будинок ({Address}): проживання, кімнат - {NumRooms}";
        Console.WriteLine(text);
        Purposes.Add(text);
    }
}

class Office : Building
{
    public int NumFloors { get; set; }

    public Office(string address, int numFloors) : base(address)
    {
        NumFloors = numFloors;
    }

    public override void GetPurpose()
    {
        string text = $"Офіс ({Address}): робота компаній, поверхів - {NumFloors}";
        Console.WriteLine(text);
        Purposes.Add(text);
    }
}

class Shop : Building
{
    public string ProductType { get; set; }

    public Shop(string address, string productType) : base(address)
    {
        ProductType = productType;
    }

    public override void GetPurpose()
    {
        string text = $"Магазин ({Address}): торгівля, тип товарів - {ProductType}";
        Console.WriteLine(text);
        Purposes.Add(text);
    }
}

class Program
{
    static void Main()
    {
        Console.OutputEncoding = System.Text.Encoding.UTF8;

        
        List<Building> buildings = new List<Building>
        {
            new House("вул. Соборна, 10", 5),
            new Office("вул. Київська, 25", 9),
            new Shop("просп. Миру, 3", "продукти"),
            new House("вул. Шевченка, 7", 3)
        };

        Console.WriteLine("=== Виклик GetPurpose() для кожної будівлі ===");
        foreach (Building b in buildings)
        {
            b.GetPurpose(); 
        }

        Console.WriteLine();
        Console.WriteLine("=== Агрегація: список призначень усіх будівель ===");
        for (int i = 0; i < Building.Purposes.Count; i++)
        {
            Console.WriteLine($"{i + 1}. {Building.Purposes[i]}");
        }

        Console.WriteLine();
        Console.WriteLine($"Усього викликів GetPurpose(): {Building.Purposes.Count}");
    }
}