namespace GitHubCV.Services;

public interface IGitHubService
{
 Task<IEnumerable<RepositoryInfo>> GetRepositoriesAsync(string username);
 Task<IEnumerable<RepositoryInfo>> GetPortfolioAsync();
 Task<IEnumerable<RepositoryInfo>> SearchRepositoriesAsync(string? repoName, string? language, string? user);
}
