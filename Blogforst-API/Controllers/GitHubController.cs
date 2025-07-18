using Microsoft.AspNetCore.Mvc;
using Blogforst_API.Services;
 
namespace Blogforst_API.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class GitHubController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;
        private readonly ILogger<GitHubController> _logger;
 
        public GitHubController(IGitHubService gitHubService, ILogger<GitHubController> logger)
        {
            _gitHubService = gitHubService;
            _logger = logger;
        }
 
        [HttpGet("activities")]
        public async Task<IActionResult> GetRecentActivities()
        {
            try
            {
                var activities = await _gitHubService.GetRecentActivitiesAsync();
                return Ok(activities);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in GetRecentActivities");
                return StatusCode(500, new { message = "获取GitHub活动失败" });
            }
        }
    }
}