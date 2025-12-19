using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Octokit;

namespace GitHubCV.Services;

public class CachedGitHubService : IGitHubService
{
    private readonly IGitHubService _innerService;
    private readonly IMemoryCache _cache;
    private readonly GitHubClient _client;
    private readonly GitHubSettings _settings;
    private const string PortfolioKey = "portfolio_cache";

    public CachedGitHubService(IGitHubService innerService, IMemoryCache cache, IOptions<GitHubSettings> options)
    {
        _innerService = innerService;
        _cache = cache;
        _settings = options.Value;
        _client = new GitHubClient(new ProductHeaderValue("GitHubCV"));
        if (!string.IsNullOrEmpty(_settings.Token)) _client.Credentials = new Credentials(_settings.Token);
    }

    public async Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync()
    {
        // אם המידע קיים בזיכרון, נחזיר אותו מיד
        if (_cache.TryGetValue(PortfolioKey, out IEnumerable<RepositoryInfo>? cachedData))
        {
            return cachedData!;
        }

        // אם לא, נשלוף מ-GitHub דרך השירות הפנימי
        var freshData = await _innerService.GetPortfolioAsync();

        // נשמור בזיכרון ל-5 דקות (כפי שנדרש בהוראות)
        var cacheOptions = new MemoryCacheEntryOptions()
            .SetAbsoluteExpiration(TimeSpan.FromMinutes(5));

        _cache.Set(PortfolioKey, freshData, cacheOptions);

        return freshData;
    }

    // שאר הפונקציות פשוט קוראות לשירות המקורי ללא Cache
    public Task<IEnumerable<RepositoryInfo>> GetRepositoriesAsync(string username) => _innerService.GetRepositoriesAsync(username);
    public Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? repoName, string? language, string? user) => _innerService.SearchRepositoriesAsync(repoName, language, user);
}