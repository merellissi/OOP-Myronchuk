namespace Lab10v11;

public static class EventQueries
{
    
    public static List<EventAtVenue> Q1_QuerySyntax(List<Event> events, List<Venue> venues) =>
        (from e in events
         join v in venues on e.LocationId equals v.Id
         where e.Date.Year == 2024 && v.Capacity > 500
         orderby e.Date
         select new EventAtVenue(e.Name, e.Date, v.Name, v.Capacity))
        .ToList();

    public static List<EventAtVenue> Q1_MethodSyntax(List<Event> events, List<Venue> venues) =>
        events
            .Join(venues, e => e.LocationId, v => v.Id, (e, v) => new { e, v })
            .Where(x => x.e.Date.Year == 2024 && x.v.Capacity > 500)
            .OrderBy(x => x.e.Date)
            .Select(x => new EventAtVenue(x.e.Name, x.e.Date, x.v.Name, x.v.Capacity))
            .ToList();

    

    public static List<EventVenuePair> Q2_QuerySyntax(List<Event> events, List<Venue> venues) =>
        (from e in events
         join v in venues on e.LocationId equals v.Id
         orderby e.Id
         select new EventVenuePair(e.Id, e.Name, v.Name))
        .ToList();

    public static List<EventVenuePair> Q2_MethodSyntax(List<Event> events, List<Venue> venues) =>
        events
            .Join(venues, e => e.LocationId, v => v.Id, (e, v) => new EventVenuePair(e.Id, e.Name, v.Name))
            .OrderBy(p => p.EventId)
            .ToList();

 
    public static List<VenueEventCount> Q3_QuerySyntax(List<Event> events, List<Venue> venues) =>
        (from v in venues
         join e in events on v.Id equals e.LocationId into venueEvents
         let count = venueEvents.Count()
         orderby count descending, v.Name
         select new VenueEventCount(v.Name, count))
        .ToList();

    public static List<VenueEventCount> Q3_MethodSyntax(List<Event> events, List<Venue> venues) =>
        venues
            .GroupJoin(events, v => v.Id, e => e.LocationId,
                       (v, venueEvents) => new VenueEventCount(v.Name, venueEvents.Count()))
            .OrderByDescending(x => x.EventCount)
            .ThenBy(x => x.VenueName)
            .ToList();

    

    public static Event? Q4_QuerySyntax(List<Event> events, DateTime today) =>
        (from e in events
         where e.Date >= today
         orderby e.Date
         select e)
        .FirstOrDefault();

    public static Event? Q4_MethodSyntax(List<Event> events, DateTime today) =>
        events
            .Where(e => e.Date >= today)
            .MinBy(e => e.Date);   
}