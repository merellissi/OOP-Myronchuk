# Лабораторна робота №7. Generics, IRepository

Варіант 11, сутність Movie.

- **IEntity** — інтерфейс з властивістю `int Id`.
- **Movie** — сутність: Title, Director, ReleaseYear.
- **IRepository<T>** — узагальнений інтерфейс CRUD з обмеженням `where T : class, IEntity`.
- **InMemoryRepository<T>** — реалізація, що зберігає дані в `List<T>`.

Запуск тестів: `dotnet test lab7v11.Tests`