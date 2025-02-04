using Microsoft.AspNetCore.Mvc;
using Service;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace APICV.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly IGitHubService _gitHubService;
        public UserController(IGitHubService gitHubService)
        {
            _gitHubService = gitHubService;
        }

        // GET api/<ValuesController>/5
        [HttpGet("{userName}/PublicRepositiories")]
        public async Task<int> GetUserPublicRepositioriesAsync(string userName)
        {
            return await _gitHubService.GetUserPublicRepositioriesAsync(userName);
        }

        [HttpGet("/portfolio")]
        public async Task<List<Portfolio>> GetPortfolio()
        {
            return await _gitHubService.GetPortfolio();
        }
        [HttpGet("{userName}/followers")]
        public async Task<int> GetUserFollowersAsync(string userName)
        {
            return await _gitHubService.GetUserFollowersAsync(userName);
        }

        [HttpGet("{userName}")]
        public async Task<List<SearchRepositoryInfo>> SearchRepositories(string repoName = null, string language = null, string userName = null)
        {
            return await _gitHubService.SearchRepositories(repoName, language, userName);
        }
    }
}
