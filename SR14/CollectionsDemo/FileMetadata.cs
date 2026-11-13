namespace CollectionsDemo;


public class FileMetadata
{
    public string FileName { get; set; } = string.Empty;
    public long Size { get; set; }
    public DateTime LastModified { get; set; }
    public string Hash { get; set; } = string.Empty;

    public override string ToString() =>
        $"{FileName} | {Size} байт | змінено {LastModified:dd.MM.yyyy HH:mm} | hash {Hash}";
}