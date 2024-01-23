using Microsoft.Extensions.Configuration;

namespace MicrosoftGraphTestApp
{
    public class Settings
    {
        public required string ClientId { get; init; }
        public required string GrantType { get; init; }
        public required string Scope { get; init; }
        public required string ClientSecret { get; init; }
        public required string IdentityApiUrl { get; init; }
        public required string GraphApiUrl { get; init; }
        public required string SaveGroupsPath { get; init; }
        
        public static Settings LoadSettings()
        {
            // Load settings
            IConfiguration config = new ConfigurationBuilder()
                // appsettings.json is required
                .AddJsonFile("appsettings.json", optional: false)
                // appsettings.Development.json" is optional, values override appsettings.json
                .AddJsonFile($"appsettings.Development.json", optional: true)
                // User secrets are optional, values override both JSON files
                .AddUserSecrets<Program>()
                .Build();

            return config.GetRequiredSection("Settings").Get<Settings>() ??
                   throw new Exception("Could not load app settings. See README for configuration instructions.");
        }
    }
}