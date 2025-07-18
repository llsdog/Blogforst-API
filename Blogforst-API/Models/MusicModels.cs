namespace Blogforst_API.Models;

public class PlaylistResponse
{
    public int Code { get; set; }
    public List<Song> Songs { get; set; } = new();
    public string? Message { get; set; }
}

public class Song
{
    public long Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string Artist { get; set; } = string.Empty;
    public string Album { get; set; } = string.Empty;
    public string AlbumPic { get; set; } = string.Empty;
    public long Duration { get; set; }
}

public class SongUrlResponse
{
    public int Code { get; set; }
    public List<SongUrlData> Data { get; set; } = new();
}

public class SongUrlData
{
    public string? Url { get; set; }
}