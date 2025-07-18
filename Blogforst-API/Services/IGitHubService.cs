using Blogforst_API.Models;

namespace Blogforst_API.Services;

public interface IGitHubService
{
    Task<List<GitHubActivity>> GetRecentActivitiesAsync();
}