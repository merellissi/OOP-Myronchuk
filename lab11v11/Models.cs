namespace Lab11v11;

public class Ticket
{
    public int Id { get; set; }
    public string Type { get; set; } = string.Empty;
    public decimal Price { get; set; }
}

public class Participant
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Ticket> Tickets { get; set; } = new();
}

public class Event
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public List<Participant> Participants { get; set; } = new();
}

public record TicketInfo(string EventName, string ParticipantName, int TicketId, string Type, decimal Price);