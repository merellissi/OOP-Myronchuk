

using System.Diagnostics;
using System.Globalization;
using Lab10v11;

Console.OutputEncoding = System.Text.Encoding.UTF8;
CultureInfo.CurrentCulture = new CultureInfo("uk-UA");

List<Venue> venues = TestData.Venues();
List<Event> events = TestData.Events();
DateTime today = DateTime.Today;

Console.WriteLine("===== Тестові дані =====");
Console.WriteLine($"Локацій: {venues.Count}, подій: {events.Count}, сьогодні: {today:dd.MM.yyyy}");

const int Repeats = 1000;
var timings = new List<(string Query, string Syntax, double FirstUs, double AvgUs)>();


var (q1a, f, a) = Measure(() => EventQueries.Q1_QuerySyntax(events, venues), Repeats);
timings.Add(("1. Фільтр + join", "запитів", f, a));
var (q1b, f2, a2) = Measure(() => EventQueries.Q1_MethodSyntax(events, venues), Repeats);
timings.Add(("1. Фільтр + join", "методів", f2, a2));

Console.WriteLine();
Console.WriteLine("===== Запит 1. Події 2024 року на локаціях місткістю > 500 =====");
foreach (EventAtVenue e in q1b)
    Console.WriteLine($"   {e.Date:dd.MM.yyyy}  {e.EventName,-28} {e.VenueName} ({e.Capacity} місць)");
PrintCheck(q1a.SequenceEqual(q1b), q1b.Count);


var (q2a, f3, a3) = Measure(() => EventQueries.Q2_QuerySyntax(events, venues), Repeats);
timings.Add(("2. Join", "запитів", f3, a3));
var (q2b, f4, a4) = Measure(() => EventQueries.Q2_MethodSyntax(events, venues), Repeats);
timings.Add(("2. Join", "методів", f4, a4));

Console.WriteLine();
Console.WriteLine("===== Запит 2. Події та їхні локації =====");
foreach (EventVenuePair p in q2b)
    Console.WriteLine($"   [{p.EventId,2}] {p.EventName,-30} -> {p.VenueName}");
PrintCheck(q2a.SequenceEqual(q2b), q2b.Count);


var (q3a, f5, a5) = Measure(() => EventQueries.Q3_QuerySyntax(events, venues), Repeats);
timings.Add(("3. GroupJoin + Count", "запитів", f5, a5));
var (q3b, f6, a6) = Measure(() => EventQueries.Q3_MethodSyntax(events, venues), Repeats);
timings.Add(("3. GroupJoin + Count", "методів", f6, a6));

Console.WriteLine();
Console.WriteLine("===== Запит 3. Кількість подій для кожної локації =====");
foreach (VenueEventCount c in q3b)
    Console.WriteLine($"   {c.VenueName,-26} {c.EventCount} подій");
PrintCheck(q3a.SequenceEqual(q3b), q3b.Count);


var (q4a, f7, a7) = Measure(() => EventQueries.Q4_QuerySyntax(events, today), Repeats);
timings.Add(("4. Найближча подія", "запитів", f7, a7));
var (q4b, f8, a8) = Measure(() => EventQueries.Q4_MethodSyntax(events, today), Repeats);
timings.Add(("4. Найближча подія", "методів", f8, a8));

Console.WriteLine();
Console.WriteLine($"===== Запит 4. Найближча подія (від {today:dd.MM.yyyy}) =====");
if (q4b is null)
{
    Console.WriteLine("   Майбутніх подій немає");
}
else
{
    int locationId = q4b.LocationId;
    string venueName = venues.First(v => v.Id == locationId).Name;
    Console.WriteLine($"   {q4b.Name}, {q4b.Date:dd.MM.yyyy}, {venueName} (через {(q4b.Date - today).Days} дн.)");
}
PrintCheck(q4a?.Id == q4b?.Id, q4b is null ? 0 : 1);


Console.WriteLine();
Console.WriteLine($"===== Час виконання на тестових даних (20 подій), Stopwatch, середнє з {Repeats} запусків =====");
PrintTimings(timings);


var (bigVenues, bigEvents) = TestData.Generate(venueCount: 1_000, eventCount: 100_000);
const int BigRepeats = 20;
var bigTimings = new List<(string Query, string Syntax, double FirstUs, double AvgUs)>();

var (_, bf1, ba1) = Measure(() => EventQueries.Q1_QuerySyntax(bigEvents, bigVenues), BigRepeats);
var (_, bf2, ba2) = Measure(() => EventQueries.Q1_MethodSyntax(bigEvents, bigVenues), BigRepeats);
var (_, bf3, ba3) = Measure(() => EventQueries.Q2_QuerySyntax(bigEvents, bigVenues), BigRepeats);
var (_, bf4, ba4) = Measure(() => EventQueries.Q2_MethodSyntax(bigEvents, bigVenues), BigRepeats);
var (_, bf5, ba5) = Measure(() => EventQueries.Q3_QuerySyntax(bigEvents, bigVenues), BigRepeats);
var (_, bf6, ba6) = Measure(() => EventQueries.Q3_MethodSyntax(bigEvents, bigVenues), BigRepeats);
var (_, bf7, ba7) = Measure(() => EventQueries.Q4_QuerySyntax(bigEvents, today), BigRepeats);
var (_, bf8, ba8) = Measure(() => EventQueries.Q4_MethodSyntax(bigEvents, today), BigRepeats);

bigTimings.Add(("1. Фільтр + join", "запитів", bf1, ba1));
bigTimings.Add(("1. Фільтр + join", "методів", bf2, ba2));
bigTimings.Add(("2. Join", "запитів", bf3, ba3));
bigTimings.Add(("2. Join", "методів", bf4, ba4));
bigTimings.Add(("3. GroupJoin + Count", "запитів", bf5, ba5));
bigTimings.Add(("3. GroupJoin + Count", "методів", bf6, ba6));
bigTimings.Add(("4. Найближча подія", "запитів (OrderBy+First)", bf7, ba7));
bigTimings.Add(("4. Найближча подія", "методів (MinBy)", bf8, ba8));

Console.WriteLine();
Console.WriteLine($"===== Час виконання на великому наборі (1 000 локацій, 100 000 подій), середнє з {BigRepeats} запусків =====");
PrintTimings(bigTimings);


static (T Result, double FirstUs, double AvgUs) Measure<T>(Func<T> query, int repeats)
{
    var first = Stopwatch.StartNew();
    T result = query();
    first.Stop();

    var warm = Stopwatch.StartNew();
    for (int i = 0; i < repeats; i++)
        query();
    warm.Stop();

    return (result,
            first.Elapsed.TotalMilliseconds * 1000,
            warm.Elapsed.TotalMilliseconds * 1000 / repeats);
}

static void PrintCheck(bool same, int count) =>
    Console.WriteLine($"   Записів: {count}. Синтаксис запитів і методів дали однаковий результат: {(same ? "так" : "НІ")}");

static void PrintTimings(List<(string Query, string Syntax, double FirstUs, double AvgUs)> rows)
{
    Console.WriteLine($"{"Запит",-22} {"Синтаксис",-26} {"Перший, мкс",12} {"Середній, мкс",14}");
    foreach (var (query, syntax, firstUs, avgUs) in rows)
        Console.WriteLine($"{query,-22} {syntax,-26} {firstUs,12:F1} {avgUs,14:F2}");
}