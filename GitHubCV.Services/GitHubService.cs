using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.Extensions.Options;
using Octokit;

namespace GitHubCV.Services;

public class GitHubService : IGitHubService
{
    private readonly GitHubClient _client;
    private readonly GitHubSettings _settings;

    public GitHubService(IOptions<GitHubSettings> options)
    {
        _settings = options?.Value ?? new GitHubSettings();
        _client = new GitHubClient(new ProductHeaderValue("GitHubCV"));
        if (!string.IsNullOrEmpty(_settings.Token))
        {
            _client.Credentials = new Credentials(_settings.Token);
        }
    }

    // מימוש הפונקציה שהייתה חסרה וגרמה לשגיאה
    public async Task<IEnumerable<RepositoryInfo>> GetRepositoriesAsync(string username)
    {
        var repos = await _client.Repository.GetAllForUser(username);
        return await MapReposAsync(repos);
    }

    private async Task<IEnumerable<RepositoryInfo>> MapReposAsync(IEnumerable<Repository> repos)
    {
        var result = new List<RepositoryInfo>();
        foreach (var r in repos)
        {
            var info = new RepositoryInfo
            {
                Name = r.Name,
                Description = r.Description,
                Url = r.HtmlUrl,
                Homepage = r.Homepage,
                Stars = r.StargazersCount, // הוספת כוכבים (דרישת פרויקט)
                LastCommitDate = DateTimeOffset.MinValue,
                Languages = new Dictionary<string, long>()
            };

            try
            {
                // שליפת קומיט אחרון
                var commits = await _client.Repository.Commit.GetAll(r.Owner.Login, r.Name);
                var lastCommit = commits.FirstOrDefault();
                if (lastCommit != null)
                {
                    info.LastCommitDate = lastCommit.Commit?.Committer?.Date ?? DateTimeOffset.MinValue;
                }

                // שליפת שפות
                var languagesRaw = await _client.Repository.GetAllLanguages(r.Owner.Login, r.Name);
                info.Languages = languagesRaw.ToDictionary(l => l.Name, l => l.NumberOfBytes);

                // שליפת Pull Requests (דרישת פרויקט)
                var prs = await _client.PullRequest.GetAllForRepository(r.Owner.Login, r.Name);
                info.PullRequestsCount = prs.Count;
            }
            catch (ApiException) { /* דילוג על פרויקטים ריקים */ }

            result.Add(info);
        }
        return result;
    }

    public async Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync()
    {
        if (string.IsNullOrEmpty(_settings.Username)) return Enumerable.Empty<RepositoryInfo>();
        var repos = await _client.Repository.GetAllForUser(_settings.Username);
        return await MapReposAsync(repos);
    }

    // עדכון לחיפוש גלובלי בכל GitHub לפי דרישות הפרויקט
    public async Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? repoName, string? language, string? user)
    {
        // תיקון: אם שם הפרויקט ריק, נשתמש בסימן "*" כדי לאפשר חיפוש לפי פרמטרים אחרים (כמו יוזר בלבד)
        var searchTerm = string.IsNullOrWhiteSpace(repoName) ? "*" : repoName;

        var request = new SearchRepositoriesRequest(searchTerm)
        {
            Language = string.IsNullOrEmpty(language) ? null : (Language?)Enum.Parse(typeof(Language), language, true),
            User = user
        };

        var searchResult = await _client.Search.SearchRepo(request);
        return await MapReposAsync(searchResult.Items);
    }
}

public class RepositoryInfo
{
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public string Url { get; set; } = string.Empty;
    public string? Homepage { get; set; }
    public int Stars { get; set; }
    public int PullRequestsCount { get; set; }
    public DateTimeOffset LastCommitDate { get; set; }
    public Dictionary<string, long> Languages { get; set; } = new();
}