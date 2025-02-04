using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Octokit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Service
{
    public class GitHubService:IGitHubService
    {
        private readonly GitHubClient _publicClient;
        private readonly GitHubClient _authenticatedClient;
        private readonly GitHubIntegrationOptions _options;
        //private readonly IConfiguration _configuration;

        //public GitHubService(IConfiguration configuration)
        public GitHubService(IOptions<GitHubIntegrationOptions> options)
        {
            _options = options.Value;

            //חיבור ציבורי
            _publicClient = new GitHubClient(new ProductHeaderValue("my-github-app"));

            // חיבור מאומת
            _authenticatedClient = new GitHubClient(new ProductHeaderValue("AuthenticatedGitHubClient"))
            {
                Credentials = new Credentials(_options.Token) // התחברות עם טוקן אישי
            };

            // _configuration = configuration;
        }
        public async Task<int> GetUserFollowersAsync(string userName)
        {
            var user = await _publicClient.User.Get(userName);
            Console.WriteLine(user.Followers + " followers!");
            return user.Followers;
        }
        public async Task<int> GetUserPublicRepositioriesAsync(string userName)
        {
            var user = await _publicClient.User.Get(userName);
            return user.PublicRepos;
        }
        public async Task<List<Portfolio>> GetPortfolio()
        {
            var repositories = await _authenticatedClient.Repository.GetAllForCurrent();
            var portfolio = new List<Portfolio>();

            foreach (var repo in repositories)
            {
                var languages = await _authenticatedClient.Repository.GetAllLanguages(repo.Id);
                portfolio.Add(new Portfolio
                {
                    Name = repo.Name,
                    Url = repo.HtmlUrl,
                    LastCommitDate = repo.UpdatedAt.DateTime,
                    StarCount = repo.StargazersCount,
                    PullRequestCount = (await _authenticatedClient.PullRequest.GetAllForRepository(repo.Owner.Login, repo.Name)).Count,
                    Languages = languages.Select(l => l.Name).ToList()
                });
            }

            return portfolio;
        }

        //public async Task<List<SearchRepositoryInfo>> SearchRepositories(string repoName = null, string language = null, string userName = null)
        //{
        //    var request = new SearchRepositoriesRequest
        //    {
        //        Language = language,
        //        User = userName
        //    };

        //    if (!string.IsNullOrEmpty(repoName))
        //    {
        //        request.Query = repoName;
        //    }

        //    var searchResult = await _publicClient.Search.SearchRepo(request);

        //    return searchResult.Items.Select(repo => new SearchRepositoryInfo
        //    {
        //        Name = repo.Name,
        //        Url = repo.HtmlUrl,
        //        Owner = repo.Owner.Login,
        //        Language = repo.Language,
        //        Stars = repo.StargazersCount
        //    }).ToList();
        //}

      
        public async Task<List<SearchRepositoryInfo>> SearchRepositories(string repoName = null, string language = null, string userName = null)
        {
            // בניית מחרוזת החיפוש
            var searchQuery = "";

            if (!string.IsNullOrWhiteSpace(repoName))
            {
                searchQuery += repoName + " ";  // הוספת שם הריפוזיטורי לשאילתה
            }

            if (!string.IsNullOrWhiteSpace(userName))
            {
                searchQuery += $"user:{userName} ";  // הוספת שם המשתמש לשאילתה
            }

            if (!string.IsNullOrWhiteSpace(language))
            {
                searchQuery += $"language:{language} ";  // הוספת שפת הפיתוח לשאילתה
            }

            // יצירת בקשת חיפוש עם השאילתה שנבנתה
            var request = new SearchRepositoriesRequest(searchQuery.Trim());

            // ביצוע החיפוש ב-GitHub
            var searchResult = await _publicClient.Search.SearchRepo(request);

            // המרת התוצאה לרשימה ידידותית יותר
            return searchResult.Items.Select(repo => new SearchRepositoryInfo
            {
                Name = repo.Name,
                Url = repo.HtmlUrl,
                Owner = repo.Owner.Login,
                Language = repo.Language,
                Stars = repo.StargazersCount
            }).ToList();
        }

    }
}
