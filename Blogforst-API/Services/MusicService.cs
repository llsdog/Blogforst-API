using Blogforst_API.Models;
using Blogforst_API.Services;
using System.Text.Json;
using Blogforst_API.Services;

namespace Blogforst_API.Services
{
    public class MusicService : IMusicService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<MusicService> _logger;

        public MusicService(HttpClient httpClient, IConfiguration configuration, ILogger<MusicService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<Song>> GetPlaylistAsync(string playlistId)
        {
            try
            {
                var baseUrl = _configuration["CloudMusic:BaseURL"];
                var response = await _httpClient.GetAsync($"{baseUrl}/playlist/track/all?id={playlistId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch playlist: {StatusCode}", response.StatusCode);
                    return new List<Song>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var playlistData = JsonSerializer.Deserialize<JsonElement>(content);

                if (playlistData.GetProperty("code").GetInt32() != 200)
                {
                    _logger.LogError("API returned error code: {Code}", playlistData.GetProperty("code").GetInt32());
                    return new List<Song>();
                }

                var songs = new List<Song>();
                foreach (var songElement in playlistData.GetProperty("songs").EnumerateArray())
                {
                    var artists = songElement.GetProperty("ar").EnumerateArray()
                        .Select(ar => ar.GetProperty("name").GetString())
                        .Where(name => !string.IsNullOrEmpty(name))
                        .ToArray();

                    songs.Add(new Song
                    {
                        Id = songElement.GetProperty("id").GetInt64(),
                        Name = songElement.GetProperty("name").GetString() ?? "",
                        Artist = string.Join(" / ", artists),
                        Album = songElement.GetProperty("al").GetProperty("name").GetString() ?? "",
                        AlbumPic = songElement.GetProperty("al").GetProperty("picUrl").GetString() ?? "",
                        Duration = songElement.GetProperty("dt").GetInt64()
                    });
                }

                return songs;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching playlist {PlaylistId}", playlistId);
                return new List<Song>();
            }
        }

        public async Task<string?> GetSongUrlAsync(string songId)
        {
            try
            {
                var baseUrl = _configuration["CloudMusic:BaseURL"];
                var response = await _httpClient.GetAsync($"{baseUrl}/song/url?id={songId}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch song URL: {StatusCode}", response.StatusCode);
                    return null;
                }

                var content = await response.Content.ReadAsStringAsync();
                var urlData = JsonSerializer.Deserialize<JsonElement>(content);

                if (urlData.GetProperty("code").GetInt32() == 200 && 
                    urlData.GetProperty("data").EnumerateArray().Any())
                {
                    return urlData.GetProperty("data")[0].GetProperty("url").GetString();
                }

                return null;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching song URL {SongId}", songId);
                return null;
            }
        }
    }
}
