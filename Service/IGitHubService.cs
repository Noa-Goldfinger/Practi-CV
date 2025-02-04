using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public interface IGitHubService
    {
        Task<int> GetUserFollowersAsync(string userName);
        Task<int> GetUserPublicRepositioriesAsync(string userName);
        Task<List<Portfolio>> GetPortfolio();
        Task<List<SearchRepositoryInfo>> SearchRepositories(string repoName = null, string language = null, string userName = null);
    }
}
