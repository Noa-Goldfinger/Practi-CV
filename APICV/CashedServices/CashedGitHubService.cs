using Microsoft.Extensions.Caching.Memory;
using Service;

namespace APICV.CashedServices
{
    public class CashedGitHubService : IGitHubService
    {
        private readonly IGitHubService _gitHubService;
        private readonly IMemoryCache _memoryCache;
        private const string UserPortfolioKey = "UserPortfolioKey";
        public CashedGitHubService(IGitHubService gitHubService, IMemoryCache memoryCache)
        {
            _gitHubService = gitHubService;
            _memoryCache = memoryCache;
        }

        public async Task<List<Portfolio>> GetPortfolio()
        {
            if (_memoryCache.TryGetValue(UserPortfolioKey, out List<Portfolio> portfolio))
                return portfolio;

            var cacheOptions = new MemoryCacheEntryOptions()
                .SetAbsoluteExpiration(TimeSpan.FromMinutes(2))
                .SetSlidingExpiration(TimeSpan.FromMinutes(1));

            portfolio = await _gitHubService.GetPortfolio();
            _memoryCache.Set(UserPortfolioKey, portfolio);
            return portfolio;
        }

        public Task<int> GetUserFollowersAsync(string userName)
        {
            return _gitHubService.GetUserFollowersAsync(userName);
        }

        public Task<int> GetUserPublicRepositioriesAsync(string userName)
        {
            return _gitHubService.GetUserPublicRepositioriesAsync(userName);
        }

        public Task<List<SearchRepositoryInfo>> SearchRepositories(string repoName = null, string language = null, string userName = null)
        {
            return _gitHubService.SearchRepositories(repoName, language, userName);
        }
    }
}
