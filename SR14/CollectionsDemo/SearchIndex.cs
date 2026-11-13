using System.Text.RegularExpressions;

namespace CollectionsDemo;


public class SearchIndex
{
    private readonly Dictionary<string, HashSet<int>> _invertedIndex =
        new(StringComparer.OrdinalIgnoreCase);

    
    public void AddDocument(int docId, string content)
    {
   
        string[] words = Regex.Split(content, @"\W+");

        foreach (string word in words)
        {
            if (string.IsNullOrWhiteSpace(word))
                continue;

            if (!_invertedIndex.TryGetValue(word, out HashSet<int>? docs))
            {
                docs = new HashSet<int>();
                _invertedIndex[word] = docs;
            }

            docs.Add(docId);
        }
    }

   
    public HashSet<int> Search(string word)
    {
        return _invertedIndex.TryGetValue(word, out HashSet<int>? docs)
            ? new HashSet<int>(docs)
            : new HashSet<int>();
    }

   
    public HashSet<int> SearchMultiple(params string[] words)
    {
        if (words.Length == 0)
            return new HashSet<int>();

        HashSet<int> result = Search(words[0]);

        for (int i = 1; i < words.Length; i++)
            result.IntersectWith(Search(words[i]));

        return result;
    }
}