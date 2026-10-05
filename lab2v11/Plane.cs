namespace lab2v11;

public class Plane
{
    
    private string _airline;
    private string _model;
    private int _capacity;

    
    public string Airline
    {
        get { return _airline; }
        set { _airline = value; }
    }

    public string Model
    {
        get { return _model; }
        set { _model = value; }
    }

    
    public int Capacity
    {
        get { return _capacity; }
        set
        {
            if (value <= 0)
            {
                throw new ArgumentException("Місткість повинна бути більшою за 0!");
            }
            _capacity = value;
        }
    }

   
    public Plane(string airline, string model, int capacity)
    {
        _airline = airline;
        _model = model;
        Capacity = capacity; 
        Console.WriteLine($"[Конструктор] Створено літак: {_airline} {_model}, місць: {_capacity}");
    }


    public Plane() : this("N/A", "Boeing 737", 150)
    {
    }

   
    public void Fly()
    {
        Console.WriteLine($"Літак {Model} авіакомпанії {Airline} злетів і летить з {Capacity} пасажирами.");
    }


    ~Plane()
    {
        Console.WriteLine($"[Деструктор] Знищено літак: {_airline} {_model}");
    }
}