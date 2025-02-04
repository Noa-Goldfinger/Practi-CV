
using APICV.CashedServices;
using Microsoft.Extensions.Caching.Memory;
using Service;

namespace APICV
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            builder.Services.Configure<GitHubIntegrationOptions>(builder.Configuration.GetSection("GitHubIntegrationOptions"));
            //builder.Configuration.AddUserSecrets<Program>(); // Enables secrets.json

            builder.Services.AddMemoryCache();
            //builder.Services.AddScoped<IGitHubService, GitHubService>();
            //builder.Services.Decorate<IGitHubService, CashedGitHubService>();

            builder.Services.AddScoped<GitHubService>(); 
            builder.Services.AddScoped<IGitHubService, GitHubService>();
            builder.Services.AddScoped<CashedGitHubService>();

            // מחליפים את IGitHubService בגרסה עם Cache
            builder.Services.AddScoped<IGitHubService>(provider =>
            {
                var gitHubService = provider.GetRequiredService<GitHubService>(); 
                var memoryCache = provider.GetRequiredService<IMemoryCache>();

                return new CashedGitHubService(gitHubService, memoryCache);
            });


            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}