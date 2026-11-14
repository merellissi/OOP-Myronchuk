
using System.Globalization;
using Lab11v11;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

List<Event> events = TestData.Events();

Console.WriteLine("===== Тестові дані =====");
Console.WriteLine($"Подій: {events.Count}, учасників: {events.Sum(e => e.Participants.Count)}, " +
                  $"квитків: {events.Sum(e => e.Participants.Sum(p => p.Tickets.Count))}");


Console.WriteLine();
Console.WriteLine("===== 1. SelectMany: усі квитки в одному списку =====");

List<Ticket> allTickets = events
    .SelectMany(e => e.Participants)
    .SelectMany(p => p.Tickets)
    .ToList();

List<TicketInfo> ticketsWithContext = events
    .SelectMany(e => e.Participants, (e, p) => new { Event = e, Participant = p })
    .SelectMany(x => x.Participant.Tickets,
                (x, t) => new TicketInfo(x.Event.Name, x.Participant.Name, t.Id, t.Type, t.Price))
    .ToList();

List<TicketInfo> ticketsQuerySyntax =
    (from e in events
     from p in e.Participants
     from t in p.Tickets
     select new TicketInfo(e.Name, p.Name, t.Id, t.Type, t.Price))
    .ToList();

Console.WriteLine($"{"№",3}  {"Подія",-26} {"Учасник",-21} {"Тип квитка",-18} {"Ціна, грн",9}");
foreach (TicketInfo t in ticketsWithContext)
    Console.WriteLine($"{t.TicketId,3}  {t.EventName,-26} {t.ParticipantName,-21} {t.Type,-18} {t.Price,9:N0}");

Console.WriteLine($"Усього квитків: {allTickets.Count}");
Console.WriteLine($"Синтаксис запитів (from … from … from) дав той самий результат: " +
                  $"{(ticketsQuerySyntax.SequenceEqual(ticketsWithContext) ? "так" : "НІ")}");

Console.WriteLine();
Console.WriteLine("--- Select vs SelectMany ---");
IEnumerable<List<Participant>> nested = events.Select(e => e.Participants);
IEnumerable<Participant> flat = events.SelectMany(e => e.Participants);
Console.WriteLine($"Select:     {nested.Count()} елементів типу List<Participant> (список списків)");
Console.WriteLine($"SelectMany: {flat.Count()} елементів типу Participant (один плаский список)");

Console.WriteLine();
Console.WriteLine("--- Квитки за типами (SelectMany + GroupBy) ---");
var byType = allTickets
    .GroupBy(t => t.Type)
    .Select(g => new { Type = g.Key, Count = g.Count(), Sum = g.Sum(t => t.Price) })
    .OrderByDescending(x => x.Sum)
    .ThenBy(x => x.Type);
foreach (var x in byType)
    Console.WriteLine($"   {x.Type,-18} {x.Count} шт.  {x.Sum,8:N0} грн");


const int PageSize = 2;
int pageCount = events.PageCount(PageSize);

Console.WriteLine();
Console.WriteLine($"===== 2. Пагінація: {events.Count} подій по {PageSize} на сторінку, сторінок: {pageCount} =====");

for (int page = 1; page <= pageCount; page++)
{
    Console.WriteLine($"--- Сторінка {page} з {pageCount} (Skip({(page - 1) * PageSize}).Take({PageSize})) ---");
    foreach (Event e in events.Paginate(page, PageSize))
        Console.WriteLine($"   [{e.Id}] {e.Name,-26} учасників: {e.Participants.Count}, дохід: {e.TotalTicketRevenue():N0} грн");
}

int outOfRange = pageCount + 1;
Console.WriteLine($"--- Сторінка {outOfRange} (поза межами) ---");
Console.WriteLine($"   Подій на сторінці: {events.Paginate(outOfRange, PageSize).Count()}");


Console.WriteLine();
Console.WriteLine("===== 3. Метод розширення TotalTicketRevenue() =====");

decimal total = events.TotalTicketRevenue();
Console.WriteLine($"events.TotalTicketRevenue() = {total:N0} грн");
Console.WriteLine($"Перевірка через allTickets.Sum(): {allTickets.Sum(t => t.Price):N0} грн -> " +
                  $"{(total == allTickets.Sum(t => t.Price) ? "збігається" : "НЕ збігається")}");

decimal concerts = events
    .Where(e => e.Name.Contains("концерт", StringComparison.OrdinalIgnoreCase))
    .TotalTicketRevenue();
Console.WriteLine($"Лише концерти (Where + TotalTicketRevenue): {concerts:N0} грн");

decimal firstPage = events.Paginate(1, PageSize).TotalTicketRevenue();
Console.WriteLine($"Дохід подій першої сторінки (Paginate + TotalTicketRevenue): {firstPage:N0} грн");

Console.WriteLine("Топ-3 події за доходом:");
foreach (Event e in events.OrderByDescending(ev => ev.TotalTicketRevenue()).Take(3))
    Console.WriteLine($"   {e.Name,-26} {e.TotalTicketRevenue(),8:N0} грн");

Console.WriteLine($"Порожній список: new List<Event>().TotalTicketRevenue() = {new List<Event>().TotalTicketRevenue():N0} грн");