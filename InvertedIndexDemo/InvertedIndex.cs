namespace InvertedIndexDemo;


public class InvertedIndex
{
    
    private readonly Dictionary<string, HashSet<int>> _index;

  
    private readonly Dictionary<int, Document> _documents;

    public InvertedIndex()
    {
        _index = new Dictionary<string, HashSet<int>>();
        _documents = new Dictionary<int, Document>();
    }

    public int DocumentCount => _documents.Count;

    public int UniqueWords => _index.Keys.Count;


    public void AddDocument(Document doc)
    {
        ArgumentNullException.ThrowIfNull(doc);

        
        _documents[doc.Id] = doc;

        
        foreach (string word in TextProcessor.GetTerms(doc.Content))
        {
            if (!_index.TryGetValue(word, out HashSet<int>? ids))
            {
                ids = new HashSet<int>();
                _index[word] = ids;
            }

            ids.Add(doc.Id);
        }
    }

    
    public List<Document> SearchSingleWord(string word)
    {
        string? term = NormalizeQuery(word).FirstOrDefault();
        if (term is null || !_index.TryGetValue(term, out HashSet<int>? ids))
            return new List<Document>();

        return ToDocuments(ids);
    }

    
    public List<Document> SearchMultipleWords(params string[] words)
    {
        List<string> terms = NormalizeQuery(words);
        if (terms.Count == 0)
            return new List<Document>();

        HashSet<int>? result = null;

        foreach (string term in terms)
        {
        
            if (!_index.TryGetValue(term, out HashSet<int>? ids))
                return new List<Document>();

            if (result is null)
                result = new HashSet<int>(ids); 
            else
                result.IntersectWith(ids);
        }

        return ToDocuments(result!);
    }


    public List<Document> SearchAnyWord(params string[] words)
    {
        List<string> terms = NormalizeQuery(words);
        var result = new HashSet<int>();

        foreach (string term in terms)
        {
            if (_index.TryGetValue(term, out HashSet<int>? ids))
                result.UnionWith(ids);
        }


        return result
            .OrderByDescending(id => terms.Count(t => _index.TryGetValue(t, out HashSet<int>? set) && set.Contains(id)))
            .ThenBy(id => id)
            .Select(id => _documents[id])
            .ToList();
    }

  
    private static List<string> NormalizeQuery(params string[] words)
    {
        return TextProcessor.GetTerms(string.Join(' ', words)).Distinct().ToList();
    }

    private List<Document> ToDocuments(IEnumerable<int> ids)
    {
        return ids.OrderBy(id => id).Select(id => _documents[id]).ToList();
    }
}