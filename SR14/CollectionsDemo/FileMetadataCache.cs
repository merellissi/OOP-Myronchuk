namespace CollectionsDemo;


public class FileMetadataCache
{

    private readonly Dictionary<string, FileMetadata> _cache =
        new(StringComparer.OrdinalIgnoreCase);

    public int Count => _cache.Count;

    
    public void Add(FileMetadata metadata)
    {
        ArgumentNullException.ThrowIfNull(metadata);
        _cache[metadata.FileName] = metadata;
    }

   
    public FileMetadata Get(string fileName)
    {
        if (_cache.TryGetValue(fileName, out FileMetadata? metadata))
            return metadata;

        throw new KeyNotFoundException($"Файл '{fileName}' відсутній у кеші.");
    }

    
    public bool Remove(string fileName) => _cache.Remove(fileName);

   
    public bool Contains(string fileName) => _cache.ContainsKey(fileName);
}