using Blogforst_API.Models;
 
namespace Blogforst_API.Services
{
    public interface IMusicService
    {
        Task<List<Song>> GetPlaylistAsync(string playlistId);
        Task<string?> GetSongUrlAsync(string songId);
    }
}