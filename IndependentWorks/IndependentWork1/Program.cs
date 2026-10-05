using System.Text;

namespace IndependentWork1;

public class Program
{
    public static void Main(string[] args)
    {
        Console.OutputEncoding = Encoding.UTF8;

        Console.WriteLine("=== Клас Employee ===");
        Employee employee = new Employee("Олена Коваленко", "Розробниця", 30000m);
        employee.PrintInfo();
        Console.WriteLine($"Премія 15%: {employee.CalculateBonus(15):F2} грн");
        employee.ApplyRaise(10);
        Console.WriteLine($"Зарплата після підвищення на 10%: {employee.Salary:F2} грн");

        Console.WriteLine();
        Console.WriteLine("=== Клас Rectangle ===");
        Rectangle rectangle = new Rectangle(4, 6);
        Console.WriteLine($"Площа: {rectangle.Area}, периметр: {rectangle.GetPerimeter()}");
        Console.WriteLine($"Це квадрат? {(rectangle.IsSquare() ? "Так" : "Ні")}");
        rectangle.Height = 4;
        Console.WriteLine($"Після зміни висоти: це квадрат? {(rectangle.IsSquare() ? "Так" : "Ні")}");

        Console.WriteLine();
        Console.WriteLine("=== Клас Playlist ===");
        Playlist playlist = new Playlist("Для навчання");
        playlist.AddTrack("Lo-fi Morning", 42);
        playlist.AddTrack("Deep Focus", 55);
        playlist.AddTrack("", 5);
        playlist.PrintTracks();
        Console.WriteLine($"Загальна тривалість: {playlist.TotalMinutes} хв ({playlist.GetDurationText()})");
    }
}