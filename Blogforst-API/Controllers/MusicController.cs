using Microsoft.AspNetCore.Mvc;
using Blogforst_API.Services;
using Blogforst_API.Models;

namespace Blogforst_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class MusicController : ControllerBase
    {
        private readonly IMusicService _musicService;
        private readonly ILogger<MusicController> _logger;
 
        public MusicController(IMusicService musicService, ILogger<MusicController> logger)
        {
            _musicService = musicService;
            _logger = logger;
        }
 
        [HttpGet("playlist/{playlistId}")]
        public async Task<IActionResult> GetPlaylist(string playlistId)
        {
            try
            {
                var songs = await _musicService.GetPlaylistAsync(playlistId);
                
                var formattedSongs = songs.Select(song => new
                {
                    id = song.Id,
                    name = song.Name,
                    ar = new[] { new { name = song.Artist } },
                    al = new { name = song.Album, picUrl = song.AlbumPic },
                    dt = song.Duration
                }).ToList();
        
                return Ok(new { code = 200, songs = formattedSongs });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetPlaylist");
                return StatusCode(500, new { code = 500, message = "获取歌单失败" });
            }
        }
 
        [HttpGet("song-url/{songId}")]
        public async Task<IActionResult> GetSongUrl(string songId)
        {
            try
            {
                var url = await _musicService.GetSongUrlAsync(songId);
                if (string.IsNullOrEmpty(url))
                {
                    return NotFound(new { code = 404, message = "获取歌曲URL失败" });
                }
 
                return Ok(new { code = 200, data = new[] { new { url = url } } });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetSongUrl");
                return StatusCode(500, new { code = 500, message = "获取歌曲URL失败" });
            }
        }
    }
}