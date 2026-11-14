namespace Lab10v11;


public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime Date { get; set; }
    public int LocationId { get; set; }
}


public class Venue
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Capacity { get; set; }
}



public record EventAtVenue(string EventName, DateTime Date, string VenueName, int Capacity);

public record EventVenuePair(int EventId, string EventName, string VenueName);

public record VenueEventCount(string VenueName, int EventCount);