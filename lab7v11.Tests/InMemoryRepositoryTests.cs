using lab7v11;
using Xunit;

namespace lab7v11.Tests;

public class InMemoryRepositoryTests
{
    private static Movie NewMovie(string title = "Test") =>
        new Movie { Title = title, Director = "Dir", ReleaseYear = 2000 };

    // ---------- Add ----------
    [Fact]
    public void Add_NewItem_IsStoredAndGetsId()
    {
        var repo = new InMemoryRepository<Movie>();
        var movie = NewMovie();

        repo.Add(movie);

        Assert.Equal(1, movie.Id);
        Assert.Single(repo.GetAll());
    }

    [Fact]
    public void Add_TwoItems_GetDifferentIds()
    {
        var repo = new InMemoryRepository<Movie>();
        var a = NewMovie("A");
        var b = NewMovie("B");

        repo.Add(a);
        repo.Add(b);

        Assert.NotEqual(a.Id, b.Id);
    }

    // ---------- GetById ----------
    [Fact]
    public void GetById_ExistingId_ReturnsItem()
    {
        var repo = new InMemoryRepository<Movie>();
        var movie = NewMovie("Matrix");
        repo.Add(movie);

        var result = repo.GetById(movie.Id);

        Assert.NotNull(result);
        Assert.Equal("Matrix", result!.Title);
    }

    [Fact]
    public void GetById_NonExistingId_ReturnsNull()
    {
        var repo = new InMemoryRepository<Movie>();

        Assert.Null(repo.GetById(999));
    }

    // ---------- GetAll ----------
    [Fact]
    public void GetAll_EmptyRepository_ReturnsEmpty()
    {
        var repo = new InMemoryRepository<Movie>();

        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void GetAll_WithItems_ReturnsAll()
    {
        var repo = new InMemoryRepository<Movie>();
        repo.Add(NewMovie("A"));
        repo.Add(NewMovie("B"));
        repo.Add(NewMovie("C"));

        Assert.Equal(3, repo.GetAll().Count());
    }

    // ---------- Update ----------
    [Fact]
    public void Update_ExistingItem_ChangesData()
    {
        var repo = new InMemoryRepository<Movie>();
        var movie = NewMovie("Old");
        repo.Add(movie);

        repo.Update(new Movie { Id = movie.Id, Title = "New", Director = "D2", ReleaseYear = 2020 });

        var result = repo.GetById(movie.Id);
        Assert.Equal("New", result!.Title);
        Assert.Equal(2020, result.ReleaseYear);
    }

    [Fact]
    public void Update_NonExistingItem_ThrowsKeyNotFoundException()
    {
        var repo = new InMemoryRepository<Movie>();
        var ghost = new Movie { Id = 42, Title = "Ghost" };

        Assert.Throws<KeyNotFoundException>(() => repo.Update(ghost));
    }

    // ---------- Delete ----------
    [Fact]
    public void Delete_ExistingItem_RemovesIt()
    {
        var repo = new InMemoryRepository<Movie>();
        var movie = NewMovie();
        repo.Add(movie);

        repo.Delete(movie.Id);

        Assert.Null(repo.GetById(movie.Id));
        Assert.Empty(repo.GetAll());
    }

    [Fact]
    public void Delete_NonExistingItem_ThrowsKeyNotFoundException()
    {
        var repo = new InMemoryRepository<Movie>();

        Assert.Throws<KeyNotFoundException>(() => repo.Delete(999));
    }
}