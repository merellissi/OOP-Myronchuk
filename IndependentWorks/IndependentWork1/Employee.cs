namespace IndependentWork1;

public class Employee
{
    private string _name;
    private string _position;
    private decimal _salary;

    public string Name
    {
        get { return _name; }
        set { _name = value; }
    }

    public string Position
    {
        get { return _position; }
        set { _position = value; }
    }

    // read-only: немає set
    public decimal Salary
    {
        get { return _salary; }
    }

    public Employee(string name, string position, decimal salary)
    {
        _name = name;
        _position = position;
        _salary = salary;
    }

    public decimal CalculateBonus(decimal percentage)
    {
        return _salary * percentage / 100;
    }

    public bool ApplyRaise(decimal percentage)
    {
        if (percentage <= 0)
        {
            return false;
        }

        _salary += _salary * percentage / 100;
        return true;
    }

    public void PrintInfo()
    {
        Console.WriteLine($"Працівник: {_name}, посада: {_position}, зарплата: {_salary:F2} грн");
    }
}