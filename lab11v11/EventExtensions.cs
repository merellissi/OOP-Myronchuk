namespace Lab11v11;

public static class EventExtensions
{
    
    public static decimal TotalTicketRevenue(this IEnumerable<Event> events)
    {
        ArgumentNullException.ThrowIfNull(events);

        return events
            .SelectMany(e => e.Participants)
            .SelectMany(p => p.Tickets)
            .Sum(t => t.Price);
    }

   
    public static decimal TotalTicketRevenue(this Event ev)
    {
        ArgumentNullException.ThrowIfNull(ev);
        return new[] { ev }.TotalTicketRevenue();
    }

   
    public static IEnumerable<T> Paginate<T>(this IEnumerable<T> source, int pageNumber, int pageSize)
    {
        ArgumentNullException.ThrowIfNull(source);
        if (pageNumber < 1)
            throw new ArgumentOutOfRangeException(nameof(pageNumber), "Номер сторінки починається з 1.");
        if (pageSize < 1)
            throw new ArgumentOutOfRangeException(nameof(pageSize), "Розмір сторінки має бути більшим за 0.");

        return source
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize);
    }

    
    public static int PageCount<T>(this IReadOnlyCollection<T> source, int pageSize) =>
        (int)Math.Ceiling(source.Count / (double)pageSize);
}