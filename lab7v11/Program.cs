using lab7v11;

var repo = new InMemoryRepository<Movie>();

repo.Add(new Movie { Title = "Inception", Director = "Christopher Nolan", ReleaseYear = 2010 });
repo.Add(new Movie { Title = "Pulp Fiction", Director = "Quentin Tarantino", ReleaseYear = 1994 });
repo.Add(new Movie { Title = "The Matrix", Director = "Wachowski", ReleaseYear = 1999 });

Console.WriteLine("Усі фільми:");
foreach (var m in repo.GetAll()) Console.WriteLine(m);

Console.WriteLine("\nФільм з Id=2:");
Console.WriteLine(repo.GetById(2));

repo.Update(new Movie { Id = 2, Title = "Pulp Fiction (Remastered)", Director = "Quentin Tarantino", ReleaseYear = 1994 });
Console.WriteLine("\nПісля оновлення Id=2:");
Console.WriteLine(repo.GetById(2));

repo.Delete(1);
Console.WriteLine("\nПісля видалення Id=1:");
foreach (var m in repo.GetAll()) Console.WriteLine(m);

Console.WriteLine("\nНеіснуючий Id=99: " + (repo.GetById(99)?.ToString() ?? "null"));