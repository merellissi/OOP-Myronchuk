using System.Text.RegularExpressions;

namespace InvertedIndexDemo;


public static class TextProcessor
{

    private static readonly Regex WordRegex = new(@"\w+[#+]*", RegexOptions.Compiled);

    private static readonly HashSet<string> StopWords = new()
    {
        "a", "an", "the", "is", "are", "was", "were",
        "in", "on", "at", "to", "for", "of", "and", "or"
    };

  
    public static IEnumerable<string> Tokenize(string text)
    {
        if (string.IsNullOrWhiteSpace(text))
            yield break;

        foreach (Match match in WordRegex.Matches(text.ToLowerInvariant()))
            yield return match.Value;
    }

  
    public static IEnumerable<string> RemoveStopWords(IEnumerable<string> words)
    {
        return words.Where(w => !StopWords.Contains(w));
    }


    public static IEnumerable<string> GetTerms(string text)
    {
        return RemoveStopWords(Tokenize(text));
    }
}