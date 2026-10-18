namespace Lab6v11;

public interface IComparer<T>
{
    int Compare(T x, T y);
}

public class StringLengthComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return x.Length.CompareTo(y.Length);
    }
}

public class AlphabeticalComparer : IComparer<string>
{
    public int Compare(string x, string y)
    {
        return string.Compare(x, y, StringComparison.OrdinalIgnoreCase);
    }
}