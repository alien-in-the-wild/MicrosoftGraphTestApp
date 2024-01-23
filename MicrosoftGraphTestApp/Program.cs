using System.Text.Json.Nodes;
using MicrosoftGraphTestApp.GraphData;

namespace MicrosoftGraphTestApp
{
    class Program
    {
        static async Task Main(string[] _)
        {
            GraphHelper graphHelper;
            Settings settings;
            
            try
            {
                // Initialize Graph
                settings = Settings.LoadSettings();
                graphHelper = new GraphHelper(settings);
            }
            catch (Exception e)
            {
                Console.WriteLine("Error initializing Graph: " + e.Message);
                return;
            }
            
            int choice = -1;

            while (choice != 0)
            {
                Console.WriteLine("Please choose one of the following options:");
                Console.WriteLine("0. Exit");
                Console.WriteLine("1. Download groups");

                try
                {
                    choice = int.Parse(Console.ReadLine() ?? string.Empty);
                }
                catch (FormatException)
                {
                    // Set to invalid value
                    choice = -1;
                }

                switch(choice)
                {
                    case 0:
                        // Exit the program
                        Console.WriteLine("Goodbye...");
                        break;
                    case 1:
                        // Download groups
                        await DownloadGroupsAsync(settings, graphHelper);
                        break;
                    default:
                        Console.WriteLine("Invalid choice! Please try again.");
                        break;
                }
            }
        }

        private static async Task DownloadGroupsAsync(Settings settings, GraphHelper graphHelper)
        {
            try
            {
                var groupsResponse = await graphHelper.GetGroupsAsync();
                if (groupsResponse == null || groupsResponse.Value == null)
                    throw new NullReferenceException("Error getting groups response.");
            
                string path = Path.Combine(settings.SaveGroupsPath, "/MSGraph/Groups");
                Directory.CreateDirectory(path);
            
                JsonNode? jsonNode = JsonNode.Parse(groupsResponse.Response);
                if (jsonNode != null)
                {
                    Console.WriteLine($"Groups count: {groupsResponse.Value.Count}");
                    JsonArray jsonGroups = jsonNode["value"]!.AsArray();

                    for (int nGroup = 0; nGroup < groupsResponse.Value.Count; nGroup++)
                    {
                        Group group = groupsResponse.Value[nGroup];
                        string filePath = Path.Combine(path, $"{group.DisplayName}.json");
                        string groupJsonText = jsonGroups[nGroup]!.ToJsonString();
                    
                        await File.WriteAllTextAsync(filePath, groupJsonText);
                        Console.WriteLine($"Group {nGroup+1}: {filePath}");
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine("Error downloading groups: " + e.Message);
            }
        }
    }
}