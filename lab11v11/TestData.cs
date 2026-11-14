namespace Lab11v11;

public static class TestData
{
    private static int _ticketId;

    private static Ticket T(string type, decimal price) => new() { Id = ++_ticketId, Type = type, Price = price };

    public static List<Event> Events()
    {
        _ticketId = 0;

        return new List<Event>
        {
            new() { Id = 1, Name = "IT Conference Rivne", Participants = new()
            {
                new() { Id = 1, Name = "Олена Коваленко", Tickets = new() { T("Стандарт", 800), T("Воркшоп", 400) } },
                new() { Id = 2, Name = "Андрій Мельник",  Tickets = new() { T("VIP", 2500) } },
                new() { Id = 3, Name = "Софія Ткаченко",  Tickets = new() { T("Студентський", 300) } }
            }},
            new() { Id = 2, Name = "Концерт гурту «Антитіла»", Participants = new()
            {
                new() { Id = 4, Name = "Максим Шевченко", Tickets = new() { T("Фан-зона", 1200) } },
                new() { Id = 5, Name = "Ірина Бондар",    Tickets = new() { T("VIP", 3500), T("Мерч-пакет", 600) } }
            }},
            new() { Id = 3, Name = "Опера «Наталка Полтавка»", Participants = new()
            {
                new() { Id = 6, Name = "Богдан Лисенко",  Tickets = new() { T("Партер", 900) } },
                new() { Id = 7, Name = "Наталія Руденко", Tickets = new() { T("Ложа", 1500) } },
                new() { Id = 8, Name = "Іван Мороз",      Tickets = new() { T("Балкон", 450) } }
            }},
            new() { Id = 4, Name = "Фестиваль вуличної їжі", Participants = new()
            {
                new() { Id = 9,  Name = "Анна Савчук",    Tickets = new() { T("Вхідний", 200) } },
                new() { Id = 10, Name = "Дмитро Олійник", Tickets = new() { T("Вхідний", 200), T("Дегустаційний сет", 350) } }
            }},
            new() { Id = 5, Name = ".NET Meetup", Participants = new()
            {
                new() { Id = 11, Name = "Олександр Кравченко", Tickets = new() { T("Безкоштовний", 0) } },
                new() { Id = 12, Name = "Марія Поліщук",       Tickets = new() { T("Стандарт", 150) } }
            }},
            new() { Id = 6, Name = "Рок-фестиваль", Participants = new()
            {
                new() { Id = 13, Name = "Віктор Гнатюк", Tickets = new() { T("3 дні", 2800) } },
                new() { Id = 14, Name = "Юлія Петренко", Tickets = new() { T("1 день", 1100) } },
                new() { Id = 15, Name = "Тарас Бойко",   Tickets = new() { T("3 дні", 2800), T("Кемпінг", 700) } }
            }},
            new() { Id = 7, Name = "Різдвяний ярмарок", Participants = new()
            {
                new() { Id = 16, Name = "Оксана Мельничук", Tickets = new() { T("Вхідний", 100) } }
            }},
            new() { Id = 8, Name = "Симфонічний концерт", Participants = new()
            {
                new() { Id = 17, Name = "Роман Зінченко",   Tickets = new() { T("Партер", 700) } },
                new() { Id = 18, Name = "Катерина Левченко", Tickets = new() { T("Партер", 700), T("Партер", 700) } }
            }},
            new() { Id = 9, Name = "Гейм-джем", Participants = new()
            {
                new() { Id = 19, Name = "Павло Кузьменко",    Tickets = new() { T("Учасник", 250) } },
                new() { Id = 20, Name = "Вікторія Остапчук",  Tickets = new() { T("Учасник", 250) } },
                new() { Id = 21, Name = "Денис Гаврилюк",     Tickets = new() { T("Глядач", 100) } }
            }}
        };
    }
}