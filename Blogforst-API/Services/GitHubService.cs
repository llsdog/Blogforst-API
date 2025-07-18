using Blogforst_API.Models;
using Blogforst_API.Services;
using System.Text.Json;

namespace Blogforst_API.Services
{
    public class GitHubService : IGitHubService
    {
        private readonly HttpClient _httpClient;
        private readonly IConfiguration _configuration;
        private readonly ILogger<GitHubService> _logger;

        public GitHubService(HttpClient httpClient, IConfiguration configuration, ILogger<GitHubService> logger)
        {
            _httpClient = httpClient;
            _configuration = configuration;
            _logger = logger;
        }

        public async Task<List<GitHubActivity>> GetRecentActivitiesAsync()
        {
            try
            {
                var token = _configuration["GitHub:Token"];
                var username = _configuration["GitHub:Username"];
                var timestamp = DateTimeOffset.UtcNow.ToUnixTimeMilliseconds();

                _httpClient.DefaultRequestHeaders.Clear();
                _httpClient.DefaultRequestHeaders.Add("Accept", "application/vnd.github+json");
                _httpClient.DefaultRequestHeaders.Add("Authorization", $"Bearer {token}");
                _httpClient.DefaultRequestHeaders.Add("X-GitHub-Api-Version", "2022-11-28");
                _httpClient.DefaultRequestHeaders.Add("User-Agent", "Blogforst_API");

                var response = await _httpClient.GetAsync($"https://api.github.com/users/{username}/events?t={timestamp}");
                
                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogError("Failed to fetch GitHub events: {StatusCode}", response.StatusCode);
                    return new List<GitHubActivity>();
                }

                var content = await response.Content.ReadAsStringAsync();
                var events = JsonSerializer.Deserialize<List<GitHubEvent>>(content, new JsonSerializerOptions
                {
                    PropertyNameCaseInsensitive = true
                });

                return events?.Take(5).Select(ProcessEvent).ToList() ?? new List<GitHubActivity>();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GitHub activities");
                return new List<GitHubActivity>();
            }
        }

        private GitHubActivity ProcessEvent(GitHubEvent gitHubEvent)
        {
            return new GitHubActivity
            {
                Type = gitHubEvent.Type,
                Repo = gitHubEvent.Repo.Name,
                Message = GetActivityMessage(gitHubEvent),
                TimeAgo = GetTimeAgo(gitHubEvent.Created_at)
            };
        }

        private string GetActivityMessage(GitHubEvent gitHubEvent)
        {
            return gitHubEvent.Type switch
            {
                "PushEvent" => gitHubEvent.Payload.Commits.FirstOrDefault()?.Message?.Substring(0, Math.Min(40, gitHubEvent.Payload.Commits.FirstOrDefault()?.Message?.Length ?? 0)) + "..." ?? "No commit message",
                "PullRequestEvent" => $"{gitHubEvent.Payload.Action}了一个拉取请求: {gitHubEvent.Payload.Pull_request?.Title}",
                "IssueCommentEvent" => gitHubEvent.Payload.Comment?.Body?.Substring(0, Math.Min(50, gitHubEvent.Payload.Comment.Body.Length)) + "..." ?? "",
                "WatchEvent" => "Star了此仓库",
                "ForkEvent" => "Fork了此仓库",
                "CreateEvent" => $"创建了{gitHubEvent.Payload.Ref_type}:{gitHubEvent.Payload.Ref ?? ""}",
                "DeleteEvent" => $"删除了{gitHubEvent.Payload.Ref_type}:{gitHubEvent.Payload.Ref}",
                _ => $"执行了{gitHubEvent.Type}操作"
            };
        }

        private string GetTimeAgo(DateTime eventTime)
        {
            var diff = DateTime.UtcNow - eventTime;
            
            return diff.TotalMinutes switch
            {
                < 60 => $"{(int)diff.TotalMinutes}分钟前",
                < 1440 => $"{(int)diff.TotalHours}小时前",
                _ => $"{(int)diff.TotalDays}天前"
            };
        }
    }
}
