namespace Blogforst_API.Models;

public class GitHubActivity
{
    public string Type { get; set; } = string.Empty;
    public string Repo { get; set; } = string.Empty;
    public string Message { get; set; } = string.Empty;
    public string TimeAgo { get; set; } = string.Empty;
}

public class GitHubEvent
{
    public string Type { get; set; } = string.Empty;
    public GitHubRepo Repo { get; set; } = new();
    public GitHubPayload Payload { get; set; } = new();
    public DateTime Created_at { get; set; }
}

public class GitHubRepo
{
    public string Name { get; set; } = string.Empty;
}

public class GitHubPayload
{
    public List<GitHubCommit> Commits { get; set; } = new();
    public GitHubPullRequest? Pull_request { get; set; }
    public GitHubComment? Comment { get; set; }
    public string? Action { get; set; }
    public string? Ref_type { get; set; }
    public string? Ref { get; set; }
}

public class GitHubCommit
{
    public string Message { get; set; } = string.Empty;
}
 
public class GitHubPullRequest
{
    public string Title { get; set; } = string.Empty;
}
 
public class GitHubComment
{
    public string Body { get; set; } = string.Empty;
}