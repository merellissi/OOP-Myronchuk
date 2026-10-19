namespace lab7v11;

public class InMemoryRepository<T> : IRepository<T> where T : class, IEntity
{
    private readonly List<T> _items = new();
    private int _nextId = 1;

    public T? GetById(int id) => _items.FirstOrDefault(x => x.Id == id);

    public IEnumerable<T> GetAll() => _items.ToList(); // копія списку

    public void Add(T item)
    {
        item.Id = _nextId++; // Id призначає репозиторій
        _items.Add(item);
    }

    public void Update(T item)
    {
        int index = _items.FindIndex(x => x.Id == item.Id);
        if (index == -1)
            throw new KeyNotFoundException($"Елемент з Id={item.Id} не знайдено.");
        _items[index] = item;
    }

    public void Delete(int id)
    {
        var item = GetById(id);
        if (item == null)
            throw new KeyNotFoundException($"Елемент з Id={id} не знайдено.");
        _items.Remove(item);
    }
}