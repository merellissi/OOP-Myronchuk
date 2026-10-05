namespace IndependentWork1;

public class Playlist
{
    private string _title;
    private List<string> _tracks;
    private int _totalMinutes;

    public string Title
    {
        get { return _title; }
        set { _title = value; }
    }

    public int TrackCount
    {
        get { return _tracks.Count; }
    }

    public int TotalMinutes
    {
        get { return _totalMinutes; }
    }

    public Playlist(string title)
    {
        _title = title;
        _tracks = new List<string>();
        _totalMinutes = 0;
    }

    public void AddTrack(string trackName, int minutes)
    {
        if (string.IsNullOrWhiteSpace(trackName) || minutes <= 0)
        {
            Console.WriteLine("Трек не додано: некоректні дані.");
            return;
        }

        _tracks.Add(trackName);
        _totalMinutes += minutes;
    }

    public string GetDurationText()
    {
        return $"{_totalMinutes / 60} год {_totalMinutes % 60} хв";
    }

    public void PrintTracks()
    {
        Console.WriteLine($"Плейлист \"{_title}\":");
        for (int i = 0; i < _tracks.Count; i++)
        {
            Console.WriteLine($"  {i + 1}. {_tracks[i]}");
        }
    }
}