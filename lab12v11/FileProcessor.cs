namespace Lab12v11;

public class FileProcessor
{
    private readonly int _failuresBeforeSuccess;
    private int _calls;

    public FileProcessor(int failuresBeforeSuccess = 2)
    {
        _failuresBeforeSuccess = failuresBeforeSuccess;
    }

    public int Calls => _calls;

    public List<string> LoadProductNames(string path)
    {
        _calls++;

        if (_calls <= _failuresBeforeSuccess)
            throw new FileNotFoundException($"Файл ще не синхронізовано з мережевим диском (виклик #{_calls})", path);

        return File.ReadAllLines(path)
            .Select(line => line.Trim())
            .Where(line => line.Length > 0)
            .ToList();
    }
}