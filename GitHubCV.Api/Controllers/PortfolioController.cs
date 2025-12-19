using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using GitHubCV.Services;

namespace GitHubCV.Api.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PortfolioController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;

        public PortfolioController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        [HttpGet]
        public async Task<IActionResult> GetPortfolio()
        {
            var result = await _gitHubService.GetPortfolioAsync();
            return Ok(result);
        }

        [HttpGet("search")]
        public async Task<IActionResult> Search(string? repoName, string? language, string? user)
        {
            var result = await _gitHubService.SearchRepositoriesAsync(repoName, language, user);
            return Ok(result);
        }
    }
}
