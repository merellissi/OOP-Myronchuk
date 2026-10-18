namespace Lab6v11;

public abstract class FileProcessor
{
    protected List<string> Lines = new List<string>();

    public abstract void ReadFile(string path);
    public abstract void ProcessContent();

    public void WriteResult(string path)
    {
        File.WriteAllLines(path, Lines);
        Console.WriteLine($"  Результат записано у файл: {path} (рядків: {Lines.Count})");
    }

    public void PrintLines()
    {
        foreach (string line in Lines)
        {
            Console.WriteLine($"    {line}");
        }
    }
}

public class LogFileProcessor : FileProcessor
{
    public override void ReadFile(string path)
    {
        Lines = File.ReadAllLines(path).ToList();
        Console.WriteLine($"  [Log] Прочитано рядків: {Lines.Count}");
    }

    public override void ProcessContent()
    {
        Lines = Lines
            .Where(l => l.Contains("ERROR") || l.Contains("WARN"))
            .ToList();
        Console.WriteLine($"  [Log] Після фільтрації (ERROR/WARN) залишилось: {Lines.Count}");
    }
}

public class ConfigFileProcessor : FileProcessor
{
    public override void ReadFile(string path)
    {
        Lines = File.ReadAllLines(path).ToList();
        Console.WriteLine($"  [Config] Прочитано рядків: {Lines.Count}");
    }

    public override void ProcessContent()
    {
        Lines = Lines
            .Select(l => l.Trim())
            .Where(l => l.Length > 0 && !l.StartsWith("#"))
            .ToList();
        Console.WriteLine($"  [Config] Після очищення залишилось: {Lines.Count}");
    }
}