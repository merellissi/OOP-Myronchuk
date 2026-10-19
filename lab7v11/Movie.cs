namespace lab7v11;

public class Movie : IEntity
{
    public int Id { get; set; }
    public string Title { get; set; } = "";
    public string Director { get; set; } = "";
    public int ReleaseYear { get; set; }

    public override string ToString() =>
        $"[{Id}] {Title}, реж. {Director}, {ReleaseYear}";
}