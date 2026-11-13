public static class CollectionHelper<T>
{

    public static int Count(IEnumerable<T> collection, Predicate<T> predicate)
    {
        if (collection == null)
            throw new ArgumentNullException(nameof(collection));
        if (predicate == null)
            throw new ArgumentNullException(nameof(predicate));

        int count = 0;
        foreach (T item in collection)
        {
            if (predicate(item))
            {
                count++;
            }
        }
        return count;
    }
}