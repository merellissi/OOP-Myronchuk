namespace Lab10v11;


public static class TestData
{
    public static List<Venue> Venues() => new()
    {
        new Venue { Id = 1,  Name = "Палац «Україна»",          Capacity = 3700 },
        new Venue { Id = 2,  Name = "Клуб «Docker Pub»",        Capacity = 400 },
        new Venue { Id = 3,  Name = "Atlas",                    Capacity = 1500 },
        new Venue { Id = 4,  Name = "Львівська опера",          Capacity = 1100 },
        new Venue { Id = 5,  Name = "Рівненська філармонія",    Capacity = 450 },
        new Venue { Id = 6,  Name = "Stereo Plaza",             Capacity = 3000 },
        new Venue { Id = 7,  Name = "Мистецький Арсенал",       Capacity = 2000 },
        new Venue { Id = 8,  Name = "Коворкінг «Hub»",          Capacity = 120 },
        new Venue { Id = 9,  Name = "Стадіон «Авангард»",       Capacity = 4500 },
        new Venue { Id = 10, Name = "Луцький будинок культури", Capacity = 350 }
    };

    public static List<Event> Events() => new()
    {
        new Event { Id = 1,  Name = "IT Conference Rivne",          Date = new DateTime(2024, 3, 15),  LocationId = 9 },
        new Event { Id = 2,  Name = "Джазовий вечір",               Date = new DateTime(2024, 4, 20),  LocationId = 5 },
        new Event { Id = 3,  Name = "Концерт гурту «Антитіла»",     Date = new DateTime(2024, 6, 8),   LocationId = 6 },
        new Event { Id = 4,  Name = "Хакатон OOP",                  Date = new DateTime(2024, 9, 12),  LocationId = 2 },
        new Event { Id = 5,  Name = "Опера «Наталка Полтавка»",     Date = new DateTime(2024, 10, 5),  LocationId = 4 },
        new Event { Id = 6,  Name = "Stand-up шоу",                 Date = new DateTime(2023, 12, 1),  LocationId = 3 },
        new Event { Id = 7,  Name = "Новорічний бал",               Date = new DateTime(2024, 12, 28), LocationId = 1 },
        new Event { Id = 8,  Name = "Виставка сучасного мистецтва", Date = new DateTime(2025, 2, 14),  LocationId = 7 },
        new Event { Id = 9,  Name = "Фестиваль вуличної їжі",       Date = new DateTime(2025, 5, 24),  LocationId = 9 },
        new Event { Id = 10, Name = "Літературний вечір",           Date = new DateTime(2025, 8, 30),  LocationId = 10 },
        new Event { Id = 11, Name = ".NET Meetup",                  Date = new DateTime(2025, 10, 18), LocationId = 3 },
        new Event { Id = 12, Name = "Благодійний концерт",          Date = new DateTime(2026, 3, 8),   LocationId = 1 },
        new Event { Id = 13, Name = "Кінопоказ просто неба",        Date = new DateTime(2026, 7, 11),  LocationId = 9 },
        new Event { Id = 14, Name = "Книжковий ярмарок",            Date = new DateTime(2026, 9, 19),  LocationId = 7 },
        new Event { Id = 15, Name = "Рок-фестиваль",                Date = new DateTime(2026, 11, 21), LocationId = 6 },
        new Event { Id = 16, Name = "Різдвяний ярмарок",            Date = new DateTime(2026, 12, 19), LocationId = 7 },
        new Event { Id = 17, Name = "Симфонічний концерт",          Date = new DateTime(2027, 1, 23),  LocationId = 4 },
        new Event { Id = 18, Name = "Квіз-вечірка",                 Date = new DateTime(2027, 2, 6),   LocationId = 2 },
        new Event { Id = 19, Name = "Гейм-джем",                    Date = new DateTime(2027, 3, 13),  LocationId = 5 },
        new Event { Id = 20, Name = "Весняний марафон",             Date = new DateTime(2027, 4, 25),  LocationId = 9 }
    };

    
    public static (List<Venue> Venues, List<Event> Events) Generate(int venueCount, int eventCount, int seed = 2026)
    {
        var random = new Random(seed);

        var venues = Enumerable.Range(1, venueCount)
            .Select(id => new Venue { Id = id, Name = $"Локація {id}", Capacity = random.Next(50, 5001) })
            .ToList();

        var start = new DateTime(2023, 1, 1);
        var events = Enumerable.Range(1, eventCount)
            .Select(id => new Event
            {
                Id = id,
                Name = $"Подія {id}",
                Date = start.AddDays(random.Next(5 * 365)),   
                LocationId = random.Next(1, venueCount + 1)
            })
            .ToList();

        return (venues, events);
    }
}